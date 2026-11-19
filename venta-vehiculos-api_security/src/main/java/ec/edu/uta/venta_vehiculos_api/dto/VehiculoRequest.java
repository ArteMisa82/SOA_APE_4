package ec.edu.uta.venta_vehiculos_api.dto;

// Importa la clase para manejar valores monetarios.
import java.math.BigDecimal;

// Importa la anotación de validación de nulidad.
// Nota: en Spring Boot/Jakarta se usa normalmente jakarta.validation.constraints.NotNull,
// no org.antlr.v4.runtime.misc.NotNull. Esta importación puede ser incorrecta según la versión.
import org.antlr.v4.runtime.misc.NotNull;

// Importa las anotaciones de validación que se aplican al registro.
import jakarta.validation.constraints.*;

// Define un DTO (Data Transfer Object) para recibir la información de creación/actualización de un vehículo.
// Un record es una clase inmutable con acceso directo a sus campos por nombre.
public record VehiculoRequest(
    
    // La marca es obligatoria y no puede estar vacía ni nula.
    @NotBlank String marca,

    // El modelo es obligatorio y no puede estar vacío.
    @NotBlank String modelo,

    // La placa es obligatoria y no puede estar vacía.
    @NotBlank String placa,

    // El chasis es obligatorio, no puede estar vacío y debe tener exactamente 17 caracteres.
    @NotBlank @Size(min=17, max=17) String chasis,

    // El año es obligatorio.
    @NotNull Integer anio,

    // El color puede ser nulo o vacío, según el requisito del negocio.
    String color,

    // El precio es obligatorio, debe ser positivo y admite máximo 2 decimales.
    @NotNull @Positive @Digits(integer=10, fraction=2)
    BigDecimal precio
) {
    
}
