# Configuración de RabbitMQ (Actividad 8 y 9 — APE 04)

Definición reproducible de **RabbitMQ** con interfaz de administración (`rabbitmq:3.13-management`):
- **Puerto AMQP:** `5672`
- **Panel de administración:** `http://127.0.0.1:15672`
- **Cola durable de ventas:** `notificaciones.ventas` (`durable=True`, mensajes persistentes `delivery_mode=2`)

## Arranque con Docker Desktop
1. Abra **Docker Desktop** y espere a que el motor esté activo (`Engine running`).
2. Ejecute:
```powershell
cd C:\Users\User\Documents\Uni\SOA\2_parcial\APE_4\rabbitmq
docker compose up -d
```
3. Verifique que responde el panel en `http://127.0.0.1:15672` (usuario/clave definidos por variables de entorno locales o por defecto en desarrollo).

## Detener
```powershell
cd C:\Users\User\Documents\Uni\SOA\2_parcial\APE_4\rabbitmq
docker compose down
```
