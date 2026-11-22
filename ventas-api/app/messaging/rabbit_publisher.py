import json
import logging
from datetime import datetime, timezone

import pika

from app.config import (
    RABBITMQ_HOST,
    RABBITMQ_PORT,
    RABBITMQ_USER,
    RABBITMQ_PASS,
    RABBITMQ_QUEUE,
)

logger = logging.getLogger("ventas.rabbitmq")


def construir_evento_venta(venta) -> dict:
    """
    Construye el payload JSON de evento de venta confirmada.
    Omite contraseñas, JWT y datos personales innecesarios (correo/cédula).
    El eventoId es determinístico por venta (venta.operacion_id) para garantizar
    deduplicación en el consumidor ante reentregas.
    """
    fecha_iso = (
        venta.fecha.replace(tzinfo=timezone.utc).isoformat().replace("+00:00", "Z")
        if venta.fecha is not None
        else datetime.now(timezone.utc).isoformat().replace("+00:00", "Z")
    )
    return {
        "eventoId": f"evt-venta-{venta.operacion_id}",
        "tipo": "VENTA_CONFIRMADA",
        "ventaId": venta.id,
        "vehiculoId": venta.vehiculo_id,
        "compradorId": venta.comprador_id,
        "precioAcordado": str(venta.precio_acordado) if venta.precio_acordado is not None else None,
        "moneda": venta.moneda or "USD",
        "estado": venta.estado,
        "fechaHora": fecha_iso,
        "mensaje": f"Venta #{venta.id} confirmada para vehículo #{venta.vehiculo_id}",
    }


class RabbitEventPublisher:
    """
    Publicador independiente de eventos de venta hacia una cola durable de RabbitMQ
    con mensajes persistentes (delivery_mode=2).
    """

    def __init__(
        self,
        host: str = RABBITMQ_HOST,
        port: int = RABBITMQ_PORT,
        username: str = RABBITMQ_USER,
        password: str = RABBITMQ_PASS,
        queue: str = RABBITMQ_QUEUE,
    ):
        self.host = host
        self.port = port
        self.username = username
        self.password = password
        self.queue = queue

    def publicar_venta_confirmada(self, evento: dict) -> bool:
        """
        Publica un evento persistente en la cola durable configurada.
        Retorna True si RabbitMQ aceptó el mensaje (confirm_delivery),
        o False si RabbitMQ no está disponible (para reintento posterior por worker.py).
        """
        credentials = pika.PlainCredentials(self.username, self.password)
        params = pika.ConnectionParameters(
            host=self.host,
            port=self.port,
            credentials=credentials,
            connection_attempts=1,
            socket_timeout=3,
            blocked_connection_timeout=3,
        )
        connection = None
        try:
            connection = pika.BlockingConnection(params)
            channel = connection.channel()
            # Declaración de cola durable (persiste ante reinicios del broker)
            channel.queue_declare(queue=self.queue, durable=True)
            channel.confirm_delivery()

            body = json.dumps(evento, ensure_ascii=False).encode("utf-8")
            channel.basic_publish(
                exchange="",
                routing_key=self.queue,
                body=body,
                properties=pika.BasicProperties(
                    content_type="application/json",
                    delivery_mode=2,  # Mensaje persistente en disco
                    message_id=evento["eventoId"],
                    type=evento["tipo"],
                ),
                mandatory=True,
            )
            logger.info(
                "[RabbitMQ] Evento publicado en cola '%s': eventoId=%s ventaId=%s vehiculoId=%s",
                self.queue,
                evento["eventoId"],
                evento["ventaId"],
                evento["vehiculoId"],
            )
            return True
        except Exception as ex:
            logger.warning(
                "[RabbitMQ] No se pudo publicar eventoId=%s (ventaId=%s) en '%s': %s. "
                "La venta queda con notificacion='PENDIENTE' para reintento por app.worker.",
                evento.get("eventoId"),
                evento.get("ventaId"),
                self.queue,
                ex,
            )
            return False
        finally:
            if connection is not None and connection.is_open:
                try:
                    connection.close()
                except Exception:
                    pass
