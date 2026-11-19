# Aplicación Móvil PWA (`AplicacionMovilPWA`)

Progressive Web App (HTML5 / CSS3 / ES Modules + Node.js) ejecutada en el puerto **5173**. Actúa como el **único cliente autorizado para registrar ventas** (`POST /api/ventas` con `Idempotency-Key`), obteniendo su JWT exclusivamente mediante formulario de inicio de sesión (`POST /oauth/token`) a través de NGINX (`8090`).

## Requisitos
- Node.js 20+ y npm.
- NGINX en ejecución en `http://127.0.0.1:8090`.

## Compilación y Ejecución
```powershell
cd AplicacionMovilPWA
npm run build
npm start
```
Acceso local: `http://127.0.0.1:5173`
