# Cliente de Escritorio JavaFX (`escritorioFX`)

Aplicación de escritorio en **JavaFX + Maven** orientada a la visualización automática de notificaciones de venta consumidas desde `notificaciones-service` (`GET /api/notificaciones`), manteniendo un indicador de estado de conexión y sondeo periódico sin acceso directo a RabbitMQ ni MySQL.

## Requisitos
- JDK 17+ (verificado con OpenJDK 25) y Apache Maven 3.9+.
- Servicio `notificaciones-service` activo (vía NGINX `8090` o puerto `8081`).

## Compilación y Ejecución
```powershell
cd consultarvehiculojavafx
mvn clean compile
mvn javafx:run
```
