package ec.edu.uta.venta_vehiculos_api.dto;

// Importa la clase para manejar valores monetarios.
import java.math.BigDecimal;

// Importa las anotaciones estándar de Jakarta Bean Validation (incluyendo @NotNull).
import jakarta.validation.constraints.*;

// Define un DTO (Data Transfer Object) para recibir la información de creación/actualización de un vehículo.
// Un record es una clase inmutable con acceso directo a sus campos por nombre.
public record VehiculoRequest(

    // La marca es obligatoria, con longitud controlada y sin símbolos inválidos.
    @NotBlank(message = "La marca es obligatoria")
    @Size(min = 2, max = 60, message = "La marca debe tener entre 2 y 60 caracteres")
    @Pattern(regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñÜü0-9 .\\-]+$", message = "La marca contiene caracteres no permitidos")
    String marca,

    // El modelo es obligatorio, con longitud controlada y sin símbolos inválidos.
    @NotBlank(message = "El modelo es obligatorio")
    @Size(min = 1, max = 60, message = "El modelo debe tener entre 1 y 60 caracteres")
    @Pattern(regexp = "^[A-Za-zÁÉÍÓÚáéíóúÑñÜü0-9 ./\\-]+$", message = "El modelo contiene caracteres no permitidos")
    String modelo,

    // La placa es obligatoria, alfanumérica con guion opcional.
    @NotBlank(message = "La placa es obligatoria")
    @Size(min = 4, max = 15, message = "La placa debe tener entre 4 y 15 caracteres")
    @Pattern(regexp = "^[A-Za-z0-9\\-]+$", message = "La placa solo admite letras, números y guion")
    String placa,

    // El chasis es obligatorio y debe tener exactamente 17 caracteres alfanuméricos.
    @NotBlank(message = "El chasis es obligatorio")
    @Size(min = 17, max = 17, message = "El chasis debe tener exactamente 17 caracteres")
    @Pattern(regexp = "^[A-Za-z0-9]{17}$", message = "El chasis debe tener exactamente 17 caracteres alfanuméricos")
    String chasis,

    // El año es obligatorio (el rango exacto se valida además en VehiculoService).
    @NotNull(message = "El año es obligatorio")
    Integer anio,

    // El color es opcional en el API, pero si se envía debe cumplir longitud y caracteres válidos.
    @Size(max = 40, message = "El color no debe superar 40 caracteres")
    @Pattern(regexp = "^$|^[A-Za-zÁÉÍÓÚáéíóúÑñÜü0-9 .\\-]+$", message = "El color contiene caracteres no permitidos")
    String color,

    // El precio es obligatorio, debe ser positivo y admite máximo 2 decimales.
    @NotNull(message = "El precio es obligatorio")
    @Positive(message = "El precio debe ser mayor a cero")
    @Digits(integer = 10, fraction = 2, message = "El precio admite hasta 10 dígitos enteros y 2 decimales")
    BigDecimal precio
) {

}
