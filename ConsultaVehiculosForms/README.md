# 🚗 Consulta de Vehículos con JWT (Windows Forms - C#)

Implementación puntual del ejercicio introductorio de la **Unidad 4 (Tema 4.1: Consumo desde escritorio)** según las diapositivas 17 y 18 del PDF.

---

## 🎯 Alcance del Ejercicio

La aplicación cuenta con una interfaz directa y limpia enfocada exclusivamente en:
1. **Configuración del JWT:**
   - Campo para ingresar o inspeccionar el **Token Bearer**.
   - Botón **"🔑 Token Válido"**: solicita automáticamente un JWT real a Spring Boot (`POST /oauth/token` con `operador01`/`operador123`).
   - Botón **"❌ Token Inválido (401)"**: asigna un token no válido para comprobar que la API responda `HTTP 401 Unauthorized`.
2. **Consultar Vehículos (`btnCargar_Click`):**
   - Consume de forma asíncrona (`async/await`) el endpoint `GET http://127.0.0.1:8080/api/vehiculos`.
   - Carga la lista en el `DataGridView` con formato de moneda y estados visuales.
   - Manejo de estados:
     - `Cargando...` mientras espera respuesta.
     - `{autos.Count} vehículos cargados` ante éxito.
     - `HTTP 401: Token no autorizado o expirado` si el token es inválido o no se envía.
     - `No conecta con Java` si el servicio Spring Boot está apagado.
3. **Botón Limpiar (`btnLimpiar_Click`):**
   - Limpia la tabla y restablece el mensaje de estado.

---

## 🚀 Cómo Ejecutar

```powershell
dotnet run --project ConsultaVehiculosForms
```
O abrir la solución `ConsultaVehiculosForms.slnx` en Visual Studio y presionar `F5`.
