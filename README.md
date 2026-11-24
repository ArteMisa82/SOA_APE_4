# APE 04 - Arquitectura Multicanal de Vehículos, Compradores, Ventas y Notificaciones (NEXORA Autos)

Repositorio integral de la **Guía Práctica 4 (Aplicación Orientada a Servicios - SOA)** que articula cuatro aplicaciones cliente con identidad visual unificada **NEXORA Autos** (Windows Forms, PWA móvil, React y JavaFX), tres servicios REST/mensajería (Spring Boot Vehículos, FastAPI Ventas y Spring Boot Notificaciones), balanceo de carga ponderado 3:1 en NGINX, monitor de disponibilidad, persistencia en MySQL y mensajería asíncrona durable en RabbitMQ.

- **Estudiante:** Jaque Bonilla Verónica Anabel
- **Repositorio Git:** `https://github.com/ArteMisa82/SOA_APE_4`

## Estructura del Repositorio

| Carpeta | Tecnología | Puerto | Responsabilidad principal |
| :--- | :--- | :---: | :--- |
| `venta-vehiculos-api_security/` | Java / Spring Boot + JWT + JPA + Actuator | `8080` / `8082` | Autenticación OAuth2/JWT stateless, catálogo de vehículos y reservas (dos instancias detrás de NGINX). |
| `ventas-api/` | Python / FastAPI + SQLAlchemy + Alembic + pika | `8002` | Gestión de compradores, ventas con `Idempotency-Key` (SHA-256), migraciones SQL y publicación durable a RabbitMQ. |
| `notificaciones-service/` | Java / Spring Boot + AMQP (`@RabbitListener`) | `8081` | Consumidor durable de RabbitMQ (`notificaciones.ventas`), deduplicación por `eventoId` y consulta `GET /api/notificaciones`. |
| `monitor-balancer-service/` | Java / Spring Boot | `8083` | Supervisión independiente de NGINX (`8090`), instancias `8080`/`8082`, métricas de `access.log` y alertas. |
| `ConsultaVehiculosForms/` | C# / .NET Windows Forms | Escritorio | Administración NEXORA: login JWT, crear vehículos/compradores y listar vehículos, compradores y ventas (sin crear ventas). |
| `AplicacionMovilPWA/` | HTML5 / CSS3 / JS + Node.js PWA | `5173` | Login JWT en memoria y **único cliente** autorizado para registrar ventas (`POST /api/ventas`) con `Idempotency-Key` y consulta `GET /api/ventas/{id}`. |
| `reactweb/` | React 19 + Vite 8 + Fetch API | `5174` | Plataforma web NEXORA de solo consulta: KPIs reales, catálogo (tabla/tarjetas), filtro por marca, ficha técnica y seguimiento de ventas. |
| `escritorioFX/` | JavaFX 21 + Maven | Escritorio | Monitor NEXORA de notificaciones de ventas en tiempo real (sondeo automático cada 3 s a `GET /api/notificaciones` vía NGINX). |
| `nginx/` | NGINX | `8090` | Puerta única de entrada REST, balanceo ponderado 3:1 hacia `8080`/`8082` y proxy hacia FastAPI (`8002`) y Notificaciones (`8081`). |
| `rabbitmq/` | Docker Compose (`rabbitmq:3.13-management`) | `5672` / `15672` | Broker AMQP con cola durable `notificaciones.ventas` y mensajes persistentes (`delivery_mode=2`). |

## Requisitos y Dependencias

- **Java JDK 17+** (verificado con OpenJDK 25) y **Apache Maven 3.9+**
- **Python 3.11+** con entorno virtual (`.venv`) y `pip`
- **.NET SDK 8 / 10** (Windows Forms)
- **Node.js 20+** y **npm**
- **MySQL / MariaDB** (XAMPP puerto `3306`, esquema `venta_vehiculos`)
- **NGINX** (puerto `8090`) y **Docker Desktop** (para RabbitMQ `5672` / `15672`)

## Configuración de Secretos (Plantillas sin valores reales)

Este repositorio **no almacena contraseñas, claves JWT ni archivos `.env` / `application.properties` reales**. Antes de ejecutar localmente, copie cada plantilla y complete sus credenciales locales:

1. `venta-vehiculos-api_security/src/main/resources/application.properties.example` → `application.properties`
2. `notificaciones-service/src/main/resources/application.properties.example` → `application.properties`
3. `monitor-balancer-service/src/main/resources/application.properties.example` → `application.properties`
4. `ventas-api/.env.example` → `.env`

## Orden Oficial de Arranque y Comandos

### 1. Base de datos MySQL (`3306`) y RabbitMQ (`5672` / `15672`)
- Inicie **MySQL** en XAMPP (puerto `3306`, base de datos `venta_vehiculos`).
- Inicie **RabbitMQ** durable:
  ```powershell
  cd rabbitmq
  docker compose up -d
  ```

### 2. API Vehículos — Instancia A (`8080`) e Instancia B (`8082`)
En dos terminales dentro de `venta-vehiculos-api_security`:
```powershell
# Terminal 1 (Instancia A - 8080)
.\mvnw.cmd spring-boot:run

# Terminal 2 (Instancia B - 8082)
.\mvnw.cmd spring-boot:run "-Dspring-boot.run.arguments=--server.port=8082"
```

### 3. NGINX (`8090`) y Monitor de Balanceo (`8083`)
Copie `nginx/nginx.conf` a su instalación local de NGINX (`C:\nginx\conf\nginx.conf`), valide sintaxis e inicie:
```powershell
nginx -t
start nginx
```
Opcional (supervisión de disponibilidad y balanceo 3:1):
```powershell
cd monitor-balancer-service
.\mvnw.cmd spring-boot:run
```

### 4. API de Compradores y Ventas FastAPI (`8002`) y Migraciones Alembic
```powershell
cd ventas-api
python -m venv .venv
.\.venv\Scripts\pip.exe install -r requirements.txt
.\.venv\Scripts\alembic.exe upgrade head
.\.venv\Scripts\python.exe -m uvicorn app.main:app --host 127.0.0.1 --port 8002 --reload
```

### 5. Servicio de Notificaciones Spring Boot (`8081`)
```powershell
cd notificaciones-service
mvn spring-boot:run
```

### 6. Aplicaciones Cliente (NEXORA Autos)

- **Windows Forms (Administración):**
  ```powershell
  cd ConsultaVehiculosForms\ConsultaVehiculosForms
  $env:APE04_OAUTH_CLIENT_SECRET = "<TU_OAUTH_CLIENT_SECRET_LOCAL>"
  dotnet run
  ```
- **PWA Móvil (`5173` — único cliente con registro de ventas):**
  ```powershell
  cd AplicacionMovilPWA\AplicacionMovilPWA
  $env:API_BASE_URL = "http://127.0.0.1:8090"
  $env:OAUTH_CLIENT_ID = "vehiculos-client"
  $env:OAUTH_CLIENT_SECRET = "<TU_OAUTH_CLIENT_SECRET_LOCAL>"
  npm start
  ```
- **Cliente Web React (`5174` — solo consulta):**
  ```powershell
  cd reactweb\react-fetch-vehiculos
  npm install
  $env:VITE_GATEWAY_URL = "http://127.0.0.1:8090"
  $env:OAUTH_CLIENT_ID = "vehiculos-client"
  $env:OAUTH_CLIENT_SECRET = "<TU_OAUTH_CLIENT_SECRET_LOCAL>"
  npm run dev
  ```
- **Monitor de Notificaciones JavaFX:**
  ```powershell
  cd escritorioFX\consultarvehiculojavafx
  mvn javafx:run
  ```
