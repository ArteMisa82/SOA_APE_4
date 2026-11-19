const API_URL = '/api/vehiculos';

export async function listarVehiculos(token = '') {
  const headers = new Headers({ Accept: 'application/json' });
  const jwt = token.trim();

  if (jwt) {
    headers.set('Authorization', `Bearer ${jwt}`);
  }

  let respuesta;

  try {
    respuesta = await fetch(API_URL, {
      method: 'GET',
      headers,
      cache: 'no-store',
    });
  } catch {
    throw new Error('No se pudo conectar. Verifique el servidor y la red.');
  }

  if (!respuesta.ok) {
    const mensajes = {
      401: 'HTTP 401: Falta el JWT o el token ya no es válido.',
      403: 'HTTP 403: El usuario no tiene permiso para consultar.',
      404: 'HTTP 404: No existe la ruta /api/vehiculos.',
      500: 'HTTP 500: Error del servidor o de la conexión con la API Java.',
      502: 'HTTP 502: El servidor intermedio no pudo contactar a Java.',
      503: 'HTTP 503: El servicio de vehículos no está disponible.',
    };

    const error = new Error(
      mensajes[respuesta.status] ?? `La API devolvió HTTP ${respuesta.status}.`,
    );
    error.status = respuesta.status;
    throw error;
  }

  if (respuesta.status === 204) {
    return { vehiculos: [], codigoHttp: 204 };
  }

  let datos;

  try {
    datos = await respuesta.json();
  } catch {
    throw new Error('La API respondió, pero el JSON no pudo interpretarse.');
  }

  if (!Array.isArray(datos)) {
    throw new Error('Se esperaba una lista JSON de vehículos (un arreglo).');
  }

  return { vehiculos: datos, codigoHttp: respuesta.status };
}
