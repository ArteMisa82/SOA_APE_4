using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ConsultaVehiculosForms.DTOs;

namespace ConsultaVehiculosForms.Services
{
    public class ApiHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string RawBody { get; }

        public ApiHttpException(HttpStatusCode statusCode, string message, string rawBody = "")
            : base(message)
        {
            StatusCode = statusCode;
            RawBody = rawBody;
        }
    }

    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        // Puerta única de entrada REST: NGINX en puerto 8090
        public string BaseUrl { get; set; } = "http://127.0.0.1:8090";

        public string ClientId { get; set; } = "vehiculos-client";

        public string ClientSecret { get; set; } = string.Empty;

        public ApiClient()
        {
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            var envFile = CargarVariablesDesdeEnvLocal();

            BaseUrl = ResolverValor(
                new[] { "APE04_GATEWAY_URL", "API_BASE_URL" },
                envFile,
                "http://127.0.0.1:8090");

            ClientId = ResolverValor(
                new[] { "APE04_OAUTH_CLIENT_ID", "OAUTH_CLIENT_ID" },
                envFile,
                string.Empty);

            ClientSecret = ResolverValor(
                new[] { "APE04_OAUTH_CLIENT_SECRET", "OAUTH_CLIENT_SECRET" },
                envFile,
                string.Empty);

            // Respaldo local en desarrollo dentro del repositorio: leer application.properties de Spring Boot (ignorado por Git)
            if (string.IsNullOrWhiteSpace(ClientSecret) || string.IsNullOrWhiteSpace(ClientId))
            {
                var props = CargarPropiedadesSpringLocal();
                if (string.IsNullOrWhiteSpace(ClientId) && props.TryGetValue("app.security.client-id", out var cid))
                {
                    ClientId = ResolverPlaceholderSpring(cid);
                }
                if (string.IsNullOrWhiteSpace(ClientSecret) && props.TryGetValue("app.security.client-secret", out var sec))
                {
                    var valor = ResolverPlaceholderSpring(sec);
                    if (!valor.StartsWith("CAMBIAR_", StringComparison.OrdinalIgnoreCase))
                    {
                        ClientSecret = valor;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                ClientId = "vehiculos-client";
            }
        }

        private static string ResolverValor(string[] claves, Dictionary<string, string> envFile, string valorPorDefecto)
        {
            foreach (var clave in claves)
            {
                var env = Environment.GetEnvironmentVariable(clave);
                if (!string.IsNullOrWhiteSpace(env))
                {
                    return env.Trim();
                }
            }

            foreach (var clave in claves)
            {
                if (envFile.TryGetValue(clave, out var val) && !string.IsNullOrWhiteSpace(val))
                {
                    return val.Trim();
                }
            }

            return valorPorDefecto;
        }

        private static Dictionary<string, string> CargarVariablesDesdeEnvLocal()
        {
            var resultado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var rutasCandidatas = new[] { ".env.local", ".env" };

            foreach (var dir in ObtenerDirectoriosAscendentes())
            {
                foreach (var nombre in rutasCandidatas)
                {
                    var archivo = Path.Combine(dir, nombre);
                    if (!File.Exists(archivo)) continue;

                    try
                    {
                        foreach (var lineaRaw in File.ReadAllLines(archivo))
                        {
                            var linea = lineaRaw.Trim();
                            if (string.IsNullOrEmpty(linea) || linea.StartsWith('#')) continue;
                            var idx = linea.IndexOf('=');
                            if (idx <= 0) continue;

                            var k = linea[..idx].Trim();
                            var v = linea[(idx + 1)..].Trim().Trim('"', '\'');
                            if (!string.IsNullOrEmpty(k) && !resultado.ContainsKey(k))
                            {
                                resultado[k] = v;
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar archivos inaccesibles
                    }
                }
            }

            return resultado;
        }

        private static Dictionary<string, string> CargarPropiedadesSpringLocal()
        {
            var resultado = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var dir in ObtenerDirectoriosAscendentes())
            {
                var archivo = Path.Combine(dir, "venta-vehiculos-api_security", "src", "main", "resources", "application.properties");
                if (!File.Exists(archivo)) continue;

                try
                {
                    foreach (var lineaRaw in File.ReadAllLines(archivo))
                    {
                        var linea = lineaRaw.Trim();
                        if (string.IsNullOrEmpty(linea) || linea.StartsWith('#')) continue;
                        var idx = linea.IndexOf('=');
                        if (idx <= 0) continue;

                        var k = linea[..idx].Trim();
                        var v = linea[(idx + 1)..].Trim();
                        if (!string.IsNullOrEmpty(k))
                        {
                            resultado[k] = v;
                        }
                    }
                    break;
                }
                catch
                {
                    // Ignorar si no es accesible
                }
            }

            return resultado;
        }

        private static IEnumerable<string> ObtenerDirectoriosAscendentes()
        {
            var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var iniciales = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };

            foreach (var inicio in iniciales)
            {
                var actual = new DirectoryInfo(inicio);
                for (int i = 0; i < 6 && actual != null; i++)
                {
                    if (visitados.Add(actual.FullName))
                    {
                        yield return actual.FullName;
                    }
                    actual = actual.Parent;
                }
            }
        }

        private static string ResolverPlaceholderSpring(string valor)
        {
            var limpio = valor.Trim();
            if (limpio.StartsWith("${") && limpio.EndsWith("}"))
            {
                var interno = limpio[2..^1];
                var sep = interno.IndexOf(':');
                var varNombre = sep >= 0 ? interno[..sep].Trim() : interno.Trim();
                var defecto = sep >= 0 ? interno[(sep + 1)..].Trim() : string.Empty;
                var env = Environment.GetEnvironmentVariable(varNombre);
                return !string.IsNullOrWhiteSpace(env) ? env.Trim() : defecto;
            }
            return limpio;
        }

        /// <summary>
        /// Autentica al usuario contra POST /oauth/token vía NGINX (8090) y retorna el JWT emitido.
        /// </summary>
        public async Task<(string AccessToken, int StatusCode)> ObtenerTokenAsync(
            string username, string password, string? clientSecretOverride = null)
        {
            var secret = !string.IsNullOrWhiteSpace(clientSecretOverride)
                ? clientSecretOverride.Trim()
                : ClientSecret;

            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException(
                    "No se encontró OAUTH_CLIENT_SECRET. Defina la variable de entorno APE04_OAUTH_CLIENT_SECRET o configure un archivo local .env.");
            }

            var url = $"{BaseUrl.TrimEnd('/')}/oauth/token";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            var basicCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{secret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicCreds);

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username.Trim() },
                { "password", password },
                { "idapli", "92" }
            });

            var response = await _http.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CrearExcepcionHttp(response.StatusCode, body);
            }

            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
            {
                return (tokenProp.GetString() ?? string.Empty, (int)response.StatusCode);
            }

            throw new Exception("La respuesta de /oauth/token no contiene el campo 'access_token'.");
        }

        // 1. LISTAR VEHICULOS (GET /api/vehiculos -> NGINX -> Spring Boot 8080/8082)
        public Task<(List<VehiculoDto> Datos, int StatusCode)> ListarVehiculosAsync(string token) =>
            SendJsonAsync<List<VehiculoDto>>(HttpMethod.Get, "/api/vehiculos", token);

        // 2. CREAR VEHICULO (POST /api/vehiculos -> NGINX -> Spring Boot 8080/8082)
        public Task<(VehiculoDto Datos, int StatusCode)> CrearVehiculoAsync(string token, CrearVehiculoDto dto) =>
            SendJsonAsync<VehiculoDto>(HttpMethod.Post, "/api/vehiculos", token, dto);

        // 3. LISTAR COMPRADORES (GET /api/compradores -> NGINX -> FastAPI 8002)
        public Task<(List<CompradorDto> Datos, int StatusCode)> ListarCompradoresAsync(string token) =>
            SendJsonAsync<List<CompradorDto>>(HttpMethod.Get, "/api/compradores", token);

        // 4. CREAR COMPRADOR (POST /api/compradores -> NGINX -> FastAPI 8002)
        public Task<(CompradorDto Datos, int StatusCode)> CrearCompradorAsync(string token, CrearCompradorDto dto) =>
            SendJsonAsync<CompradorDto>(HttpMethod.Post, "/api/compradores", token, dto);

        // 5. LISTAR VENTAS (GET /api/ventas -> NGINX -> FastAPI 8002) - SOLO CONSULTA
        public Task<(List<VentaDto> Datos, int StatusCode)> ListarVentasAsync(string token) =>
            SendJsonAsync<List<VentaDto>>(HttpMethod.Get, "/api/ventas", token);

        private async Task<(T Datos, int StatusCode)> SendJsonAsync<T>(
            HttpMethod method, string ruta, string token, object? payload = null) where T : new()
        {
            var url = $"{BaseUrl.TrimEnd('/')}{ruta}";
            using var request = new HttpRequestMessage(method, url);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
            }

            if (payload != null)
            {
                var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            var response = await _http.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw CrearExcepcionHttp(response.StatusCode, body);
            }

            var data = JsonSerializer.Deserialize<T>(body, _jsonOptions) ?? new T();
            return (data, (int)response.StatusCode);
        }

        private static ApiHttpException CrearExcepcionHttp(HttpStatusCode code, string body)
        {
            var detalle = ExtraerMensajeDetalle(body);
            var mensaje = (int)code switch
            {
                400 => $"HTTP 400 (Validación): {detalle}",
                401 => $"HTTP 401 (No autenticado / JWT inválido o vencido): {detalle}",
                403 => $"HTTP 403 (Sin permiso para esta operación): {detalle}",
                404 => $"HTTP 404 (Recurso no encontrado): {detalle}",
                409 => $"HTTP 409 (Conflicto de datos/negocio): {detalle}",
                422 => $"HTTP 422 (Error de validación en campos): {detalle}",
                502 or 503 or 504 => $"HTTP {(int)code} (Servicio backend no disponible vía NGINX): {detalle}",
                _ => $"HTTP {(int)code}: {detalle}"
            };

            return new ApiHttpException(code, mensaje, body);
        }

        private static string ExtraerMensajeDetalle(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return "Sin detalle adicional del servidor.";

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.TryGetProperty("detail", out var detail)) return detail.ToString();
                if (root.TryGetProperty("mensaje", out var msg)) return msg.ToString();
                if (root.TryGetProperty("message", out var message)) return message.ToString();
                if (root.TryGetProperty("errores", out var errores)) return errores.ToString();
                if (root.TryGetProperty("title", out var title)) return title.ToString();
            }
            catch
            {
                // Respuesta no JSON
            }

            return body.Length > 240 ? body[..240] + "..." : body;
        }
    }
}
