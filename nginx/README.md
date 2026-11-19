# Configuración NGINX (`nginx/`)

Puerta única de entrada REST en entorno local (`http://127.0.0.1:8090`).

## Rutas Configuradas
- `GET /nginx-ping`: diagnóstico local de NGINX (HTTP 200 sin acceder al backend).
- `/oauth/token`, `/api/vehiculos`, `/api/identidad`, `/`: balanceo ponderado **3:1** hacia `127.0.0.1:8080` (`weight=3`) y `127.0.0.1:8082` (`weight=1`).
- `/api/compradores` y `/api/ventas`: proxy hacia FastAPI (`127.0.0.1:8002`) reenviando `Authorization` e `Idempotency-Key`.
- `/api/notificaciones`: proxy hacia `notificaciones-service` (`127.0.0.1:8081`).

## Verificación y Recarga
```powershell
Copy-Item .\nginx\nginx.conf C:\nginx\conf\nginx.conf -Force
cd C:\nginx
.\nginx.exe -t
.\nginx.exe -s reload
```
