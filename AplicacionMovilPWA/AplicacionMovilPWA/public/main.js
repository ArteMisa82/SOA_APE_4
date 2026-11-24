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
let vehiculosCache = [];
let filtroEstadoActivo = 'TODOS';
let busquedaVehiculo = '';

// Pantallas
const screenLogin = document.querySelector('#screen-login');
const screenApp = document.querySelector('#screen-app');

// Login
const formLogin = document.querySelector('#formLogin');
const inputUsuario = document.querySelector('#usuario');
const inputPassword = document.querySelector('#password');
const btnTogglePassword = document.querySelector('#btnTogglePassword');
const icoEyeShow = document.querySelector('#icoEyeShow');
const icoEyeHide = document.querySelector('#icoEyeHide');
const btnLogin = document.querySelector('#btnLogin');
const btnJwtInvalidoLogin = document.querySelector('#btnJwtInvalidoLogin');
const mensajeLogin = document.querySelector('#mensajeLogin');

// Topbar & KPIs
const badgeSesion = document.querySelector('#badgeSesion');
const btnLogout = document.querySelector('#btnLogout');
const btnJwtInvalido = document.querySelector('#btnJwtInvalido');
const kpiDisponibles = document.querySelector('#kpiDisponibles');
const kpiVehiculos = document.querySelector('#kpiVehiculos');
const kpiCompradores = document.querySelector('#kpiCompradores');

// Venta
const formVenta = document.querySelector('#formVenta');
const selectVehiculo = document.querySelector('#selectVehiculo');
const selectComprador = document.querySelector('#selectComprador');
const inputIdempotency = document.querySelector('#idempotencyKey');
const btnNuevaClave = document.querySelector('#btnNuevaClave');
const btnRegistrarVenta = document.querySelector('#btnRegistrarVenta');
const btnRecargar = document.querySelector('#btnRecargar');
const mensajeVenta = document.querySelector('#mensajeVenta');
const resultadoVenta = document.querySelector('#resultadoVenta');

// Consulta
const formConsultaVenta = document.querySelector('#formConsultaVenta');
const inputConsultaVentaId = document.querySelector('#consultaVentaId');
const btnConsultarVenta = document.querySelector('#btnConsultarVenta');
const mensajeConsulta = document.querySelector('#mensajeConsulta');
const detalleConsulta = document.querySelector('#detalleConsulta');

// Catálogo
const contenedorVehiculos = document.querySelector('#vehiculos');
const contadorVehiculos = document.querySelector('#contador');
const chipsFiltro = document.querySelector('#chipsFiltro');
const inputBuscarVehiculo = document.querySelector('#inputBuscarVehiculo');
const conexion = document.querySelector('#conexion');

// Navegación
const navItems = document.querySelectorAll('.bottom-nav .nav-item');
const tabViews = document.querySelectorAll('.tab-view');

const moneda = new Intl.NumberFormat('es-EC', { style: 'currency', currency: 'USD' });

function mostrarPantallaApp(autenticado) {
  screenLogin.classList.toggle('oculto', autenticado);
  screenApp.classList.toggle('oculto', !autenticado);
  window.scrollTo({ top: 0, behavior: 'instant' });
}

