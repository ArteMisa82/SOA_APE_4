package ec.edu.uta.venta_vehiculos_api.controller;

// Importa la clase para crear respuestas HTTP con códigos de estado.
import java.net.URI;

// Importa utilidades para trabajar con mapas y listas.
import java.util.*;

// Importa la validación de Bean Validation para validar entrada del cliente.
import jakarta.validation.Valid;

import org.springframework.beans.factory.annotation.Value;
// Importa la respuesta HTTP de Spring.
import org.springframework.http.ResponseEntity;

// Importa las anotaciones del controlador REST.
import org.springframework.web.bind.annotation.*;

// Importa los DTOs del proyecto.
import ec.edu.uta.venta_vehiculos_api.dto.*;

// Importa el servicio de vehículos.
import ec.edu.uta.venta_vehiculos_api.service.VehiculoService;

// Importa el cliente externo para enviar notificaciones.
import ec.edu.uta.venta_vehiculos_api.client.NotificacionClient;

// Define este controlador como un endpoint REST para vehículos.
@RestController

// Establece la ruta base de todos los endpoints de este controlador.
@RequestMapping("/api/vehiculos")
public class VehiculoController {

    // Servicio que contiene la lógica de negocio relacionada con vehículos.
    private final VehiculoService service;

    // Cliente para enviar mensajes/notificaciones cuando cambia un vehículo.
    private final NotificacionClient avisos;

    @Value("${server.port:8080}")
    private String puertoInstancia;

    // Constructor con inyección de dependencias de Spring.
    public VehiculoController(
            VehiculoService service,
            NotificacionClient avisos) {

        this.service = service;
        this.avisos = avisos;
    }

    // Identifica qué instancia (8080 o 8082) atendió la solicitud detrás de NGINX.
    @GetMapping("/instancia")
    public Map<String, Object> obtenerInstancia() {
        return Map.of(
                "servicio", "venta-vehiculos-api",
                "instancia", "API-" + puertoInstancia,
                "puerto", Integer.parseInt(puertoInstancia)
        );
    }

    // Obtiene la lista de vehículos opcionalmente filtrada por marca.
    @GetMapping
    public List<VehiculoResponse> listar(
            @RequestParam(required = false) String marca) {

        return service.listar(marca);
    }

    // Obtiene un vehículo por su ID.
    @GetMapping("/{id}")
    public VehiculoResponse obtener(
            @PathVariable int id) {

        return service.obtener(id);
    }

    // Crea un nuevo vehículo con validación del cuerpo JSON.
    @PostMapping
    public ResponseEntity<VehiculoResponse> crear(
            @Valid @RequestBody VehiculoRequest d) {

        // Guarda el vehículo y obtiene la respuesta del servicio.
        var r = service.crear(d);

        // Envía una notificación de registro.
        avisos.enviar(
                r.idVehiculo(),
                "REGISTRO",
                "Vehículo registrado"
        );

        // Devuelve 201 Created con la ubicación del recurso recién creado.
        return ResponseEntity.created(
                URI.create("/api/vehiculos/" + r.idVehiculo())
        ).body(r);
    }

    // Actualiza un vehículo existente por su ID.
    @PutMapping("/{id}")
    public VehiculoResponse actualizar(
            @PathVariable int id,
            @Valid @RequestBody VehiculoRequest d) {

        var r = service.actualizar(id, d);

        // Envía una notificación de actualización.
        avisos.enviar(
                id,
                "ACTUALIZACION",
                "Vehículo actualizado"
        );

        return r;
    }

    // Actualiza el precio del vehículo usando PATCH con formato merge-patch.
    @PatchMapping(
            value = "/{id}",
            consumes = "application/merge-patch+json"
    )
    public VehiculoResponse precio(
            @PathVariable int id,
            @RequestBody Map<String, Object> d) {

        var r = service.precio(id, d);

        // Envía notificación cuando cambia el precio.
        avisos.enviar(
                id,
                "ACTUALIZACION",
                "Precio actualizado"
        );

        return r;
    }

    // Elimina un vehículo por su ID.
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> eliminar(
            @PathVariable int id) {

        service.eliminar(id);

        // Envía notificación de eliminación.
        avisos.enviar(
                id,
                "ELIMINACION",
                "Vehículo eliminado"
        );

        // Devuelve una respuesta vacía con código 204 No Content.
        return ResponseEntity.noContent().build();
    }

}
