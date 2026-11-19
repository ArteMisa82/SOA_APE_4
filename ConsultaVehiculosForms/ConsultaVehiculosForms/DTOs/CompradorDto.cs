using System.Text.Json.Serialization;

namespace ConsultaVehiculosForms.DTOs
{
    public class CompradorDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("documento")]
        public string? Documento { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Nombre} ({Documento}) - {Correo}";
    }

    public class CrearCompradorDto
    {
        [JsonPropertyName("documento")]
        public string Documento { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;
    }
}
