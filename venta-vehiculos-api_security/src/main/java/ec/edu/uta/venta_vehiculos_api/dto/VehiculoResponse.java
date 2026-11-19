package ec.edu.uta.venta_vehiculos_api.dto;

// Importa el tipo monetario.
import java.math.BigDecimal;

// Importa la entidad Vehiculo para convertirla a respuesta.
import ec.edu.uta.venta_vehiculos_api.model.Vehiculo;

// DTO que representa la información pública de un vehículo para enviar al cliente.
public record VehiculoResponse(Integer idVehiculo,

    String marca, String modelo,

    String placa, String chasis, Integer anio,

    String color, BigDecimal precio, String estado) {

        // Convierte una entidad Vehiculo a su DTO de respuesta.
        public static VehiculoResponse de(Vehiculo v) {

        return new VehiculoResponse(

            v.getIdVehiculo(),
            v.getMarca(),
            v.getModelo(),
            v.getPlaca(),
            v.getChasis(),
            v.getAnio(),
            v.getColor(),
            v.getPrecio(),
            v.getEstado()
        );
    }
}
