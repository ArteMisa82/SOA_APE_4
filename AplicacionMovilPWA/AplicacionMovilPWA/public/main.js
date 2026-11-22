import {
  iniciarSesion,
  listarVehiculos,
  listarCompradores,
  registrarVenta,
  consultarVentaPorId,
  generarClaveIdempotente,
} from './api.js';

// Estado en memoria únicamente (nunca se guarda el JWT en localStorage/sessionStorage).
let sesionJwt = '';
let sesionUsuario = '';

const formLogin = document.querySelector('#formLogin');
const inputUsuario = document.querySelector('#usuario');
const inputPassword = document.querySelector('#password');
const btnLogin = document.querySelector('#btnLogin');
const btnLogout = document.querySelector('#btnLogout');
const btnJwtInvalido = document.querySelector('#btnJwtInvalido');
const badgeSesion = document.querySelector('#badgeSesion');
const mensajeLogin = document.querySelector('#mensajeLogin');

const formVenta = document.querySelector('#formVenta');
const selectVehiculo = document.querySelector('#selectVehiculo');
const selectComprador = document.querySelector('#selectComprador');
const inputIdempotency = document.querySelector('#idempotencyKey');
const btnNuevaClave = document.querySelector('#btnNuevaClave');
const btnRegistrarVenta = document.querySelector('#btnRegistrarVenta');
const btnRecargar = document.querySelector('#btnRecargar');
const mensajeVenta = document.querySelector('#mensajeVenta');
const resultadoVenta = document.querySelector('#resultadoVenta');

const formConsultaVenta = document.querySelector('#formConsultaVenta');
const inputConsultaVentaId = document.querySelector('#consultaVentaId');
const btnConsultarVenta = document.querySelector('#btnConsultarVenta');
const mensajeConsulta = document.querySelector('#mensajeConsulta');
const detalleConsulta = document.querySelector('#detalleConsulta');

const contenedorVehiculos = document.querySelector('#vehiculos');
const contadorVehiculos = document.querySelector('#contador');
const conexion = document.querySelector('#conexion');

const moneda = new Intl.NumberFormat('es-EC', { style: 'currency', currency: 'USD' });

function mostrarMensaje(elemento, texto, tipo = 'neutro') {
  elemento.textContent = texto;
  elemento.className = `mensaje ${tipo}`;
}

function crearElemento(etiqueta, clase, contenido) {
  const el = document.createElement(etiqueta);
  if (clase) el.className = clase;
  if (contenido !== undefined) el.textContent = String(contenido);
  return el;
}

function texto(valor) {
  return valor === null || valor === undefined || valor === '' ? '—' : String(valor);
}

function formatoPrecio(valor) {
  if (valor === null || valor === undefined || valor === '') return '—';
  const numero = Number(valor);
  return Number.isFinite(numero) ? moneda.format(numero) : texto(valor);
}

function agregarDato(grilla, etiqueta, valor, extra = '') {
  const grupo = crearElemento('div', extra);
  grupo.append(
    crearElemento('span', 'dato-etiqueta', etiqueta),
    crearElemento('span', 'dato-valor', valor),
  );
  grilla.append(grupo);
}

function pintarTarjetaVenta(contenedor, venta, codigoHttp) {
  contenedor.replaceChildren();
  contenedor.classList.remove('oculto');

  const cabecera = crearElemento('div', 'vehiculo-cabecera');
  const titulo = crearElemento('div');
  titulo.append(
    crearElemento('h3', 'vehiculo-titulo', `Venta #${texto(venta.id)}`),
    crearElemento('span', 'vehiculo-id', `Operación: ${texto(venta.operacionId)}`),
  );
  const estadoStr = texto(venta.estado);
  const claseEstado = estadoStr === 'COMPLETADA' ? 'disponible' : estadoStr === 'FALLIDA' ? 'vendido' : '';
  cabecera.append(titulo, crearElemento('span', `estado ${claseEstado}`, `${estadoStr} (HTTP ${codigoHttp})`));

  const grilla = crearElemento('div', 'datos');
  agregarDato(grilla, 'Vehículo ID', texto(venta.vehiculoId));
  agregarDato(grilla, 'Comprador ID', texto(venta.compradorId));
  agregarDato(grilla, 'Precio acordado', formatoPrecio(venta.precioAcordado), 'dato-precio');
  agregarDato(grilla, 'Moneda', texto(venta.moneda));
  agregarDato(grilla, 'Notificación', texto(venta.notificacion));
  agregarDato(grilla, 'Fecha UTC', texto(venta.fechaUTC));

  contenedor.append(cabecera, grilla);
}