function cambiarPestana(nombreTab) {
  tabViews.forEach((vista) => {
    vista.classList.toggle('active', vista.dataset.tab === nombreTab);
  });
  navItems.forEach((btn) => {
    const activo = btn.dataset.target === nombreTab;
    btn.classList.toggle('active', activo);
    if (activo) btn.setAttribute('aria-current', 'page');
    else btn.removeAttribute('aria-current');
  });
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

navItems.forEach((btn) => {
  btn.addEventListener('click', () => cambiarPestana(btn.dataset.target));
});

btnTogglePassword.addEventListener('click', () => {
  const esPass = inputPassword.type === 'password';
  inputPassword.type = esPass ? 'text' : 'password';
  icoEyeShow.classList.toggle('oculto', esPass);
  icoEyeHide.classList.toggle('oculto', !esPass);
});

function mostrarMensajeLogin(textoMsg, tipo = 'neutro') {
  mensajeLogin.textContent = textoMsg;
  mensajeLogin.className = `mensaje-dark ${tipo}`;
}

function mostrarMensaje(elemento, textoMsg, tipo = 'neutro') {
  elemento.textContent = textoMsg;
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

function claseEstadoSemantica(estadoRaw) {
  const e = String(estadoRaw || '').toUpperCase();
  if (e === 'DISPONIBLE' || e === 'COMPLETADA') return 'disponible';
  if (e === 'RESERVADO' || e === 'PENDIENTE' || e === 'EN_REVISION') return 'reservado';
  if (e === 'VENDIDO') return 'vendido';
  if (e === 'FALLIDA') return 'error';
  return 'vendido';
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

  const cabecera = crearElemento('div', 'receipt-head');
  const titulo = crearElemento('div');
  titulo.append(
    crearElemento('h3', 'receipt-title', `Venta #${texto(venta.id)}`),
    crearElemento('span', 'receipt-op', `Operación: ${texto(venta.operacionId)}`),
  );
  const estadoStr = texto(venta.estado);
  const claseEst = claseEstadoSemantica(estadoStr);
  cabecera.append(titulo, crearElemento('span', `estado ${claseEst}`, `${estadoStr} · HTTP ${codigoHttp}`));

  const grilla = crearElemento('div', 'receipt-body');
  agregarDato(grilla, 'Vehículo ID', `#${texto(venta.vehiculoId)}`);
  agregarDato(grilla, 'Comprador ID', `#${texto(venta.compradorId)}`);
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
  vehiculosCache = vehiculos;
  const q = busquedaVehiculo.trim().toLowerCase();

  const filtrados = vehiculos.filter((v) => {
    const okEstado =
      filtroEstadoActivo === 'TODOS' ||
      String(v.estado || '').toUpperCase() === filtroEstadoActivo;
    const okTexto =
      !q ||
      String(v.marca || '').toLowerCase().includes(q) ||
      String(v.modelo || '').toLowerCase().includes(q) ||
      String(v.placa || '').toLowerCase().includes(q);
    return okEstado && okTexto;
  });

  contenedorVehiculos.replaceChildren();
  contadorVehiculos.textContent = `${filtrados.length} de ${vehiculos.length}`;

  if (vehiculos.length === 0) {
    contenedorVehiculos.append(crearElemento('div', 'vacio', 'HTTP 200: no hay vehículos registrados.'));
    return;
  }
  if (filtrados.length === 0) {
    contenedorVehiculos.append(crearElemento('div', 'vacio', 'No hay vehículos que coincidan con los filtros.'));
    return;
  }

  for (const vehiculo of filtrados) {
    const id = vehiculo.idVehiculo ?? vehiculo.id;
    const estado = texto(vehiculo.estado);
    const upper = String(estado).toUpperCase();
    const esDisponible = upper === 'DISPONIBLE';
    const esVendido = upper === 'VENDIDO';
    const claseEst = claseEstadoSemantica(estado);

    const tarjeta = crearElemento(
      'article',
      `vehiculo${esDisponible ? ' seleccionable' : ''}${esVendido ? ' es-vendido' : ''}`,
    );

    const media = crearElemento('div', 'vehiculo-media');
    media.append(
      crearElemento('span', 'vehiculo-id', `ID #${texto(id)} · ${texto(vehiculo.placa)}`),
      crearElemento('span', `estado ${claseEst}`, estado),
    );

    const cuerpo = crearElemento('div', 'vehiculo-body');
    cuerpo.append(crearElemento('h3', 'vehiculo-titulo', `${texto(vehiculo.marca)} ${texto(vehiculo.modelo)}`));

    const grilla = crearElemento('div', 'datos');
    agregarDato(grilla, 'Año', texto(vehiculo.anio));
    agregarDato(grilla, 'Color', texto(vehiculo.color));
    agregarDato(grilla, 'Chasis', texto(vehiculo.chasis));
    agregarDato(grilla, 'Precio', formatoPrecio(vehiculo.precio), 'dato-precio');
    cuerpo.append(grilla);

    tarjeta.append(media, cuerpo);

    tarjeta.addEventListener('click', () => {
      selectVehiculo.value = String(id);
      mostrarMensaje(
        mensajeVenta,
        `Vehículo #${id} (${texto(vehiculo.marca)} ${texto(vehiculo.modelo)}) seleccionado para venta.`,
        esVendido ? 'aviso' : 'neutro',
      );
      cambiarPestana('vender');
    });

    contenedorVehiculos.append(tarjeta);
  }
}

chipsFiltro.addEventListener('click', (e) => {
  const btn = e.target.closest('.chip');
  if (!btn) return;
  filtroEstadoActivo = btn.dataset.estado || 'TODOS';
  chipsFiltro.querySelectorAll('.chip').forEach((c) => c.classList.toggle('active', c === btn));
  if (vehiculosCache.length > 0 || sesionJwt) {
    pintarListadoVehiculos(vehiculosCache);
  }
});

inputBuscarVehiculo.addEventListener('input', (e) => {
  busquedaVehiculo = e.target.value;
  if (vehiculosCache.length > 0 || sesionJwt) {
    pintarListadoVehiculos(vehiculosCache);
  }
});

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
    kpiDisponibles.textContent = String(disponibles);
    kpiVehiculos.textContent = String(resVeh.vehiculos.length);
    kpiCompradores.textContent = String(resComp.compradores.length);

    mostrarMensaje(
      mensajeVenta,
      `HTTP 200: ${resVeh.vehiculos.length} vehículo(s) (${disponibles} disponible/s) y ${resComp.compradores.length} comprador(es) cargados.`,
      'neutro',
    );
  } catch (error) {
    mostrarMensaje(mensajeVenta, error.message, 'error');
    vehiculosCache = [];
    kpiDisponibles.textContent = '0';
    kpiVehiculos.textContent = '0';
    kpiCompradores.textContent = '0';
    contenedorVehiculos.replaceChildren(crearElemento('div', 'vacio', 'No se pudieron cargar los datos del servicio.'));
    contadorVehiculos.textContent = '0';
  } finally {
    btnRecargar.disabled = false;
  }
}

