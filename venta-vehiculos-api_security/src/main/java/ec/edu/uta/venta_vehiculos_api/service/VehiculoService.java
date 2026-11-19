package ec.edu.uta.venta_vehiculos_api.service;

// Importa la clase para manejar valores monetarios.
import java.math.BigDecimal;

// Importa la clase para obtener el año actual del sistema.
import java.time.Year;

// Importa utilidades de colecciones y mapas.
import java.util.*;

// Marca la clase como un servicio de Spring para inyección de dependencias.
import org.springframework.stereotype.Service;

// Permite controlar transacciones de base de datos.
import org.springframework.transaction.annotation.Transactional;

// Importa los DTOs usados en la capa de servicio.
import ec.edu.uta.venta_vehiculos_api.dto.*;

// Importa la clase personalizada de errores de la API.
import ec.edu.uta.venta_vehiculos_api.exception.ApiError;

// Importa la entidad Vehiculo que se persiste en base de datos.
import ec.edu.uta.venta_vehiculos_api.model.Vehiculo;

// Importa repositorios para acceder a datos.
import ec.edu.uta.venta_vehiculos_api.repository.*;

// Indica que esta clase es un servicio gestionado por Spring.
@Service

// Hace que todas las operaciones de esta clase se ejecuten dentro de una transacción.
@Transactional
public class VehiculoService {

    // Repositorio para manejar la entidad Vehiculo.
    private final VehiculoRepository repo;

    // Repositorio para manejar reservas relacionadas con vehículos.
    private final ReservaRepository reservas;

    // Constructor inyectado por Spring con las dependencias requeridas.
    public VehiculoService(
            VehiculoRepository repo,
            ReservaRepository reservas) {

        // Asigna el repositorio de vehículos.
        this.repo = repo;

        // Asigna el repositorio de reservas.
        this.reservas = reservas;
    }

    // Consulta de solo lectura: lista vehículos con filtro opcional por marca.
    @Transactional(readOnly = true)
    public List<VehiculoResponse> listar(String marca) {

        // Si marca es nula, trae todos los vehículos; si no, trae los que contengan la marca.
        var lista = marca == null
                ? repo.findAll()
                : repo.findByMarcaContainingIgnoreCase(marca);

        // Convierte cada entidad Vehiculo a su DTO de respuesta.
        return lista.stream()
                .map(VehiculoResponse::de)
                .toList();
    }

    // Consulta de solo lectura: obtiene un vehículo por su ID.
    @Transactional(readOnly = true)
    public VehiculoResponse obtener(int id) {

        // Busca el vehículo y lanza una excepción si no existe.
        return VehiculoResponse.de(
                repo.findById(id)
                    .orElseThrow(() -> falta())
        );
    }

    // Crea un nuevo vehículo con la información recibida.
    public VehiculoResponse crear(VehiculoRequest d) {

        // Crea una nueva entidad Vehiculo vacía.
        Vehiculo v = new Vehiculo();

        // Copia los datos del DTO a la entidad.
        copiar(v, d);

        // Establece el estado inicial del vehículo como disponible.
        v.setEstado("DISPONIBLE");

        // Guarda y devuelve la entidad convertida a respuesta.
        return VehiculoResponse.de(repo.saveAndFlush(v));
    }

    // Actualiza un vehículo existente con la información recibida.
    public VehiculoResponse actualizar(int id, VehiculoRequest d) {

        // Busca el vehículo editable y valida que esté disponible.
        Vehiculo v = editable(id);

        // Copia los nuevos datos sobre la entidad existente.
        copiar(v, d);

        // Guarda los cambios en base de datos y devuelve el resultado.
        return VehiculoResponse.de(repo.saveAndFlush(v));
    }

