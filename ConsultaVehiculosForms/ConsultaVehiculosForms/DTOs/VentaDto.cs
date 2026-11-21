using System.Text.Json.Serialization;

namespace ConsultaVehiculosForms.DTOs
{
    /// <summary>
    /// DTO de solo lectura para consultar ventas registradas desde FastAPI (GET /api/ventas).
    /// Windows Forms no registra ventas (exclusivo de la PWA móvil).
    /// </summary>
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
        public string? Moneda { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("notificacion")]
        public string Notificacion { get; set; } = string.Empty;

        [JsonPropertyName("fechaUTC")]
        public string FechaUTC { get; set; } = string.Empty;
    }
}
