package ec.edu.uta.venta_vehiculos_api.dto;

// DTO que representa la respuesta de una operación de notificación.
public class NotificacionResponse {

    // Indica si la operación fue exitosa.
    private boolean success;

    // Estado de la operación, por ejemplo: OK, ERROR, PENDIENTE.
    private String estado;

    // Mensaje descriptivo para el cliente.
    private String mensaje;

    // Constructor vacío necesario para serialización/deserialización.
    public NotificacionResponse(){

    }

    // Asigna el resultado de la operación.
    public void setSuccess(boolean success) {
        this.success = success;
    }

    // Asigna el estado de la operación.
    public void setEstado(String estado) {
        this.estado = estado;
    }

    // Asigna el mensaje de respuesta.
    public void setMensaje(String mensaje) {
        this.mensaje = mensaje;
    }

    // Retorna si la operación tuvo éxito.
    public boolean isSuccess() {
        return success;
    }

    // Retorna el estado de la operación.
    public String getEstado() {
        return estado;
    }

    // Retorna el texto explicativo de la respuesta.
    public String getMensaje() {
        return mensaje;
    }
}
