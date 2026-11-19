package ec.edu.uta.venta_vehiculos_api.repository;

// Importa la clase Optional y List para manejar valores opcionales y listas.
import java.util.*;

// Importa LockModeType para definir el tipo de bloqueo de base de datos.
import jakarta.persistence.LockModeType;

// Importa las interfaces y anotaciones de Spring Data JPA.
import org.springframework.data.jpa.repository.*;

// Importa Param para poder enlazar parámetros en una query JPQL.
import org.springframework.data.repository.query.Param;

// Importa la entidad Vehiculo del modelo.
import ec.edu.uta.venta_vehiculos_api.model.Vehiculo;

// Declara la interfaz del repositorio de vehículos.
// JpaRepository<Vehiculo, Integer> ya incluye operaciones CRUD básicas.
public interface VehiculoRepository

    extends JpaRepository<Vehiculo, Integer> {

    // Busca un vehículo por su placa y devuelve un Optional.
    // Optional evita errores si no existe ningún vehículo con esa placa.
    Optional<Vehiculo> findByPlaca(String placa);

    // Busca un vehículo por el número de chasis.
    Optional<Vehiculo> findByChasis(String chasis);

    // Busca vehículos cuya marca contenga el texto indicado, ignorando mayúsculas/minúsculas.
    List<Vehiculo> findByMarcaContainingIgnoreCase(String marca);

    // Marca esta consulta con bloqueo pesimista de escritura.
    @Lock(LockModeType.PESSIMISTIC_WRITE)

    // Define una consulta JPQL para obtener un vehículo por su ID.
    @Query("select v from Vehiculo v where v.idVehiculo = :id")

    // Devuelve un Optional con el vehículo bloqueado por ID.
    Optional<Vehiculo> bloquear(@Param("id") Integer id);

}
