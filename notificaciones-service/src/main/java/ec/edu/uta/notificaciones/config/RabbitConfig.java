 package ec.edu.uta.notificaciones.config;

    import org.springframework.amqp.core.Queue;
    import
    org.springframework.amqp.support.converter.Jackson2JsonMessageConverter;
    import org.springframework.amqp.support.converter.MessageConverter;
    import org.springframework.beans.factory.annotation.Value;
    import org.springframework.context.annotation.Bean;
    import org.springframework.context.annotation.Configuration;

    @Configuration
    public class RabbitConfig {

        @Bean
        public Queue notificacionesVentasQueue(
                @Value("${app.rabbitmq.queue:notificaciones.ventas}")
                String queueName) {
            // Cola durable = true (coincide con FastAPI)
            return new Queue(queueName, true);
        }

        @Bean
        public MessageConverter jacksonMessageConverter() {
            return new Jackson2JsonMessageConverter();
        }
    }