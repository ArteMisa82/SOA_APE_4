package ec.edu.uta.venta_vehiculos_api.dto;

// Importa el tipo monetario usado en la respuesta.
import java.math.BigDecimal;

// Importa la entidad Reserva para convertirla a DTO.
import ec.edu.uta.venta_vehiculos_api.model.Reserva;

// DTO que representa la respuesta de una reserva con información resumida.
public record ReservaResponse(String id, String operacionId, Integer vehiculoId,
 BigDecimal precio, String moneda, String estado
) {
    // Convierte una entidad Reserva en su representación de respuesta.
    public static ReservaResponse de(Reserva r) {

        return new ReservaResponse(
                r.id,
                r.operacionId,
                r.vehiculo.getIdVehiculo(),
                r.precio,
                r.moneda,
                r.estado
        );
    }
}
