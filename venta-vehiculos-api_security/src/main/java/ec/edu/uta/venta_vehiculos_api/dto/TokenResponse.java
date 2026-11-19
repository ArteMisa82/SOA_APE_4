package ec.edu.uta.venta_vehiculos_api.dto;

// DTO que representa la respuesta JWT o token de autenticación.
public class TokenResponse {

    // Token de acceso principal.
    private String access_token;

    // Tipo de token, por ejemplo: Bearer.
    private String token_type;

    // Token para renovar la sesión.
    private String refresh_token;

    // Tiempo de expiración del token en segundos.
    private long expires_in;

    // Alcance o permisos del token.
    private String scope;

    // Constructor vacío para serialización JSON.
    public TokenResponse() {
    }

    // Constructor completo para inicializar todos los campos del token.
     public TokenResponse(String access_token,
            String token_type,
            String refresh_token,
            long expires_in,
            String scope) {
                this.access_token = access_token;
                this.token_type = token_type;
                this.refresh_token = refresh_token;
                this.expires_in = expires_in;
                this.scope = scope;
    }

    // Asigna el token de acceso.
    public void setAccess_token(String access_token) {
        this.access_token = access_token;
    }

    // Asigna el tipo del token.
    public void setToken_type(String token_type) {
        this.token_type = token_type;
    }

    // Asigna el refresh token.
    public void setRefresh_token(String refresh_token) {
        this.refresh_token = refresh_token;
    }

    // Asigna el tiempo de expiración.
    public void setExpires_in(long expires_in) {
        this.expires_in = expires_in;
    }

    // Asigna el alcance del token.
    public void setScope(String scope) {
        this.scope = scope;
    }

    // Retorna el token de acceso.
     public String getAccess_token() {
         return access_token;
     }

    // Retorna el tipo del token.
     public String getToken_type() {
         return token_type;
     }

    // Retorna el refresh token.
     public String getRefresh_token() {
         return refresh_token;
     }

    // Retorna la expiración en segundos.
     public long getExpires_in() {
         return expires_in;
     }

    // Retorna el alcance del token.
     public String getScope() {
         return scope;
     }
}
