namespace ConsultaVehiculosForms.Services
{
    public class AppState
    {
        private static AppState? _instance;
        public static AppState Current => _instance ??= new AppState();

        public string JavaBaseUrl { get; set; } = "http://127.0.0.1:8080";
        public string FastApiBaseUrl { get; set; } = "http://127.0.0.1:8002";

        public string? Token { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string? Scope { get; set; }

        public string LastIdempotencyKey { get; set; } = string.Empty;

        public int? SelectedVehiculoId { get; set; }
        public int? SelectedCompradorId { get; set; }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

        public event Action? AuthStateChanged;

        public void SetSession(string token, string usuario, List<string>? roles = null, string? scope = null)
        {
            Token = token.Trim();
            Usuario = usuario;
            Roles = roles ?? new List<string>();
            Scope = scope;
            AuthStateChanged?.Invoke();
        }

        public void ClearSession()
        {
            Token = null;
            Usuario = string.Empty;
            Roles.Clear();
            Scope = null;
            AuthStateChanged?.Invoke();
        }

        public string GenerarNuevaClaveIdempotencia()
        {
            LastIdempotencyKey = "venta-" + Guid.NewGuid().ToString("N")[..12];
            return LastIdempotencyKey;
        }
    }
}
