using System.Text.Json.Serialization;

namespace ConsultaVehiculosForms.DTOs
{
    public class ComprobanteCompradorDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("documento")]
        public string Documento { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;
    }

    public class ComprobanteVehiculoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        [JsonPropertyName("modelo")]
        public string? Modelo { get; set; }

        [JsonPropertyName("anio")]
        public int? Anio { get; set; }
    }

    public class ComprobanteVentaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("operacionId")]
        public string OperacionId { get; set; } = string.Empty;

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("precioConfirmado")]
        public decimal PrecioConfirmado { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "USD";

        [JsonPropertyName("comprador")]
        public ComprobanteCompradorDto Comprador { get; set; } = new();

        [JsonPropertyName("vehiculo")]
        public ComprobanteVehiculoDto Vehiculo { get; set; } = new();
    }

    public class ResumenVentasDto
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [JsonPropertyName("total_ventas")]
        public int TotalVentas { get; set; }

        [JsonPropertyName("completadas")]
        public int Completadas { get; set; }

        [JsonPropertyName("pendientes")]
        public int Pendientes { get; set; }

        [JsonPropertyName("fallidas")]
        public int Fallidas { get; set; }

        [JsonPropertyName("en_revision")]
        public int EnRevision { get; set; }

        [JsonPropertyName("total_ingresos")]
        public decimal TotalIngresos { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "USD";

        [JsonPropertyName("por_estado")]
        public Dictionary<string, int>? PorEstado { get; set; }
    }
}
