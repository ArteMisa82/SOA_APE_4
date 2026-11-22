/**
 * Servidor de desarrollo local sin dependencias externas para la PWA móvil (APE 04).
 * Sirve la PWA y actúa como proxy mismo-origen hacia NGINX (:8090) para:
 *   - POST /oauth/token (inyecta Basic Auth desde variables de entorno, sin exponer secretos al navegador)
 *   - GET  /api/vehiculos
 *   - GET  /api/compradores
 *   - POST /api/ventas (reenvía Authorization e Idempotency-Key)
 *   - GET  /api/ventas/{id}
 */
import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { resolve, extname, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const ROOT = resolve(fileURLToPath(new URL('.', import.meta.url)));
const TIPOS = {
  '.html': 'text/html; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8',
  '.png': 'image/png',
  '.webmanifest': 'application/manifest+json; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.svg': 'image/svg+xml',
};

async function leerCuerpo(peticion) {
  const fragmentos = [];
  for await (const trozo of peticion) {
    fragmentos.push(trozo);
  }
  return Buffer.concat(fragmentos);
}

export function crearServidor({
  directorio = resolve(ROOT, 'public'),
  apiBase = process.env.API_BASE_URL || 'http://127.0.0.1:8090',
  oauthClientId = process.env.OAUTH_CLIENT_ID || 'vehiculos-client',
  oauthClientSecret = process.env.OAUTH_CLIENT_SECRET || '',
} = {}) {
  const carpeta = resolve(directorio);
  const baseLimpia = apiBase.replace(/\/$/, '');

  return createServer(async (peticion, respuesta) => {
    const url = new URL(peticion.url || '/', 'http://localhost');
    respuesta.setHeader('X-Content-Type-Options', 'nosniff');
    respuesta.setHeader('Referrer-Policy', 'no-referrer');
    respuesta.setHeader('Cache-Control', 'no-store');

    // 1. Proxy seguro de autenticación: POST /oauth/token -> NGINX (:8090)
    if (url.pathname === '/oauth/token') {
      if (peticion.method !== 'POST') {
        respuesta.writeHead(405, { 'Content-Type': 'application/json; charset=utf-8' });
        respuesta.end(JSON.stringify({ error: 'Método no permitido' }));
        return;
      }

      const secret = oauthClientSecret || process.env.OAUTH_CLIENT_SECRET || '';
      if (!secret) {
        respuesta.writeHead(500, { 'Content-Type': 'application/json; charset=utf-8' });
        respuesta.end(JSON.stringify({
          error: 'Configure la variable de entorno OAUTH_CLIENT_SECRET antes de iniciar la PWA.',
        }));
        return;
      }

      const abortador = new AbortController();
      const temporizador = setTimeout(() => abortador.abort(), 12000);
      try {
        const bodyRaw = (await leerCuerpo(peticion)).toString('utf8');
        let params;
        if ((peticion.headers['content-type'] || '').includes('application/json')) {
          const parsed = JSON.parse(bodyRaw || '{}');
          params = new URLSearchParams({
            grant_type: 'password',
            username: String(parsed.username || '').trim(),
            password: String(parsed.password || ''),
            idapli: '92',
          });
        } else {
          params = new URLSearchParams(bodyRaw);
          if (!params.get('grant_type')) params.set('grant_type', 'password');
          if (!params.get('idapli')) params.set('idapli', '92');
        }

        const basic = Buffer.from(`${oauthClientId}:${secret}`).toString('base64');
        const remoto = await fetch(`${baseLimpia}/oauth/token`, {
          method: 'POST',
          headers: {
            Accept: 'application/json',
            Authorization: `Basic ${basic}`,
            'Content-Type': 'application/x-www-form-urlencoded',
          },
          body: params.toString(),
          signal: abortador.signal,
          cache: 'no-store',
        });

        respuesta.writeHead(remoto.status, {
          'Content-Type': remoto.headers.get('content-type') || 'application/json; charset=utf-8',
          'Cache-Control': 'no-store',
        });
        respuesta.end(Buffer.from(await remoto.arrayBuffer()));
      } catch (error) {
        const tiempoAgotado = error.name === 'AbortError';
        respuesta.writeHead(tiempoAgotado ? 504 : 502, { 'Content-Type': 'application/json; charset=utf-8' });
        respuesta.end(JSON.stringify({
          error: tiempoAgotado ? 'Tiempo de espera agotado al autenticar' : 'No se pudo conectar con NGINX (:8090) para autenticar',
        }));
      } finally {
        clearTimeout(temporizador);
      }
      return;
    }

    // 2. Proxy REST hacia NGINX (:8090) para rutas permitidas de la PWA
    if (url.pathname.startsWith('/api/')) {
      const rutaPermitida =
        (url.pathname === '/api/vehiculos' && peticion.method === 'GET') ||
        (url.pathname === '/api/compradores' && peticion.method === 'GET') ||
        (url.pathname === '/api/ventas' && peticion.method === 'POST') ||
        (/^\/api\/ventas\/\d+$/.test(url.pathname) && peticion.method === 'GET');

      if (!rutaPermitida) {
        respuesta.writeHead(404, { 'Content-Type': 'application/json; charset=utf-8' });
        respuesta.end(JSON.stringify({ error: 'Endpoint no disponible en la PWA' }));
        return;
      }

      const abortador = new AbortController();
      const temporizador = setTimeout(() => abortador.abort(), 15000);
      try {
        const encabezados = { Accept: 'application/json' };
        if (peticion.headers.authorization) {
          encabezados.Authorization = peticion.headers.authorization;
        }
        if (peticion.headers['idempotency-key']) {
          encabezados['Idempotency-Key'] = peticion.headers['idempotency-key'];
        }
        if (peticion.headers['content-type']) {
          encabezados['Content-Type'] = peticion.headers['content-type'];
        }

        const opcionesFetch = {
          method: peticion.method,
          headers: encabezados,
          signal: abortador.signal,
          cache: 'no-store',
        };

        if (peticion.method === 'POST') {
          opcionesFetch.body = await leerCuerpo(peticion);
        }

        const destino = `${baseLimpia}${url.pathname}${url.search || ''}`;
        const remoto = await fetch(destino, opcionesFetch);

        const respHeaders = {
          'Content-Type': remoto.headers.get('content-type') || 'application/json; charset=utf-8',
          'Cache-Control': 'no-store',
          Vary: 'Authorization',
        };
        const location = remoto.headers.get('location');
        if (location) respHeaders.Location = location;

        respuesta.writeHead(remoto.status, respHeaders);
        respuesta.end(Buffer.from(await remoto.arrayBuffer()));
      } catch (error) {
        const tiempoAgotado = error.name === 'AbortError';
        respuesta.writeHead(tiempoAgotado ? 504 : 502, { 'Content-Type': 'application/json; charset=utf-8' });
        respuesta.end(JSON.stringify({
          error: tiempoAgotado ? 'Tiempo de espera agotado' : 'Servicio backend no disponible vía NGINX (:8090)',
        }));
      } finally {
        clearTimeout(temporizador);
      }
      return;
    }

    if (peticion.method !== 'GET' && peticion.method !== 'HEAD') {
      respuesta.writeHead(405);
      respuesta.end();
      return;
    }

    let ruta;
    try {
      ruta = decodeURIComponent(url.pathname);
    } catch {
      respuesta.writeHead(400);
      respuesta.end();
      return;
    }
    const archivo = resolve(carpeta, '.' + (ruta === '/' ? '/index.html' : ruta));
    if (!(archivo === carpeta || archivo.startsWith(carpeta + sep))) {
      respuesta.writeHead(403);
      respuesta.end();
      return;
    }
    try {
      const detalle = await stat(archivo);
      if (!detalle.isFile()) throw new Error('Archivo no encontrado');
      respuesta.setHeader('Content-Type', TIPOS[extname(archivo)] || 'application/octet-stream');
      if (peticion.method === 'HEAD') {
        respuesta.writeHead(200);
        respuesta.end();
        return;
      }
      respuesta.writeHead(200);
      respuesta.end(await readFile(archivo));
    } catch {
      respuesta.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
      respuesta.end('Recurso no encontrado');
    }
  });
}

// Se habilita uso como módulo para pruebas, sin iniciar puerto al importarlo.
if (process.argv[1] && resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const argPuerto = process.argv.find((arg) => arg.startsWith('--port='));
  const argCarpeta = process.argv.find((arg) => arg.startsWith('--dir='));
  const puerto = Number(argPuerto?.split('=')[1] || process.env.PORT || 5173);
  const host = process.env.HOST || '127.0.0.1';
  const carpeta = resolve(ROOT, argCarpeta?.split('=')[1] || 'public');
  const gateway = process.env.API_BASE_URL || 'http://127.0.0.1:8090';
  crearServidor({ directorio: carpeta }).listen(puerto, host, () => {
    console.log(`PWA disponible: http://localhost:${puerto}`);
    console.log(`Gateway NGINX: ${gateway} (/oauth/token, /api/vehiculos, /api/compradores, /api/ventas)`);
    console.log('Para cerrar: Ctrl + C');
  });
}