function pintarSelectVehiculos(vehiculos) {
  const seleccionPrevia = selectVehiculo.value;
  selectVehiculo.replaceChildren();
  selectVehiculo.append(crearElemento('option', '', '— Seleccione un vehículo —'));

  // Ordenar disponibles primero para agilizar el flujo móvil
  const ordenados = [...vehiculos].sort((a, b) => {
    const aDisp = String(a.estado).toUpperCase() === 'DISPONIBLE' ? 0 : 1;
    const bDisp = String(b.estado).toUpperCase() === 'DISPONIBLE' ? 0 : 1;
    return aDisp - bDisp || (a.idVehiculo - b.idVehiculo);
  });

  for (const v of ordenados) {
    const id = v.idVehiculo ?? v.id;
    const opt = document.createElement('option');
    opt.value = String(id);
    opt.textContent = `[#${id}] ${texto(v.marca)} ${texto(v.modelo)} (${texto(v.placa)}) — ${formatoPrecio(v.precio)} · ${texto(v.estado)}`;
    selectVehiculo.append(opt);
  }
  if (seleccionPrevia && [...selectVehiculo.options].some((o) => o.value === seleccionPrevia)) {
    selectVehiculo.value = seleccionPrevia;
  }
}

function pintarSelectCompradores(compradores) {
  const seleccionPrevia = selectComprador.value;
  selectComprador.replaceChildren();
  selectComprador.append(crearElemento('option', '', '— Seleccione un comprador —'));

  for (const c of compradores) {
    const opt = document.createElement('option');
    opt.value = String(c.id);
    opt.textContent = `[#${c.id}] ${texto(c.nombre)} (${texto(c.documento)}) — ${texto(c.correo)}`;
    selectComprador.append(opt);
  }
  if (seleccionPrevia && [...selectComprador.options].some((o) => o.value === seleccionPrevia)) {
    selectComprador.value = seleccionPrevia;
  }
}

function pintarListadoVehiculos(vehiculos) {
  contenedorVehiculos.replaceChildren();
  contadorVehiculos.textContent = `${vehiculos.length} vehículo${vehiculos.length === 1 ? '' : 's'}`;

  if (vehiculos.length === 0) {
    contenedorVehiculos.append(crearElemento('p', 'vacio', 'HTTP 200: no hay vehículos registrados.'));
    return;
  }

  for (const vehiculo of vehiculos) {
    const id = vehiculo.idVehiculo ?? vehiculo.id;
    const estado = texto(vehiculo.estado);
    const esDisponible = String(estado).toUpperCase() === 'DISPONIBLE';
    const claseEstado = esDisponible ? 'disponible' : String(estado).toUpperCase() === 'VENDIDO' ? 'vendido' : '';

    const tarjeta = crearElemento('article', `vehiculo${esDisponible ? ' seleccionable' : ''}`);
    const cabecera = crearElemento('div', 'vehiculo-cabecera');
    const encabezado = crearElemento('div');
    encabezado.append(
      crearElemento('h3', 'vehiculo-titulo', `${texto(vehiculo.marca)} ${texto(vehiculo.modelo)}`),
      crearElemento('span', 'vehiculo-id', `ID: ${texto(id)}`),
    );
    cabecera.append(encabezado, crearElemento('span', `estado ${claseEstado}`, estado));

    const grilla = crearElemento('div', 'datos');
    agregarDato(grilla, 'Placa', texto(vehiculo.placa));
    agregarDato(grilla, 'Año', texto(vehiculo.anio));
    agregarDato(grilla, 'Color', texto(vehiculo.color));
    agregarDato(grilla, 'Precio', formatoPrecio(vehiculo.precio), 'dato-precio');
    tarjeta.append(cabecera, grilla);

    tarjeta.addEventListener('click', () => {
      selectVehiculo.value = String(id);
      mostrarMensaje(mensajeVenta, `Vehículo #${id} (${texto(vehiculo.marca)} ${texto(vehiculo.modelo)}) seleccionado.`, 'neutro');
      formVenta.scrollIntoView({ behavior: 'smooth', block: 'center' });
    });

    contenedorVehiculos.append(tarjeta);
  }
}

