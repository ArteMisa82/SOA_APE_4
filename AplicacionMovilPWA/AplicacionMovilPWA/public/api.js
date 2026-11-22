/**
 * Cliente HTTP de la PWA móvil (APE 04).
 * Todas las llamadas van a rutas relativas reenviadas por server.mjs hacia NGINX (:8090).
 * No accede a MySQL ni almacena secretos ni tokens en el dispositivo.
 */
export function normalizarToken(entrada = '') {
  return String(entrada).trim().replace(/^Bearer\s+/i, '').trim();
}

export function generarClaveIdempotente() {
  const base =
    globalThis.crypto?.randomUUID?.() ||
    `${Date.now()}-${Math.random().toString(36).slice(2, 10)}`;
  return `pwa-${base}`.replace(/[^A-Za-z0-9_-]/g, '').slice(0, 48);
}

async function extraerDetalleError(respuesta) {
  try {
    const texto = await respuesta.text();
    if (!texto) return '';
    try {
      const json = JSON.parse(texto);
      return json.detail || json.mensaje || json.error || json.title || texto;
    } catch {
      return texto.slice(0, 200);
    }
  } catch {
    return '';
  }
}

async function lanzarErrorHttp(respuesta, contexto) {
  const detalle = await extraerDetalleError(respuesta);
  const basePorCodigo = {
    400: 'HTTP 400: solicitud con datos inválidos.',
    401: 'HTTP 401: credenciales inválidas o JWT ausente, inválido o vencido.',
    403: 'HTTP 403: permiso insuficiente para ejecutar esta operación.',
    404: 'HTTP 404: recurso no encontrado.',
    409: 'HTTP 409: conflicto comercial o de idempotencia.',
    422: 'HTTP 422: error de validación en los parámetros enviados.',
    502: 'HTTP 502: servicio backend no disponible vía NGINX.',
    503: 'HTTP 503: servicio temporalmente no disponible.',
    504: 'HTTP 504: tiempo de espera agotado en el servicio.',
  };
  const prefijo = basePorCodigo[respuesta.status] || `HTTP ${respuesta.status}: ${contexto} no completada.`;
  const mensaje = detalle ? `${prefijo} (${detalle})` : prefijo;
  const error = new Error(mensaje);
  error.codigoHttp = respuesta.status;
  error.detalle = detalle;
  throw error;
}

export async function iniciarSesion(username, password, ejecutarFetch = fetch) {
  let respuesta;
  try {
    respuesta = await ejecutarFetch('/oauth/token', {
      method: 'POST',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username: String(username || '').trim(), password: String(password || '') }),
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con el servidor de autenticación vía NGINX.');
  }

  if (!respuesta.ok) {
    await lanzarErrorHttp(respuesta, 'autenticación');
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La respuesta de /oauth/token no es JSON válido.');
  }
  if (!datos?.access_token) {
    throw new Error('El servidor no devolvió access_token.');
  }
  return {
    codigoHttp: respuesta.status,
    accessToken: datos.access_token,
    expiresIn: datos.expires_in,
    scope: datos.scope || '',
  };
}

export async function listarVehiculos(entradaToken, ejecutarFetch = fetch) {
  const jwt = normalizarToken(entradaToken);
  const encabezados = new Headers({ Accept: 'application/json' });
  if (jwt) encabezados.set('Authorization', `Bearer ${jwt}`);

  let respuesta;
  try {
    respuesta = await ejecutarFetch('/api/vehiculos', {
      method: 'GET',
      headers: encabezados,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con el servicio de vehículos. Compruebe la red y NGINX.');
  }

  if (!respuesta.ok) {
    await lanzarErrorHttp(respuesta, 'consulta de vehículos');
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La API devolvió una respuesta que no es JSON válido.');
  }
  if (!Array.isArray(datos)) {
    throw new Error('La API debe devolver un arreglo JSON de vehículos.');
  }
  return { codigoHttp: respuesta.status, vehiculos: datos };
}

export async function listarCompradores(entradaToken, ejecutarFetch = fetch) {
  const jwt = normalizarToken(entradaToken);
  const encabezados = new Headers({ Accept: 'application/json' });
  if (jwt) encabezados.set('Authorization', `Bearer ${jwt}`);

  let respuesta;
  try {
    respuesta = await ejecutarFetch('/api/compradores', {
      method: 'GET',
      headers: encabezados,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con el servicio de compradores (FastAPI vía NGINX).');
  }

  if (!respuesta.ok) {
    await lanzarErrorHttp(respuesta, 'consulta de compradores');
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La API de compradores devolvió una respuesta no JSON.');
  }
  if (!Array.isArray(datos)) {
    throw new Error('La API debe devolver un arreglo JSON de compradores.');
  }
  return { codigoHttp: respuesta.status, compradores: datos };
}

export async function registrarVenta(
  entradaToken,
  { vehiculoId, compradorId, idempotencyKey },
  ejecutarFetch = fetch,
) {
  const jwt = normalizarToken(entradaToken);
  const clave = String(idempotencyKey || '').trim();
  const encabezados = new Headers({
    Accept: 'application/json',
    'Content-Type': 'application/json',
  });
  if (jwt) encabezados.set('Authorization', `Bearer ${jwt}`);
  if (clave) encabezados.set('Idempotency-Key', clave);

  let respuesta;
  try {
    respuesta = await ejecutarFetch('/api/ventas', {
      method: 'POST',
      headers: encabezados,
      body: JSON.stringify({
        vehiculoId: Number(vehiculoId),
        compradorId: Number(compradorId),
      }),
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con FastAPI vía NGINX para registrar la venta. Reintente con la misma Idempotency-Key.');
  }

  if (!respuesta.ok) {
    await lanzarErrorHttp(respuesta, 'registro de venta');
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La respuesta de POST /api/ventas no es JSON válido.');
  }
  return { codigoHttp: respuesta.status, venta: datos };
}

export async function consultarVentaPorId(entradaToken, ventaId, ejecutarFetch = fetch) {
  const jwt = normalizarToken(entradaToken);
  const id = Number(ventaId);
  if (!Number.isInteger(id) || id <= 0) {
    throw new Error('Ingrese un identificador numérico válido de venta.');
  }
  const encabezados = new Headers({ Accept: 'application/json' });
  if (jwt) encabezados.set('Authorization', `Bearer ${jwt}`);

  let respuesta;
  try {
    respuesta = await ejecutarFetch(`/api/ventas/${id}`, {
      method: 'GET',
      headers: encabezados,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con FastAPI vía NGINX para consultar la venta.');
  }

  if (!respuesta.ok) {
    await lanzarErrorHttp(respuesta, 'consulta de venta');
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La respuesta de GET /api/ventas/{id} no es JSON válido.');
  }
  return { codigoHttp: respuesta.status, venta: datos };
}
