# APE 04 - Arquitectura Multicanal de Vehículos, Compradores, Ventas y Notificaciones

Repositorio integral de la **Guía Práctica 4 (SOA)** que articula cuatro aplicaciones cliente (Windows Forms, PWA móvil, React y JavaFX), tres servicios REST/mensajería (Spring Boot Vehículos, FastAPI Ventas y Spring Boot Notificaciones), balanceo de carga ponderado 3:1 en NGINX, monitor de disponibilidad, persistencia en MySQL y mensajería asíncrona durable en RabbitMQ.

## Estructura del Repositorio

| Carpeta | Tecnología | Puerto | Responsabilidad principal |
| :--- | :--- | :---: | :--- |
| `venta-vehiculos-api_security/` | Java / Spring Boot + JWT + JPA | `8080` / `8082` | Autenticación OAuth2/JWT, catálogo de vehículos y reservas (dos instancias detrás de NGINX). |
| `ventas-api/` | Python / FastAPI + SQLAlchemy + RabbitMQ | `8002` | Gestión de compradores, ventas con `Idempotency-Key` y publicación de eventos a RabbitMQ. |
| `notificaciones-service/` | Java / Spring Boot + AMQP | `8081` | Consumidor durable de RabbitMQ, deduplicación por `eventoId` y consulta `GET /api/notificaciones`. |
| `monitor-balancer-service/` | Java / Spring Boot | `8083` | Supervisión independiente de NGINX (`8090`), instancias `8080`/`8082`, métricas de log y alertas. |
| `ConsultaVehiculosForms/` | C# / .NET Windows Forms | Escritorio | Administración: crear vehículos/compradores y listar vehículos, compradores y ventas (sin crear ventas). |
| `AplicacionMovilPWA/` | HTML5 / CSS3 / JS + Node.js PWA | `5173` | Login JWT en sesión y **único cliente** autorizado para registrar ventas (`POST /api/ventas`). |
| `reactweb/` | React + Vite + Fetch API | `5174` | Plataforma web de consulta: catálogo de vehículos, filtro por marca, detalle y consulta/resumen de ventas. |
| `escritorioFX/` | JavaFX + Maven | Escritorio | Visor de notificaciones de ventas en tiempo casi real desde `notificaciones-service`. |
| `nginx/` | NGINX | `8090` | Puerta única de entrada REST y balanceo ponderado 3:1 hacia `8080` y `8082`. |

## Requisitos Generales

- **Java JDK 17+** (verificado con OpenJDK 25) y **Apache Maven 3.9+**
- **Python 3.11+** con entorno virtual (`venv`)
- **.NET SDK 8/10** (Windows Forms)
- **Node.js 20+** y **npm**
- **MySQL / MariaDB** (XAMPP puerto `3306`, esquema `venta_vehiculos`)
- **NGINX** (puerto `8090`) y **RabbitMQ** (`5672` / `15672`)

## Configuración de Secretos (Plantillas)

Este repositorio **no almacena contraseñas, claves JWT ni archivos `.env` / `application.properties` reales**. Antes de ejecutar localmente, copie cada plantilla y complete sus credenciales locales:

1. `venta-vehiculos-api_security/src/main/resources/application.properties.example` → `application.properties`
2. `notificaciones-service/src/main/resources/application.properties.example` → `application.properties`
3. `monitor-balancer-service/src/main/resources/application.properties.example` → `application.properties`
4. `ventas-api/.env.example` → `.env`

## Orden de Arranque Recomendado

1. **MySQL** (`3306`) y **RabbitMQ** (`5672` / `15672`).
2. **API Vehículos Instancia A** (`8080`) e **Instancia B** (`8082`).
3. **NGINX** (`8090`, verificando previamente con `nginx -t`).
4. **Monitor de Balanceo** (`8083`, opcional/supervisión).
5. **Ventas FastAPI** (`8002`) y **Notificaciones Service** (`8081`).
6. **Clientes**: Windows Forms, React (`5174`), PWA móvil (`5173`) y JavaFX.
