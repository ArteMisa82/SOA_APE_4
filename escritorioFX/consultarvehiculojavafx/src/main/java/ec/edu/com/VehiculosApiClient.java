package ec.edu.com;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CompletionException;

public class VehiculosApiClient {

    private static final String URL_API = "http://127.0.0.1:8080/api/vehiculos";
    private static final HttpClient CLIENTE_HTTP = HttpClient.newBuilder()
            .connectTimeout(Duration.ofSeconds(10))
            .build();
    private static final ObjectMapper OBJECT_MAPPER = new ObjectMapper();

    public VehiculosApiClient() {
    }

    public static List<Vehiculo> listarVehiculos() throws IOException, InterruptedException {
        HttpRequest request = HttpRequest.newBuilder()
                .uri(URI.create(URL_API))
                .GET()
                .build();

        HttpResponse<String> response = CLIENTE_HTTP.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() < 200 || response.statusCode() >= 300) {
            throw new IOException("Error al consultar vehículos. Código HTTP: " + response.statusCode()
                    + ". Respuesta: " + response.body());
        }

        return OBJECT_MAPPER.readValue(response.body(), new TypeReference<List<Vehiculo>>() {
        });
    }

    public static Vehiculo consultarVehiculo(int idVehiculo) throws IOException, InterruptedException {
        HttpRequest request = HttpRequest.newBuilder()
                .uri(URI.create(URL_API + "/" + idVehiculo))
                .GET()
                .build();

        HttpResponse<String> response = CLIENTE_HTTP.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() < 200 || response.statusCode() >= 300) {
            throw new IOException("Error al consultar vehículo " + idVehiculo + ". Código HTTP: "
                    + response.statusCode() + ". Respuesta: " + response.body());
        }

        return OBJECT_MAPPER.readValue(response.body(), Vehiculo.class);
    }

    public static CompletableFuture<ResultadoConsulta> consultarVehiculos(String jwt) {
        String tokenLimpio = (jwt != null) ? jwt.trim() : "";
        String authHeader = tokenLimpio.startsWith("Bearer ") ? tokenLimpio : "Bearer " + tokenLimpio;

        HttpRequest solicitud = HttpRequest.newBuilder()
                .uri(URI.create(URL_API))
                .timeout(Duration.ofSeconds(5))
                .header("Authorization", authHeader)
                .GET()
                .build();

        return CLIENTE_HTTP.sendAsync(solicitud, HttpResponse.BodyHandlers.ofString())
                .thenApply(respuesta -> {
                    int codigo = respuesta.statusCode();
                    if (codigo != 200) {
                        return new ResultadoConsulta(codigo, List.of());
                    }

                    try {
                        List<Vehiculo> vehiculos = OBJECT_MAPPER.readValue(respuesta.body(), new TypeReference<List<Vehiculo>>() {
                        });
                        return new ResultadoConsulta(codigo, vehiculos);
                    } catch (JsonProcessingException ex) {
                        throw new CompletionException(ex);
                    }
                });
    }

    public record ResultadoConsulta(int codigoHttp, List<Vehiculo> vehiculos) {
    }
}



