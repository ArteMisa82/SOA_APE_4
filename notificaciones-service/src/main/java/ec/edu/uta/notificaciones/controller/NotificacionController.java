package ec.edu.uta.notificaciones.controller;

import ec.edu.uta.notificaciones.dto.NotificacionRequest;
import ec.edu.uta.notificaciones.dto.NotificacionResponse;
import ec.edu.uta.notificaciones.dto.VentaEventoDto;
import ec.edu.uta.notificaciones.service.NotificacionEventoService;
import ec.edu.uta.notificaciones.service.NotificacionService;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/notificaciones")
public class NotificacionController {

    private final NotificacionService notificacionService;
    private final NotificacionEventoService eventoService;

    public NotificacionController(
            NotificacionService notificacionService,
            NotificacionEventoService eventoService) {
        this.notificacionService = notificacionService;
        this.eventoService = eventoService;
    }

    @PostMapping
    public ResponseEntity<NotificacionResponse> crear(
            @RequestBody NotificacionRequest request) {

        NotificacionResponse response = notificacionService.procesar(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }

    @GetMapping
    public ResponseEntity<List<VentaEventoDto>> listar() {
        return ResponseEntity.ok(eventoService.listarRecientes());
    }

    @GetMapping("/health")
    public ResponseEntity<Map<String, Object>> health() {
        return ResponseEntity.ok(Map.of(
                "status", "UP",
                "totalNotificaciones", eventoService.listarRecientes().size()
        ));
    }
}
