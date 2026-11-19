package ec.edu.uta.venta_vehiculos_api.dto;

// DTO para representar una notificación que se envía sobre un vehículo.
public class NotificacionRequest {

    // Identificador del vehículo relacionado con la notificación.
    private Integer vehiculoId;

    // Destinatario de la notificación.
    private String destinatario;

    // Tipo de evento: registro, actualización, eliminación, etc.
    private String tipo;

    // Mensaje descriptivo de la notificación.
    private String mensaje;

    // Constructor vacío requerido por frameworks de serialización/deserialización.
    public NotificacionRequest() {
    }

    // Constructor completo para crear una notificación con todos sus datos.
    public NotificacionRequest(
            Integer vehiculoId,
            String destinatario,
            String tipo,
            String mensaje) {

        this.vehiculoId = vehiculoId;
        this.destinatario = destinatario;
        this.tipo = tipo;
        this.mensaje = mensaje;
    }

    // Retorna el identificador del vehículo.
    public Integer getVehiculoId() {
        return vehiculoId;
    }

    // Asigna el identificador del vehículo.
    public void setVehiculoId(Integer vehiculoId) {
        this.vehiculoId = vehiculoId;
    }

    // Retorna el destinatario.
    public String getDestinatario() {
        return destinatario;
    }

    // Asigna el destinatario.
    public void setDestinatario(String destinatario) {
        this.destinatario = destinatario;
    }

    // Retorna el tipo de la notificación.
    public String getTipo() {
        return tipo;
    }

    // Asigna el tipo de notificación.
    public void setTipo(String tipo) {
        this.tipo = tipo;
    }

    // Retorna el mensaje de la notificación.
    public String getMensaje() {
        return mensaje;
    }

    // Asigna el mensaje de la notificación.
    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }
}