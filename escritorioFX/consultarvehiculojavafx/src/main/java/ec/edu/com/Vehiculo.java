package ec.edu.com;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import java.math.BigDecimal;

@JsonIgnoreProperties(ignoreUnknown = true)
public record Vehiculo(
    int idVehiculo, 
    String marca, 
    String modelo,
    String placa,
    String chasis,
    @JsonProperty("anio") int anio, 
    String color,
    BigDecimal precio,
    String estado) {}
