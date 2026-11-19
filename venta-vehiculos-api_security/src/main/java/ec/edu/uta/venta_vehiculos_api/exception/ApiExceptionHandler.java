package ec.edu.uta.venta_vehiculos_api.exception;

// Importa la clase URI para crear identificadores tipo URL/URN.
import java.net.URI;

// Importa UUID para generar un identificador único por cada error.
import java.util.UUID;

// Importa LoggerFactory para registrar errores en el log de la aplicación.
import org.slf4j.LoggerFactory;

// Importa la excepción que ocurre cuando hay conflicto de integridad en la base de datos.
import org.springframework.dao.DataIntegrityViolationException;

// Importa clases HTTP y ResponseEntity para devolver respuestas HTTP.
import org.springframework.http.*;

// Importa la excepción que ocurre cuando el JSON no se puede leer o convertir.
import org.springframework.http.converter.HttpMessageNotReadableException;

// Importa la excepción de validación de argumentos en Spring MVC.
import org.springframework.web.bind.MethodArgumentNotValidException;

// Importa las anotaciones de Spring para controladores y manejo de excepciones.
import org.springframework.web.bind.annotation.*;

// Importa ErrorResponse para trabajar con errores HTTP estándar de Spring.
import org.springframework.web.ErrorResponse;

// Importa la clase HttpServletRequest para obtener información de la petición actual.
import jakarta.servlet.http.HttpServletRequest;

// Indica que esta clase manejará excepciones de todos los controladores REST de la API.
@RestControllerAdvice
public class ApiExceptionHandler {

    // Este método captura cualquier excepción que no haya sido manejada antes.
    @ExceptionHandler(Exception.class)
    public ResponseEntity<ProblemDetail> manejar(
            Exception ex, HttpServletRequest req) {

        // Código HTTP por defecto: 500 Internal Server Error.
        int status = 500;

        // Código interno del error por defecto.
        String codigo = "ERROR_INTERNO";

        // Mensaje detallado por defecto para errores no identificados.
        String detalle = "No fue posible completar la solicitud";

        // Si la excepción es una ApiError personalizada, se toma su status, código y detalle.
        if (ex instanceof ApiError e) {
            status = e.status;
            codigo = e.codigo;
            detalle = e.getMessage();

        // Si la excepción es de validación de argumentos, por ejemplo @Valid en un DTO.
        } else if (ex instanceof MethodArgumentNotValidException e) {

            // 422: Unprocessable Entity (validación fallida).
            status = 422;

            // Código específico para errores de validación.
            codigo = "VALIDACION";

            // Recorre todos los errores de campos y los convierte en un texto legible.
            detalle = e.getBindingResult().getFieldErrors().stream()
                .map(x -> x.getField() + ": " + x.getDefaultMessage())
                .reduce((a,b) -> a + "; " + b)
                .orElse("Datos inválidos");

        // Si el JSON viene mal formado o no coincide con el tipo esperado.
        } else if (ex instanceof HttpMessageNotReadableException) {

            // 400: Bad Request.
            status = 400;

            // Código para JSON inválido.
            codigo = "JSON_INVALIDO";

            // Mensaje amigable para el cliente.
            detalle = "JSON ilegible o tipo de dato incorrecto";

        // Si la base de datos rechaza la operación por duplicado o integridad.
        } else if (ex instanceof DataIntegrityViolationException) {

            // 409: Conflict.
            status = 409;

            // Código para conflicto de datos.
            codigo = "CONFLICTO_DATOS";

            // Mensaje general.
            detalle = "Duplicado o relación que impide la operación";

        // Si se lanzó una IllegalArgumentException.
        } else if (ex instanceof IllegalArgumentException) {

            // 400: Bad Request.
            status = 400;

            // Código para solicitud inválida.
            codigo = "SOLICITUD_INVALIDA";

            // Se devuelve el mensaje exacto del error.
            detalle = ex.getMessage();

        // Si la excepción ya es un ErrorResponse de Spring.
        } else if (ex instanceof ErrorResponse e) {

            // Se toma el status real del ErrorResponse.
            status = e.getStatusCode().value();

            // Código general para errores HTTP.
            codigo = "ERROR_HTTP";

            // Se usa el detalle que trae la excepción.
            detalle = e.getBody().getDetail();
        }

        // Genera un identificador único para rastrear este error.
        String trace = UUID.randomUUID().toString();

        // Si el error es del servidor (500+), se registra en el log.
        if (status >= 500)
            LoggerFactory.getLogger(getClass())
                .error("Error " + trace, ex);

        // Crea un objeto estándar para errores HTTP (ProblemDetail).
        ProblemDetail p = ProblemDetail.forStatusAndDetail(
            HttpStatusCode.valueOf(status), detalle);

        // Define un identificador tipo URN para el tipo de error.
        // Ejemplo: urn:vehiculos:validacion
        p.setType(URI.create("urn:vehiculos:" + codigo.toLowerCase()));

        // Agrega la ruta exacta de la petición que falló.
        p.setInstance(URI.create(req.getRequestURI()));

        // Añade el código interno del error como propiedad personalizada.
        p.setProperty("codigo", codigo);

        // Añade el traceId para seguimiento.
        p.setProperty("traceId", trace);

        // Devuelve la respuesta HTTP con el contenido en formato JSON de problema.
        return ResponseEntity.status(status)
            .contentType(MediaType.APPLICATION_PROBLEM_JSON)
            .body(p);
    }
}
