package ec.edu.uta.notificaciones.service;

    import com.fasterxml.jackson.databind.ObjectMapper;
    import ec.edu.uta.notificaciones.dto.VentaEventoDto;
    import jakarta.annotation.PostConstruct;
    import org.slf4j.Logger;
    import org.slf4j.LoggerFactory;
    import org.springframework.beans.factory.annotation.Value;
    import org.springframework.stereotype.Service;

    import java.io.IOException;
    import java.nio.charset.StandardCharsets;
    import java.nio.file.Files;
    import java.nio.file.Path;
    import java.nio.file.StandardOpenOption;
    import java.time.Instant;
    import java.util.ArrayList;
    import java.util.Collections;
    import java.util.LinkedHashMap;
    import java.util.List;
    import java.util.Map;

    @Service
    public class NotificacionEventoService {

        private static final Logger log =
        LoggerFactory.getLogger(NotificacionEventoService.class);

        private final ObjectMapper objectMapper;
        private final Path archivoHistorial;
        private final Map<String, VentaEventoDto> eventosPorId = new
        LinkedHashMap<>();

        public NotificacionEventoService(
                ObjectMapper objectMapper,
                @Value("${app.notificaciones.archivo-historial:data/notificaciones-ventas.jsonl}") String rutaArchivo) {
            this.objectMapper = objectMapper;
            this.archivoHistorial = Path.of(rutaArchivo);
        }

        @PostConstruct
        public synchronized void cargarHistorialPersistente() {
            if (!Files.exists(archivoHistorial)) {
                return;
            }
            try {
                for (String linea : Files.readAllLines(archivoHistorial,
                StandardCharsets.UTF_8)) {
                    if (linea == null || linea.isBlank()) continue;
                    VentaEventoDto ev = objectMapper.readValue(linea,
                    VentaEventoDto.class);
                    if (ev.eventoId() != null && !ev.eventoId().isBlank())
                    {
                        eventosPorId.putIfAbsent(ev.eventoId(), ev);
                    }
                }
                log.info("[NOTIFICACIONES] Historial recuperado desde disco: {} aviso(s).", eventosPorId.size());
            } catch (IOException ex) {
                log.warn("[NOTIFICACIONES] No se pudo leer el historial previo: {}", ex.getMessage());
            }
        }

        /**
         * Registra un evento consumido de RabbitMQ aplicando
         deduplicación estricta por eventoId.
         * Retorna true si se insertó un aviso nuevo, false si ya existía
         (reentrega ignorada).
         */
        public synchronized boolean registrarSiNoExiste(VentaEventoDto
        evento) {
            if (evento == null || evento.eventoId() == null ||
            evento.eventoId().isBlank()) {
                log.warn("[NOTIFICACIONES] Evento descartado por no contener eventoId.");
                return false;
            }

            if (eventosPorId.containsKey(evento.eventoId())) {
                log.info("[NOTIFICACIONES] Evento duplicado ignorado (deduplicacion P22): eventoId={} ventaId={}",
                        evento.eventoId(), evento.ventaId());
                return false;
            }

            VentaEventoDto enriquecido = new VentaEventoDto(
                    evento.eventoId(),
                    evento.tipo() != null ? evento.tipo() :
                    "VENTA_CONFIRMADA",
                    evento.ventaId(),
                    evento.vehiculoId(),
                    evento.compradorId(),
                    evento.precioAcordado(),
                    evento.moneda() != null ? evento.moneda() : "USD",
                    evento.estado() != null ? evento.estado() :
                    "COMPLETADA",
                    evento.fechaHora(),
                    evento.mensaje(),
                    Instant.now().toString()
            );

            eventosPorId.put(enriquecido.eventoId(), enriquecido);
            persistirLinea(enriquecido);

            log.info("[NOTIFICACIONES] Nuevo aviso registrado: eventoId={}ventaId={} vehiculoId={} precio={} {}",
                    enriquecido.eventoId(), enriquecido.ventaId(),
                    enriquecido.vehiculoId(),
                    enriquecido.precioAcordado(), enriquecido.moneda());
            return true;
        }

        public synchronized List<VentaEventoDto> listarRecientes() {
            List<VentaEventoDto> lista = new
            ArrayList<>(eventosPorId.values());
            Collections.reverse(lista); // Más recientes primero
            return lista;
        }

        private void persistirLinea(VentaEventoDto evento) {
            try {
                Path parent = archivoHistorial.getParent();
                if (parent != null) Files.createDirectories(parent);
                String jsonLinea = objectMapper.writeValueAsString(evento)
                + System.lineSeparator();
                Files.writeString(
                        archivoHistorial,
                        jsonLinea,
                        StandardCharsets.UTF_8,
                        StandardOpenOption.CREATE,
                        StandardOpenOption.APPEND
                );
            } catch (IOException ex) {
                log.error("[NOTIFICACIONES] Error persistiendo historial en disco: {}", ex.getMessage());
            }
        }
    }
