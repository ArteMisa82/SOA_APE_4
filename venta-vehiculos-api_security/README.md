# API de Vehículos y Autenticación (`venta-vehiculos-api_security`)

Servicio REST en **Java / Spring Boot** encargado de la emisión/validación JWT (`/oauth/token`, `/api/identidad`), gestión del catálogo de vehículos (`/api/vehiculos`) y control transaccional de reservas (`/api/reservas`), ejecutado en **dos instancias equivalentes (8080 y 8082)** detrás de NGINX.

## Requisitos
- JDK 17+ y Maven 3.9+ (o wrapper `./mvnw.cmd`).
- MySQL en `127.0.0.1:3306` con esquema `venta_vehiculos`.

## Configuración Local
Copie `src/main/resources/application.properties.example` como `src/main/resources/application.properties` y configure los valores locales de base de datos y secretos JWT/integración.

## Compilación y Ejecución
```powershell
# Compilar y empaquetar
.\mvnw.cmd clean package -DskipTests

# Ejecutar Instancia A (puerto 8080)
java -jar target\venta-vehiculos-api-0.0.1-SNAPSHOT.jar --server.port=8080

# Ejecutar Instancia B (puerto 8082)
java -jar target\venta-vehiculos-api-0.0.1-SNAPSHOT.jar --server.port=8082
```
