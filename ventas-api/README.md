# Servicio de Compradores y Ventas (`ventas-api`)

Servicio REST en **Python / FastAPI + SQLAlchemy + Alembic** ejecutado en el puerto **8002**. Administra compradores (`/api/compradores`), orquesta el registro de ventas (`/api/ventas`) con idempotencia (`Idempotency-Key`), valida JWT contra la API Java y publica eventos de venta confirmada en RabbitMQ.

## Requisitos
- Python 3.11+ y entorno virtual `.venv`.
- MySQL (`venta_vehiculos`), API Java de vehículos y RabbitMQ.

## Configuración Local
Copie `.env.example` como `.env` y complete las credenciales locales de MySQL, claves de integración y RabbitMQ.

## Instalación, Migraciones y Ejecución
```powershell
python -m venv .venv
.\.venv\Scripts\pip.exe install -r requirements.txt
.\.venv\Scripts\alembic.exe upgrade head
.\.venv\Scripts\python.exe -m uvicorn app.main:app --host 127.0.0.1 --port 8002
```
