import heroShowroomImg from '../assets/hero-showroom.jpg';

const dinero = new Intl.NumberFormat('es-EC', {
  style: 'currency',
  currency: 'USD',
});

export function mostrarPrecio(precio) {
  if (precio === null || precio === undefined || precio === '') return '—';
  const numero = Number(precio);
  return Number.isFinite(numero) ? dinero.format(numero) : String(precio);
}

export function formatoFechaCorta(fechaIso) {
  if (!fechaIso) return '—';
  const d = new Date(fechaIso);
  if (Number.isNaN(d.getTime())) return fechaIso;
  return d.toLocaleString('es-EC', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function claseEstadoVehiculo(estado = '') {
  const e = String(estado).toUpperCase();
  if (e === 'DISPONIBLE') return 'status-badge status-available';
  if (e === 'RESERVADO') return 'status-badge status-reserved';
  return 'status-badge status-sold';
}

export function claseEstadoVenta(estado = '') {
  const e = String(estado).toUpperCase();
  if (e === 'COMPLETADA') return 'status-badge status-available';
  if (e === 'PENDIENTE' || e === 'EN_REVISION') return 'status-badge status-reserved';
  if (e === 'FALLIDA') return 'status-badge status-error';
  return 'status-badge status-sold';
}

export function obtenerFotoVehiculo(vehiculo) {
  return vehiculo?.imagenUrl || vehiculo?.fotoUrl || heroShowroomImg;
}