function activarModoJwtInvalido() {
  sesionJwt = 'jwt_invalido_o_vencido_prueba_401';
  sesionUsuario = 'prueba-401';
  badgeSesion.textContent = 'JWT inválido (401)';
  badgeSesion.className = 'session-pill sesion-alerta';
  mostrarPantallaApp(true);
  cambiarPestana('vender');
  mostrarMensaje(
    mensajeVenta,
    'Se cargó un JWT inválido en memoria. Intente recargar o registrar una venta para comprobar HTTP 401.',
    'error',
  );
}

// ==================== EVENTOS ====================

formLogin.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  const usuario = inputUsuario.value.trim();
  const password = inputPassword.value;

  if (!usuario || !password) {
    mostrarMensajeLogin('Ingrese usuario y contraseña para autenticarse.', 'aviso');
    return;
  }

  btnLogin.disabled = true;
  btnLogin.textContent = 'Autenticando…';
  mostrarMensajeLogin('Validando credenciales en POST /oauth/token vía NGINX…', 'neutro');

  try {
    const res = await iniciarSesion(usuario, password);
    sesionJwt = res.accessToken;
    sesionUsuario = usuario;
    inputPassword.value = '';
    inputPassword.type = 'password';
    icoEyeShow.classList.remove('oculto');
    icoEyeHide.classList.add('oculto');

    badgeSesion.textContent = sesionUsuario;
    badgeSesion.className = 'session-pill sesion-ok';
    inputIdempotency.value = generarClaveIdempotente();

    mostrarPantallaApp(true);
    cambiarPestana('vender');
    mostrarMensaje(
      mensajeVenta,
      `HTTP ${res.codigoHttp}: Sesión iniciada para '${sesionUsuario}' (JWT en memoria; expira en ${res.expiresIn || 1800} s). Cargando datos…`,
      'exito',
    );
    await cargarCatalogos();
  } catch (error) {
    sesionJwt = '';
    sesionUsuario = '';
    mostrarMensajeLogin(error.message, 'error');
  } finally {
    btnLogin.disabled = false;
    btnLogin.textContent = 'Iniciar sesión';
  }
});

btnJwtInvalidoLogin.addEventListener('click', activarModoJwtInvalido);
btnJwtInvalido.addEventListener('click', activarModoJwtInvalido);

btnLogout.addEventListener('click', () => {
  sesionJwt = '';
  sesionUsuario = '';
  vehiculosCache = [];
  busquedaVehiculo = '';
  filtroEstadoActivo = 'TODOS';
  inputBuscarVehiculo.value = '';
  kpiDisponibles.textContent = '0';
  kpiVehiculos.textContent = '0';
  kpiCompradores.textContent = '0';

  badgeSesion.textContent = 'Sin sesión';
  badgeSesion.className = 'session-pill';
  selectVehiculo.replaceChildren(crearElemento('option', '', '— Inicie sesión para cargar vehículos —'));
  selectComprador.replaceChildren(crearElemento('option', '', '— Inicie sesión para cargar compradores —'));
  contenedorVehiculos.replaceChildren(crearElemento('div', 'vacio', 'Sesión cerrada.'));
  contadorVehiculos.textContent = '0';
  resultadoVenta.classList.add('oculto');
  detalleConsulta.classList.add('oculto');

  mostrarPantallaApp(false);
  mostrarMensajeLogin('Sesión cerrada y JWT eliminado de memoria.', 'neutro');
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
    mostrarMensaje(mensajeVenta, 'Seleccione un vehículo y un comprador antes de registrar la venta.', 'aviso');
    return;
  }
  if (!/^[A-Za-z0-9_-]{8,100}$/.test(idempotencyKey)) {
    mostrarMensaje(mensajeVenta, 'La Idempotency-Key debe tener entre 8 y 100 caracteres (letras, números, - o _).', 'aviso');
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
    mostrarMensaje(mensajeConsulta, 'Ingrese el ID de la venta a consultar.', 'aviso');
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

inputIdempotency.value = generarClaveIdempotente();

function actualizarConexion() {
  conexion.textContent = navigator.onLine ? 'En línea' : 'Sin conexión';
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
