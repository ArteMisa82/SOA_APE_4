/**
 * Service Worker: permite abrir la interfaz instalada sin red.
 * Por seguridad NUNCA intercepta /api, /oauth, Authorization ni respuestas JSON.
 */
const VERSION = 'vehiculos-pwa-v4';
const RECURSOS = [
  '/', '/index.html', '/styles.css', '/main.js', '/api.js',
  '/manifest.webmanifest', '/icons/icon-192.png', '/icons/icon-512.png',
  '/assets/nexora-logo.png', '/assets/login-cars.jpg', '/assets/hero-showroom.jpg',
];

self.addEventListener('install', (evento) => {
  evento.waitUntil(
    caches.open(VERSION)
      .then((cache) => cache.addAll(RECURSOS))
      .then(() => self.skipWaiting()),
  );
});

self.addEventListener('activate', (evento) => {
  evento.waitUntil(
    caches.keys().then((nombres) => Promise.all(
      nombres.filter((nombre) => nombre !== VERSION).map((nombre) => caches.delete(nombre)),
    )).then(() => self.clients.claim()),
  );
});

self.addEventListener('fetch', (evento) => {
  const solicitud = evento.request;
  const url = new URL(solicitud.url);
  if (solicitud.method !== 'GET' || url.origin !== self.location.origin) return;

  // Los datos protegidos y la autenticación se obtienen siempre en línea; nunca se guardan en caché.
  if (url.pathname.startsWith('/api/') || url.pathname.startsWith('/oauth/')) return;

  if (solicitud.mode === 'navigate') {
    evento.respondWith(fetch(solicitud).catch(async () => (
      (await caches.match('/index.html')) || Response.error()
    )));
    return;
  }

  if (RECURSOS.includes(url.pathname)) {
    evento.respondWith(caches.match(solicitud).then((guardado) => guardado || fetch(solicitud)));
  }
});