    // Actualiza solo el precio de un vehículo mediante un mapa con la clave "precio".
    public VehiculoResponse precio(int id, Map<String, Object> d) {

        // Verifica que el mapa tenga exactamente un campo y que ese campo sea numérico.
        if (d.size() != 1 ||
                !d.containsKey("precio") ||
                !(d.get("precio") instanceof Number)) {

            // Lanza error 422 si el payload no cumple el formato esperado.
            throw new ApiError(
                    422,
                    "PATCH_INVALIDO",
                    "PATCH admite únicamente precio numérico y no nulo"
            );
        }

        // Variable para almacenar el precio convertido a BigDecimal.
        BigDecimal precio;

        // Intenta convertir el valor recibido a BigDecimal.
        try {

            precio = new BigDecimal(d.get("precio").toString());

        } catch (NumberFormatException e) {

            // Si el valor no puede convertirse, devuelve un error de validación.
            throw new ApiError(
                    422,
                    "PRECIO_INVALIDO",
                    "Precio inválido"
            );
        }

        // Valida que el precio sea positivo, tenga máximo dos decimales y no exceda el máximo.
        if (precio.signum() <= 0 ||
                precio.scale() > 2 ||
                precio.compareTo(
                    new BigDecimal("9999999999.99")) > 0) {

            // Lanza error si el precio no cumple las reglas de negocio.
            throw new ApiError(
                    422,
                    "PRECIO_INVALIDO",
                    "Precio positivo con máximo dos decimales"
            );
        }

        // Recupera el vehículo en modo bloqueado para evitar inconsistencias concurrentes.
        Vehiculo v = editable(id);

        // Asigna el nuevo precio al vehículo.
        v.setPrecio(precio);

        // Guarda y devuelve el vehículo actualizado.
        return VehiculoResponse.de(repo.saveAndFlush(v));
    }

    // Elimina un vehículo solo si no tiene historial de reservas.
    public void eliminar(int id) {

        // Obtiene el vehículo editable y valida su estado.
        Vehiculo v = editable(id);

        // Verifica si hay reservas registradas para ese vehículo.
        if (reservas.existsByVehiculo_IdVehiculo(id)) {

            // Si existe historial, no permite eliminar para preservar integridad.
            throw new ApiError(
                    409,
                    "HISTORIAL_RESERVAS",
                    "Se conserva el vehículo por su historial de reservas"
            );
        }

        // Elimina el vehículo de la base de datos.
        repo.delete(v);

        // Fuerza la sincronización con la base de datos.
        repo.flush();
    }

    // Valida que el vehículo exista y esté disponible para editar o eliminar.
    private Vehiculo editable(int id) {

        // Bloquea la fila para evitar conflictos concurrentes y busca el vehículo por ID.
        Vehiculo v = repo.bloquear(id)
                .orElseThrow(() -> falta());

        // Si el vehículo no está disponible, no se permite modificarlo ni borrarlo.
        if (!"DISPONIBLE".equals(v.getEstado())) {

            // Lanza conflicto de negocio cuando el vehículo está reservado o vendido.
            throw new ApiError(
                    409,
                    "VEHICULO_NO_DISPONIBLE",
                    "Un vehículo reservado o vendido no se modifica ni elimina"
            );
        }

        // Devuelve el vehículo validado.
        return v;
    }

    // Crea el error estándar cuando el vehículo no existe.
    private ApiError falta() {

        // Retorna un error 404 con código y mensaje propios de la API.
        return new ApiError(
                404,
                "VEHICULO_NO_ENCONTRADO",
                "El vehículo no existe"
        );
    }

    // Copia los datos del DTO a una entidad Vehiculo y valida la información.
    private void copiar(Vehiculo v, VehiculoRequest d) {

        // Valida que el año no sea menor a 1900 ni mayor a 1 año más del vigente.
        if (d.anio() < 1900 ||
                d.anio() > Year.now().getValue() + 1) {

            // Error 422 si el año queda fuera del rango permitido.
            throw new ApiError(
                    422,
                    "ANIO_INVALIDO",
                    "Año fuera de rango"
            );
        }

        // Elimina espacios al inicio y fin, y guarda la marca.
        v.setMarca(d.marca().trim());

        // Elimina espacios y guarda el modelo.
        v.setModelo(d.modelo().trim());

        // Normaliza la placa a mayúsculas y guarda.
        v.setPlaca(
                d.placa().trim().toUpperCase(Locale.ROOT)
        );

        // Normaliza el chasis a mayúsculas y guarda.
        v.setChasis(
                d.chasis().trim().toUpperCase(Locale.ROOT)
        );

        // Verifica que el número de chasis tenga exactamente 17 caracteres.
        if (v.getChasis().length() != 17) {

            // Error si el chasis no cumple la longitud esperada.
            throw new ApiError(
                    422,
                    "CHASIS_INVALIDO",
                    "Chasis de 17 caracteres"
            );
        }

        // Asigna el año del vehículo.
        v.setAnio(d.anio());

        // Asigna el color del vehículo.
        v.setColor(d.color());

        // Asigna el precio del vehículo.
        v.setPrecio(d.precio());
    }

}
