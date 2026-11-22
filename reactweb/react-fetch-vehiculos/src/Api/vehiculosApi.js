/**
 * Cliente HTTP de solo consulta para la plataforma web React (APE 04).
 * Todas las rutas relativas (/oauth/token, /api/vehiculos, /api/ventas) pasan por el proxy
 * de Vite hacia NGINX (:8090). No implementa POST /api/ventas ni almacena secretos.
 */

function normalizarToken(token = '') {
  return String(token).trim().replace(/^Bearer\s+/i, '').trim();
}

async function extraerDetalle(respuesta) {
  try {
    const texto = await respuesta.text();
    if (!texto) return '';
    try {
      const json = JSON.parse(texto);
      return json.detail || json.mensaje || json.error || json.title || texto;
    } catch {
      return texto.slice(0, 180);
    }
  } catch {
    return '';
  }
}

async function lanzarError(respuesta, recurso) {
  const detalle = await extraerDetalle(respuesta);
  const mensajes = {
    401: 'HTTP 401: Falta el JWT, credenciales inválidas o el token expiró.',
    403: 'HTTP 403: El usuario no tiene permiso para consultar este recurso.',
    404: `HTTP 404: No existe la ruta ${recurso}.`,
    500: `HTTP 500: Error interno al consultar ${recurso}.`,
    502: `HTTP 502: NGINX no pudo contactar el servicio backend para ${recurso}.`,
    503: `HTTP 503: El servicio de ${recurso} no está disponible temporalmente.`,
    504: `HTTP 504: Tiempo de espera agotado al consultar ${recurso}.`,
  };
  const base = mensajes[respuesta.status] ?? `HTTP ${respuesta.status}: error al consultar ${recurso}.`;
  const error = new Error(detalle ? `${base} (${detalle})` : base);
  error.status = respuesta.status;
  throw error;
}

export async function iniciarSesionWeb(username, password) {
  const params = new URLSearchParams({
    grant_type: 'password',
    username: String(username || '').trim(),
    password: String(password || ''),
    idapli: '92',
  });

  let respuesta;
  try {
    respuesta = await fetch('/oauth/token', {
      method: 'POST',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: params.toString(),
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con NGINX (:8090) para iniciar sesión.');
  }

  if (!respuesta.ok) {
    await lanzarError(respuesta, '/oauth/token');
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
    accessToken: datos.access_token,
    expiresIn: datos.expires_in,
    codigoHttp: respuesta.status,
  };
}

export async function listarVehiculos(token = '') {
  const headers = new Headers({ Accept: 'application/json' });
  const jwt = normalizarToken(token);
  if (jwt) headers.set('Authorization', `Bearer ${jwt}`);

  let respuesta;
  try {
    respuesta = await fetch('/api/vehiculos', {
      method: 'GET',
      headers,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con /api/vehiculos. Verifique NGINX y Spring Boot.');
  }

  if (!respuesta.ok) {
    await lanzarError(respuesta, '/api/vehiculos');
  }

  if (respuesta.status === 204) {
    return { vehiculos: [], codigoHttp: 204 };
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La API de vehículos respondió, pero el JSON no pudo interpretarse.');
  }

  if (!Array.isArray(datos)) {
    throw new Error('Se esperaba una lista JSON de vehículos (un arreglo).');
  }

  return { vehiculos: datos, codigoHttp: respuesta.status };
}

export async function listarVentas(token = '') {
  const headers = new Headers({ Accept: 'application/json' });
  const jwt = normalizarToken(token);
  if (jwt) headers.set('Authorization', `Bearer ${jwt}`);

  let respuesta;
  try {
    respuesta = await fetch('/api/ventas', {
      method: 'GET',
      headers,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar con /api/ventas. Verifique NGINX y FastAPI.');
  }

  if (!respuesta.ok) {
    await lanzarError(respuesta, '/api/ventas');
  }

  if (respuesta.status === 204) {
    return { ventas: [], codigoHttp: 204 };
  }

  let datos;
  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La API de ventas respondió, pero el JSON no pudo interpretarse.');
  }

  if (!Array.isArray(datos)) {
    throw new Error('Se esperaba una lista JSON de ventas (un arreglo).');
  }

  return { ventas: datos, codigoHttp: respuesta.status };
}