async function cargarCatalogos() {
  btnRecargar.disabled = true;
  try {
    const [resVeh, resComp] = await Promise.all([
      listarVehiculos(sesionJwt),
      listarCompradores(sesionJwt),
    ]);
    pintarListadoVehiculos(resVeh.vehiculos);
    pintarSelectVehiculos(resVeh.vehiculos);
    pintarSelectCompradores(resComp.compradores);
    const disponibles = resVeh.vehiculos.filter((v) => String(v.estado).toUpperCase() === 'DISPONIBLE').length;
    mostrarMensaje(
      mensajeVenta,
      `Datos actualizados (HTTP 200): ${resVeh.vehiculos.length} vehículo(s) (${disponibles} disponible/s) y ${resComp.compradores.length} comprador(es).`,
      'neutro',
    );
  } catch (error) {
    mostrarMensaje(mensajeVenta, error.message, 'error');
    contenedorVehiculos.replaceChildren(crearElemento('p', 'vacio', 'No se pudieron cargar los datos del servicio.'));
    contadorVehiculos.textContent = '0 vehículos';
  } finally {
    btnRecargar.disabled = false;
  }
}

// ==================== EVENTOS ====================

formLogin.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  const usuario = inputUsuario.value.trim();
  const password = inputPassword.value;

  if (!usuario || !password) {
    mostrarMensaje(mensajeLogin, 'Ingrese usuario y contraseña para autenticarse.', 'error');
    return;
  }

  btnLogin.disabled = true;
  mostrarMensaje(mensajeLogin, 'Autenticando mediante POST /oauth/token vía NGINX…', 'neutro');
  try {
    const res = await iniciarSesion(usuario, password);
    sesionJwt = res.accessToken;
    sesionUsuario = usuario;
    inputPassword.value = '';
    btnLogout.disabled = false;
    badgeSesion.textContent = `Activa: ${sesionUsuario}`;
    badgeSesion.className = 'contador sesion-ok';
    mostrarMensaje(
      mensajeLogin,
      `HTTP ${res.codigoHttp}: Sesión iniciada para '${sesionUsuario}' (JWT guardado solo en memoria; expira en ${res.expiresIn || 1800} s).`,
      'exito',
    );
    inputIdempotency.value = generarClaveIdempotente();
    await cargarCatalogos();
  } catch (error) {
    sesionJwt = '';
    sesionUsuario = '';
    btnLogout.disabled = true;
    badgeSesion.textContent = 'Sin sesión';
    badgeSesion.className = 'contador';
    mostrarMensaje(mensajeLogin, error.message, 'error');
  } finally {
    btnLogin.disabled = false;
  }
});

btnLogout.addEventListener('click', () => {
  sesionJwt = '';
  sesionUsuario = '';
  btnLogout.disabled = true;
  badgeSesion.textContent = 'Sin sesión';
  badgeSesion.className = 'contador';
  selectVehiculo.replaceChildren(crearElemento('option', '', '— Inicie sesión para cargar vehículos —'));
  selectComprador.replaceChildren(crearElemento('option', '', '— Inicie sesión para cargar compradores —'));
  contenedorVehiculos.replaceChildren(crearElemento('div', 'vacio', 'Sesión cerrada. Inicie sesión para continuar.'));
  contadorVehiculos.textContent = '0 vehículos';
  resultadoVenta.classList.add('oculto');
  detalleConsulta.classList.add('oculto');
  mostrarMensaje(mensajeLogin, 'Sesión cerrada y JWT eliminado de memoria.', 'neutro');
});

