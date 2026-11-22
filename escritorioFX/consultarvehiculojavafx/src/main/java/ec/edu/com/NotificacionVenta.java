package ec.edu.com;

    import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

    @JsonIgnoreProperties(ignoreUnknown = true)
    public record NotificacionVenta(
            String eventoId,
            String tipo,
            Integer ventaId,
            Integer vehiculoId,
            Integer compradorId,
            String precioAcordado,
            String moneda,
            String estado,
            String fechaHora,
            String mensaje,
            String recibidoEn
    ) {}