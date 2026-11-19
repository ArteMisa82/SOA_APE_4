using System.Text.Json.Serialization;

namespace ConsultaVehiculosForms.DTOs
{
    public class VehiculoDto
    {
        [JsonPropertyName("idVehiculo")]
        public int IdVehiculo { get; set; }

        [JsonPropertyName("marca")]
        public string Marca { get; set; } = string.Empty;

        [JsonPropertyName("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string Placa { get; set; } = string.Empty;

        [JsonPropertyName("chasis")]
        public string Chasis { get; set; } = string.Empty;

        [JsonPropertyName("anio")]
        public int Anio { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        public override string ToString() => $"[{IdVehiculo}] {Marca} {Modelo} ({Anio}) - ${Precio:N2} - {Estado}";
    }
}