btnJwtInvalido.addEventListener('click', () => {
  sesionJwt = 'jwt_invalido_o_vencido_prueba_401';
  sesionUsuario = 'prueba-401';
  btnLogout.disabled = false;
  badgeSesion.textContent = 'JWT inválido (prueba)';
  badgeSesion.className = 'contador sesion-alerta';
  mostrarMensaje(
    mensajeLogin,
    'Se cargó un JWT inválido en memoria. Intente recargar datos o registrar una venta para comprobar HTTP 401.',
    'error',
  );
});

btnNuevaClave.addEventListener('click', () => {
  inputIdempotency.value = generarClaveIdempotente();
  mostrarMensaje(mensajeVenta, `Nueva Idempotency-Key generada: ${inputIdempotency.value}`, 'neutro');
});

btnRecargar.addEventListener('click', async () => {
  await cargarCatalogos();
});

formVenta.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  const vehiculoId = selectVehiculo.value;
  const compradorId = selectComprador.value;
  const idempotencyKey = inputIdempotency.value.trim();

  if (!vehiculoId || !compradorId) {
    mostrarMensaje(mensajeVenta, 'Seleccione un vehículo y un comprador antes de registrar la venta.', 'error');
    return;
  }
  if (!/^[A-Za-z0-9_-]{8,100}$/.test(idempotencyKey)) {
    mostrarMensaje(mensajeVenta, 'La Idempotency-Key debe tener entre 8 y 100 caracteres (letras, números, - o _).', 'error');
    return;
  }

  btnRegistrarVenta.disabled = true;
  mostrarMensaje(mensajeVenta, 'Enviando POST /api/ventas con JWT e Idempotency-Key vía NGINX…', 'neutro');
  try {
    const res = await registrarVenta(sesionJwt, { vehiculoId, compradorId, idempotencyKey });
    pintarTarjetaVenta(resultadoVenta, res.venta, res.codigoHttp);
    inputConsultaVentaId.value = String(res.venta.id);
    const tipoMsg = res.codigoHttp === 201
      ? `HTTP 201 Created: Venta #${res.venta.id} confirmada (${res.venta.estado}).`
      : res.codigoHttp === 200
        ? `HTTP 200 OK (Reintento idempotente): Se devolvió la venta existente #${res.venta.id} sin duplicación.`
        : `HTTP ${res.codigoHttp}: Venta #${res.venta.id} en estado ${res.venta.estado}. Consulte GET /api/ventas/${res.venta.id}.`;
    mostrarMensaje(mensajeVenta, tipoMsg, 'exito');
    await cargarCatalogos();
  } catch (error) {
    mostrarMensaje(mensajeVenta, error.message, 'error');
  } finally {
    btnRegistrarVenta.disabled = false;
  }
});

formConsultaVenta.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  const ventaId = inputConsultaVentaId.value.trim();
  if (!ventaId) {
    mostrarMensaje(mensajeConsulta, 'Ingrese el ID de la venta a consultar.', 'error');
    return;
  }

  btnConsultarVenta.disabled = true;
  mostrarMensaje(mensajeConsulta, `Consultando GET /api/ventas/${ventaId} vía NGINX…`, 'neutro');
  try {
    const res = await consultarVentaPorId(sesionJwt, ventaId);
    pintarTarjetaVenta(detalleConsulta, res.venta, res.codigoHttp);
    mostrarMensaje(
      mensajeConsulta,
      `HTTP ${res.codigoHttp}: Venta #${res.venta.id} recuperada en estado ${res.venta.estado}.`,
      'exito',
    );
  } catch (error) {
    detalleConsulta.classList.add('oculto');
    mostrarMensaje(mensajeConsulta, error.message, 'error');
  } finally {
    btnConsultarVenta.disabled = false;
  }
});

// Inicializar clave idempotente sugerida
inputIdempotency.value = generarClaveIdempotente();

function actualizarConexion() {
  conexion.textContent = navigator.onLine ? 'Navegador conectado' : 'Sin conexión de red';
}
window.addEventListener('online', actualizarConexion);
window.addEventListener('offline', actualizarConexion);
actualizarConexion();

// Service Worker: solo recursos visuales, nunca /api ni /oauth.
if ('serviceWorker' in navigator && window.isSecureContext) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js').catch(() => {});
  });
}
