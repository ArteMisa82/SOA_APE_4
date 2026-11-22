 package ec.edu.uta.notificaciones.messaging;

    import ec.edu.uta.notificaciones.dto.VentaEventoDto;
    import ec.edu.uta.notificaciones.service.NotificacionEventoService;
    import org.slf4j.Logger;
    import org.slf4j.LoggerFactory;
    import org.springframework.amqp.rabbit.annotation.RabbitListener;
    import org.springframework.stereotype.Component;

    @Component
    public class VentaEventoListener {

        private static final Logger log =
        LoggerFactory.getLogger(VentaEventoListener.class);
        private final NotificacionEventoService eventoService;

        public VentaEventoListener(NotificacionEventoService
        eventoService) {
            this.eventoService = eventoService;
        }

        @RabbitListener(queues =
        "${app.rabbitmq.queue:notificaciones.ventas}")
        public void recibirEventoVenta(VentaEventoDto evento) {
            log.info("[RabbitMQ Listener] Mensaje recibido de notificaciones.ventas -> eventoId={}, ventaId={},vehiculoId={}",
                    evento != null ? evento.eventoId() : null,
                    evento != null ? evento.ventaId() : null,
                    evento != null ? evento.vehiculoId() : null);
            eventoService.registrarSiNoExiste(evento);
        }
    }