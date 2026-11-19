using System.Text.Json.Serialization;

namespace ConsultaVehiculosForms.DTOs
{
    public class CrearVentaDto
    {
        [JsonPropertyName("vehiculoId")]
        public int VehiculoId { get; set; }

        [JsonPropertyName("compradorId")]
        public int CompradorId { get; set; }
    }

    public class VentaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("operacionId")]
        public string OperacionId { get; set; } = string.Empty;

        [JsonPropertyName("vehiculoId")]
        public int VehiculoId { get; set; }

        [JsonPropertyName("compradorId")]
        public int CompradorId { get; set; }

        [JsonPropertyName("precioAcordado")]
        public string? PrecioAcordado { get; set; }

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "USD";

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("notificacion")]
        public string Notificacion { get; set; } = string.Empty;

        [JsonPropertyName("fechaUTC")]
        public string FechaUTC { get; set; } = string.Empty;
    }
}
