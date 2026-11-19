using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ConsultaVehiculosForms.DTOs;

namespace ConsultaVehiculosForms.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public string BaseUrl { get; set; } = "http://127.0.0.1:8080";

        public ApiClient()
        {
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(8)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Consume el endpoint GET /api/vehiculos con cabecera Authorization: Bearer {token}
        /// Implementa el ejemplo comentado en las diapositivas 17 y 18 del PDF.
        /// </summary>
        public async Task<List<VehiculoDto>> ListarVehiculosAsync(string token)
        {
            var url = $"{BaseUrl.TrimEnd('/')}/api/vehiculos";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
            }

            var response = await _http.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("HTTP 401: Token no autorizado, ausente o expirado.");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("HTTP 403: El usuario no tiene rol suficiente para ver este recurso.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonSerializer.Deserialize<List<VehiculoDto>>(json, _jsonOptions);
            return lista ?? new List<VehiculoDto>();
        }

        /// <summary>
        /// Obtiene un token JWT real desde POST /oauth/token de Spring Boot :8080
        /// </summary>
        public async Task<string> ObtenerTokenAsync(string username = "operador01", string password = "operador123")
        {
            var url = $"{BaseUrl.TrimEnd('/')}/oauth/token";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Basic Auth cliente:secreto
            var basicCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes("vehiculos-client:cliente-local-aula-2026-key-"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicCreds);

            var formValues = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password },
                { "idapli", "92" }
            };
            request.Content = new FormUrlEncodedContent(formValues);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error al autenticar (HTTP {(int)response.StatusCode}): {err}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
            {
                return tokenProp.GetString() ?? string.Empty;
            }

            throw new Exception("No se encontró el atributo 'access_token' en la respuesta.");
        }
    }
}
