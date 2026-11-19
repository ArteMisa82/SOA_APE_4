# Monitor de Balanceo y Disponibilidad (`monitor-balancer-service`)

Microservicio Spring Boot independiente (puerto **8083**) reutilizado para supervisar periódicamente NGINX (`/nginx-ping`), las instancias A (`8080`) y B (`8082`) mediante `/actuator/health`, calcular conteos reales desde `access.log` y registrar alertas de caída y recuperación.

## Configuración Local
Copie `src/main/resources/application.properties.example` como `src/main/resources/application.properties` y verifique la ruta `monitor.nginx.log-path`.

## Compilación y Ejecución
```powershell
mvn clean package -DskipTests
java -jar target\monitor-balancer-service-0.0.1-SNAPSHOT.jar
```
