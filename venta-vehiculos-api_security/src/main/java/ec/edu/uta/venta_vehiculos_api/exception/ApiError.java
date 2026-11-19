package ec.edu.uta.venta_vehiculos_api.exception;

// Define la clase ApiError dentro del paquete de excepciones de la API.
public class ApiError extends RuntimeException {

    // Almacena el código HTTP asociado al error, por ejemplo 400, 404 o 500.
    public final int status;

    // Guarda un identificador interno del tipo de error para facilitar el manejo y diagnóstico.
    public final String codigo;

    // Constructor de la excepción personalizada.
    // Recibe el código HTTP, el código interno del error y el detalle que se mostrará al cliente.
    public ApiError(int status, String codigo, String detalle) {
        // Llama al constructor de RuntimeException para guardar el mensaje detallado del error.
        super(detalle);

        // Asigna el estado HTTP recibido a la propiedad status de la instancia.
        this.status = status;

        // Asigna el código interno del error a la propiedad codigo.
        this.codigo = codigo;
    }
}

