 package ec.edu.com;

    import com.fasterxml.jackson.core.type.TypeReference;
    import com.fasterxml.jackson.databind.ObjectMapper;

    import java.net.URI;
    import java.net.http.HttpClient;
    import java.net.http.HttpRequest;
    import java.net.http.HttpResponse;
    import java.time.Duration;
    import java.util.List;
    import java.util.concurrent.CompletableFuture;

    public final class NotificacionesApiClient {

        private static final HttpClient CLIENT = HttpClient.newBuilder()
                .connectTimeout(Duration.ofSeconds(3))
                .build();

        private static final ObjectMapper MAPPER = new ObjectMapper();

        public record ResultadoNotificaciones(int codigoHttp,
        List<NotificacionVenta> notificaciones) {}

        public static CompletableFuture<ResultadoNotificaciones>
        listarNotificaciones(String baseUrl) {
            String url = baseUrl.replaceAll("/+$", "") +
            "/api/notificaciones";
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(url))
                    .timeout(Duration.ofSeconds(4))
                    .header("Accept", "application/json")
                    .GET()
                    .build();

            return CLIENT.sendAsync(request,
            HttpResponse.BodyHandlers.ofString())
                    .thenApply(resp -> {
                        int code = resp.statusCode();
                        if (code != 200) {
                            return new ResultadoNotificaciones(code,
                            List.of());
                        }
                        try {
                            List<NotificacionVenta> lista =
                            MAPPER.readValue(
                                    resp.body(),
                                    new
                                    TypeReference<List<NotificacionVenta>>
                                    () {}
                            );
                            return new ResultadoNotificaciones(code,
                            lista);
                        } catch (Exception ex) {
                            throw new RuntimeException("Respuesta JSON inválida desde notificaciones-service: " +
                            ex.getMessage(), ex);
                        }
                    });
        }
    }