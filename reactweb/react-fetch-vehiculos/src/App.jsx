import { useEffect, useMemo, useState } from 'react';
import { AlertTriangle, CheckCircle2, Info } from 'lucide-react';
import { iniciarSesionWeb, listarVehiculos, listarVentas } from './Api/vehiculosApi.js';
import LoginScreen from './components/login/LoginScreen.jsx';
import Sidebar from './components/layout/Sidebar.jsx';
import Topbar from './components/layout/Topbar.jsx';
import ShowroomBanner from './components/dashboard/ShowroomBanner.jsx';
import ResumenKpis from './components/dashboard/ResumenKpis.jsx';
import CatalogoToolbar from './components/vehiculos/CatalogoToolbar.jsx';
import TablaVehiculos from './components/vehiculos/TablaVehiculos.jsx';
import TarjetasVehiculos from './components/vehiculos/TarjetasVehiculos.jsx';
import FichaTecnica from './components/vehiculos/FichaTecnica.jsx';
import VentasToolbar from './components/ventas/VentasToolbar.jsx';
import TablaVentas from './components/ventas/TablaVentas.jsx';
import Pagination from './components/common/Pagination.jsx';
import './styles/shared.css';

/* ==================== APLICACIÓN PRINCIPAL ==================== */
export default function App() {
  const [usuario, setUsuario] = useState('');
  const [password, setPassword] = useState('');
  const [mostrarPassword, setMostrarPassword] = useState(false);
  const [usuarioActivo, setUsuarioActivo] = useState('');
  const [token, setToken] = useState('');
  const [expiraSeg, setExpiraSeg] = useState(null);

  const [vehiculos, setVehiculos] = useState([]);
  const [ventas, setVentas] = useState([]);
  const [marca, setMarca] = useState('');
  const [filtroEstado, setFiltroEstado] = useState('TODOS');
  const [ordenVehiculos, setOrdenVehiculos] = useState('id-desc');
  const [modoVista, setModoVista] = useState('tabla');
  const [seleccionado, setSeleccionado] = useState(null);

  // Filtros independientes para seguimiento de ventas
  const [busquedaVenta, setBusquedaVenta] = useState('');
  const [filtroEstadoVenta, setFiltroEstadoVenta] = useState('TODAS');

  // Paginación independiente para vehículos y ventas
  const [paginaVeh, setPaginaVeh] = useState(1);
  const [porPaginaVeh, setPorPaginaVeh] = useState(6);
  const [paginaVen, setPaginaVen] = useState(1);
  const [porPaginaVen, setPorPaginaVen] = useState(10);

  // Navegación sidebar
  const [seccion, setSeccion] = useState('dashboard');
  const [menuMovilAbierto, setMenuMovilAbierto] = useState(false);

  const [cargando, setCargando] = useState(false);
  const [consultadoVeh, setConsultadoVeh] = useState(false);
  const [consultadoVen, setConsultadoVen] = useState(false);
  const [ultimaSync, setUltimaSync] = useState('');

  const [mensaje, setMensaje] = useState('');
  const [errorVeh, setErrorVeh] = useState('');
  const [errorVen, setErrorVen] = useState('');
  const [errorAuth, setErrorAuth] = useState('');
  const [codigoHttp, setCodigoHttp] = useState(null);

  function marcarHoraSync() {
    setUltimaSync(
      new Date().toLocaleTimeString('es-EC', { hour: '2-digit', minute: '2-digit', second: '2-digit' }),
    );
  }

  async function cargarVehiculosInterno(jwtActual) {
    setErrorVeh('');
    try {
      const res = await listarVehiculos(jwtActual);
      setVehiculos(res.vehiculos);
      setConsultadoVeh(true);
      setCodigoHttp(res.codigoHttp);
      marcarHoraSync();
      return res;
    } catch (e) {
      setVehiculos([]);
      setSeleccionado(null);
      setErrorVeh(e.message);
      setCodigoHttp(e.status ?? null);
      return null;
    }
  }

  async function cargarVentasInterno(jwtActual) {
    setErrorVen('');
    try {
      const res = await listarVentas(jwtActual);
      setVentas(res.ventas);
      setConsultadoVen(true);
      marcarHoraSync();
      return res;
    } catch (e) {
      setVentas([]);
      setErrorVen(e.message);
      if (e.status) setCodigoHttp(e.status);
      return null;
    }
  }

  async function manejarLogin(evento) {
    evento.preventDefault();
    if (!usuario.trim() || !password) {
      setErrorAuth('Ingrese usuario y contraseña para continuar.');
      return;
    }
    setCargando(true);
    setErrorAuth('');
    setErrorVeh('');
    setErrorVen('');
    try {
      const res = await iniciarSesionWeb(usuario, password);
      const userClean = usuario.trim();
      setToken(res.accessToken);
      setUsuarioActivo(userClean);
      setExpiraSeg(res.expiresIn || 1800);
      setPassword('');
      setMostrarPassword(false);
      setCodigoHttp(res.codigoHttp);
      setMensaje(`Sesión autenticada como '${userClean}'. Sincronizando datos…`);

      const [rVeh, rVen] = await Promise.all([
        cargarVehiculosInterno(res.accessToken),
        cargarVentasInterno(res.accessToken),
      ]);
      if (rVeh || rVen) {
        setMensaje(
          `Sincronizado vía NGINX (:8090): ${rVeh?.vehiculos.length ?? 0} vehículo(s) y ${rVen?.ventas.length ?? 0} venta(s).`,
        );
      }
    } catch (e) {
      setToken('');
      setUsuarioActivo('');
      setCodigoHttp(e.status ?? null);
      setErrorAuth(e.message);
    } finally {
      setCargando(false);
    }
  }

  async function consultarTodo() {
    setCargando(true);
    setErrorAuth('');
    setMensaje('Actualizando inventario y ventas vía NGINX (:8090)…');
    const [rVeh, rVen] = await Promise.all([
      cargarVehiculosInterno(token),
      cargarVentasInterno(token),
    ]);
    if (rVeh && rVen) {
      setMensaje(`Inventario actualizado: ${rVeh.vehiculos.length} vehículo(s) y ${rVen.ventas.length} venta(s).`);
    } else if (rVeh || rVen) {
      setMensaje('Sincronización parcial: uno de los servicios devolvió error.');
    } else {
      setMensaje('No se pudo completar la consulta contra los servicios.');
    }
    setCargando(false);
  }

  async function consultarSoloVehiculos() {
    setCargando(true);
    const rVeh = await cargarVehiculosInterno(token);
    if (rVeh) setMensaje(`GET /api/vehiculos: ${rVeh.vehiculos.length} vehículo(s) actualizados.`);
    setCargando(false);
  }

  async function consultarSoloVentas() {
    setCargando(true);
    const rVen = await cargarVentasInterno(token);
    if (rVen) {
      setCodigoHttp(rVen.codigoHttp);
      setMensaje(`GET /api/ventas: ${rVen.ventas.length} venta(s) consultadas desde FastAPI.`);
    }
    setCargando(false);
  }

  function cerrarSesion() {
    setToken('');
    setUsuarioActivo('');
    setExpiraSeg(null);
    setVehiculos([]);
    setVentas([]);
    setMarca('');
    setFiltroEstado('TODOS');
    setBusquedaVenta('');
    setFiltroEstadoVenta('TODAS');
    setPaginaVeh(1);
    setPaginaVen(1);
    setSeccion('dashboard');
    setMenuMovilAbierto(false);
    setSeleccionado(null);
    setConsultadoVeh(false);
    setConsultadoVen(false);
    setErrorAuth('');
    setErrorVeh('');
    setErrorVen('');
    setCodigoHttp(null);
    setMensaje('');
    setUltimaSync('');
  }

  // Filtrado y ordenamiento reactivo de vehículos
  const visibles = useMemo(() => {
    const texto = marca.trim().toLowerCase();
    const filtrados = vehiculos.filter((v) => {
      const okTexto =
        !texto ||
        String(v.marca ?? '').toLowerCase().includes(texto) ||
        String(v.modelo ?? '').toLowerCase().includes(texto) ||
        String(v.placa ?? '').toLowerCase().includes(texto);
      const okEstado = filtroEstado === 'TODOS' || String(v.estado ?? '').toUpperCase() === filtroEstado;
      return okTexto && okEstado;
    });

    return [...filtrados].sort((a, b) => {
      const idA = Number(a.idVehiculo ?? a.id ?? 0);
      const idB = Number(b.idVehiculo ?? b.id ?? 0);
      const pA = Number(a.precio ?? 0);
      const pB = Number(b.precio ?? 0);
      const yA = Number(a.anio ?? 0);
      const yB = Number(b.anio ?? 0);

      if (ordenVehiculos === 'precio-asc') return pA - pB;
      if (ordenVehiculos === 'precio-desc') return pB - pA;
      if (ordenVehiculos === 'anio-desc') return yB - yA;
      if (ordenVehiculos === 'estado') return String(a.estado ?? '').localeCompare(String(b.estado ?? ''));
      return idB - idA;
    });
  }, [vehiculos, marca, filtroEstado, ordenVehiculos]);

  // Sincronizar ficha seleccionada con los filtros activos
  useEffect(() => {
    if (visibles.length === 0) {
      setSeleccionado(null);
      return;
    }
    const idActual = seleccionado ? (seleccionado.idVehiculo ?? seleccionado.id) : null;
    const sigueVisible = visibles.some((v) => (v.idVehiculo ?? v.id) === idActual);
    if (!sigueVisible) {
      setSeleccionado(visibles[0]);
    }
  }, [visibles, seleccionado]);

  // Filtrado de ventas (solo consulta)
  const ventasVisibles = useMemo(() => {
    const q = busquedaVenta.trim().toLowerCase();
    return ventas.filter((v) => {
      const est = String(v.estado ?? '').toUpperCase();
      const okEst =
        filtroEstadoVenta === 'TODAS' ||
        (filtroEstadoVenta === 'COMPLETADA' && est === 'COMPLETADA') ||
        (filtroEstadoVenta === 'PENDIENTE' && (est === 'PENDIENTE' || est === 'EN_REVISION')) ||
        (filtroEstadoVenta === 'FALLIDA' && est === 'FALLIDA');
      const okQ =
        !q ||
        String(v.id ?? '').includes(q) ||
        String(v.vehiculoId ?? '').includes(q) ||
        String(v.compradorId ?? '').includes(q) ||
        String(v.operacionId ?? '').toLowerCase().includes(q);
      return okEst && okQ;
    });
  }, [ventas, busquedaVenta, filtroEstadoVenta]);

  // Reiniciar a página 1 cuando cambian filtros, orden o tamaño de página
  useEffect(() => {
    setPaginaVeh(1);
  }, [marca, filtroEstado, ordenVehiculos, porPaginaVeh]);

  useEffect(() => {
    setPaginaVen(1);
  }, [busquedaVenta, filtroEstadoVenta, porPaginaVen]);

  const totalPaginasVeh = Math.max(1, Math.ceil(visibles.length / porPaginaVeh));
  const paginaVehSegura = Math.min(paginaVeh, totalPaginasVeh);
  const vehiculosPaginados = useMemo(() => {
    const inicio = (paginaVehSegura - 1) * porPaginaVeh;
    return visibles.slice(inicio, inicio + porPaginaVeh);
  }, [visibles, paginaVehSegura, porPaginaVeh]);

  const totalPaginasVen = Math.max(1, Math.ceil(ventasVisibles.length / porPaginaVen));
  const paginaVenSegura = Math.min(paginaVen, totalPaginasVen);
  const ventasPaginadas = useMemo(() => {
    const inicio = (paginaVenSegura - 1) * porPaginaVen;
    return ventasVisibles.slice(inicio, inicio + porPaginaVen);
  }, [ventasVisibles, paginaVenSegura, porPaginaVen]);

  function limpiarFiltrosVehiculos() {
    setMarca('');
    setFiltroEstado('TODOS');
    setOrdenVehiculos('id-desc');
  }

  function limpiarFiltrosVentas() {
    setBusquedaVenta('');
    setFiltroEstadoVenta('TODAS');
  }

  // ==================== PANTALLA 1: LOGIN OSCURO NEXORA ====================
  if (!token) {
    return (
      <LoginScreen
        usuario={usuario}
        password={password}
        mostrarPassword={mostrarPassword}
        cargando={cargando}
        errorAuth={errorAuth}
        codigoHttp={codigoHttp}
        onChangeUsuario={setUsuario}
        onChangePassword={setPassword}
        onTogglePassword={() => setMostrarPassword((v) => !v)}
        onSubmit={manejarLogin}
      />
    );
  }

  // ==================== PANTALLA 2: CONTROL PANEL AUTENTICADO ====================
  const hayFiltroVeh = marca.trim().length > 0 || filtroEstado !== 'TODOS';
  const hayFiltroVen = busquedaVenta.trim().length > 0 || filtroEstadoVenta !== 'TODAS';
  const tieneError = Boolean(errorVeh || errorVen);

  const claseBanner = tieneError
    ? 'http-banner http-banner-error'
    : codigoHttp && codigoHttp >= 200 && codigoHttp < 300
      ? 'http-banner http-banner-ok'
      : 'http-banner http-banner-neutral';

  const tituloSeccion =
    seccion === 'dashboard'
      ? 'Panel de control'
      : seccion === 'catalogo'
        ? 'Catálogo de vehículos'
        : 'Seguimiento de ventas';

  const subSeccion =
    seccion === 'dashboard'
      ? 'Resumen integral de inventario multicanal y transacciones comerciales en tiempo real'
      : seccion === 'catalogo'
        ? 'Exploración de la flota de Spring Boot mediante tabla interactiva o tarjetas visuales'
        : 'Auditoría de órdenes comerciales registradas en FastAPI (modo solo lectura)';

  function navegarA(dest) {
    setSeccion(dest);
    setMenuMovilAbierto(false);
  }

  return (
    <div className="app-layout">
      <Sidebar
        seccion={seccion}
        vehiculosCount={vehiculos.length}
        ventasCount={ventas.length}
        tieneError={tieneError}
        usuarioActivo={usuarioActivo}
        ultimaSync={ultimaSync}
        expiraSeg={expiraSeg}
        cargando={cargando}
        menuMovilAbierto={menuMovilAbierto}
        onNavegar={navegarA}
        onCerrarSesion={cerrarSesion}
        onCerrarMenuMovil={() => setMenuMovilAbierto(false)}
      />

      <div className="main-area">
        <Topbar
          tituloSeccion={tituloSeccion}
          subSeccion={subSeccion}
          cargando={cargando}
          onToggleMenuMovil={() => setMenuMovilAbierto((v) => !v)}
          onConsultarTodo={consultarTodo}
        />

        <main className="workspace fade-in">
          {/* BANNER DE ESTADO HTTP */}
          {mensaje && (
            <div className={claseBanner} role="status" aria-live="polite">
              <span className="http-banner-left">
                {tieneError ? (
                  <AlertTriangle size={18} />
                ) : (
                  <CheckCircle2 size={18} />
                )}
                <span className="http-banner-msg">{mensaje}</span>
              </span>
              {codigoHttp !== null && <span className="http-code-tag">HTTP {codigoHttp}</span>}
            </div>
          )}

          {/* PANEL DE CONTROL: BANNER SHOWROOM + KPIs + DISTRIBUCIÓN */}
          {seccion === 'dashboard' && (
            <>
              <ShowroomBanner
                cargando={cargando}
                onConsultarSoloVehiculos={consultarSoloVehiculos}
                onConsultarSoloVentas={consultarSoloVentas}
              />
              <ResumenKpis vehiculos={vehiculos} ventas={ventas} />
            </>
          )}

          {/* SECCIÓN DE CATÁLOGO (EN PANEL DE CONTROL Y EN CATÁLOGO) */}
          {(seccion === 'dashboard' || seccion === 'catalogo') && (
            <>
              <CatalogoToolbar
                marca={marca}
                filtroEstado={filtroEstado}
                ordenVehiculos={ordenVehiculos}
                modoVista={modoVista}
                cargando={cargando}
                hayFiltroVeh={hayFiltroVeh}
                onChangeMarca={setMarca}
                onChangeFiltroEstado={setFiltroEstado}
                onChangeOrden={setOrdenVehiculos}
                onChangeModoVista={setModoVista}
                onLimpiar={limpiarFiltrosVehiculos}
              />

              <section className="catalog-grid" aria-labelledby="titulo-catalogo">
                <div className="panel-card catalog-main-panel">
                  <div className="panel-header">
                    <div>
                      <h3 id="titulo-catalogo">Inventario automotriz</h3>
                      <p className="panel-subtitle">
                        Fuente: <code>GET /api/vehiculos</code> · Mostrando {visibles.length} de {vehiculos.length} vehículo(s).
                      </p>
                    </div>
                    <span className="badge-graphite tabular">{visibles.length} resultado(s)</span>
                  </div>

                  {errorVeh && (
                    <div className="http-banner http-banner-error" role="alert" style={{ marginBottom: 16 }}>
                      <span className="http-banner-left">
                        <AlertTriangle size={16} /> <span>{errorVeh}</span>
                      </span>
                    </div>
                  )}

                  {modoVista === 'tabla' ? (
                    <TablaVehiculos
                      vehiculos={vehiculosPaginados}
                      cargando={cargando}
                      consultado={consultadoVeh}
                      errorVeh={errorVeh}
                      hayFiltro={hayFiltroVeh}
                      seleccionado={seleccionado}
                      onSeleccionar={setSeleccionado}
                    />
                  ) : (
                    <TarjetasVehiculos
                      vehiculos={vehiculosPaginados}
                      cargando={cargando}
                      consultado={consultadoVeh}
                      errorVeh={errorVeh}
                      hayFiltro={hayFiltroVeh}
                      seleccionado={seleccionado}
                      onSeleccionar={setSeleccionado}
                    />
                  )}

                  <Pagination
                    paginaActual={paginaVehSegura}
                    totalItems={visibles.length}
                    porPagina={porPaginaVeh}
                    opcionesPorPagina={[6, 10, 15, 25]}
                    etiquetaItems="vehículo(s)"
                    onCambiarPagina={setPaginaVeh}
                    onCambiarPorPagina={setPorPaginaVeh}
                  />
                </div>

                <aside className="panel-card detail-sticky" aria-labelledby="titulo-ficha">
                  <div className="panel-header">
                    <div>
                      <h3 id="titulo-ficha">Ficha técnica</h3>
                      <p className="panel-subtitle">Especificaciones verificadas del vehículo</p>
                    </div>
                  </div>
                  <FichaTecnica seleccionado={seleccionado} />
                </aside>
              </section>
            </>
          )}

          {/* SECCIÓN DE SEGUIMIENTO DE VENTAS (SOLO LECTURA) */}
          {seccion === 'ventas' && (
            <>
              <VentasToolbar
                busquedaVenta={busquedaVenta}
                filtroEstadoVenta={filtroEstadoVenta}
                cargando={cargando}
                hayFiltroVen={hayFiltroVen}
                onChangeBusqueda={setBusquedaVenta}
                onChangeFiltroEstado={setFiltroEstadoVenta}
                onRecargar={consultarSoloVentas}
                onLimpiar={limpiarFiltrosVentas}
              />

              <section className="panel-card" aria-labelledby="titulo-ventas">
                <div className="panel-header">
                  <div>
                    <h3 id="titulo-ventas">Historial y seguimiento de ventas</h3>
                    <p className="panel-subtitle">
                      Fuente: <code>GET /api/ventas</code> · Mostrando {ventasVisibles.length} de {ventas.length} orden(es).
                    </p>
                  </div>
                  <span className="badge-graphite tabular">{ventasVisibles.length} orden(es)</span>
                </div>

                <div className="readonly-callout">
                  <div className="readonly-callout-icon">
                    <Info size={18} />
                  </div>
                  <div className="readonly-callout-text">
                    <strong>Modo de consulta y auditoría:</strong>
                    <span>
                      Esta consola web supervisa ventas reales y estados de notificación. La emisión y captura de transacciones es exclusiva de la aplicación móvil PWA.
                    </span>
                  </div>
                </div>

                {errorVen && (
                  <div className="http-banner http-banner-error" role="alert" style={{ marginBottom: 16 }}>
                    <span className="http-banner-left">
                      <AlertTriangle size={16} /> <span>{errorVen}</span>
                    </span>
                  </div>
                )}

                <TablaVentas
                  ventas={ventasPaginadas}
                  cargando={cargando}
                  consultado={consultadoVen}
                  errorVen={errorVen}
                  hayFiltro={hayFiltroVen}
                />

                <Pagination
                  paginaActual={paginaVenSegura}
                  totalItems={ventasVisibles.length}
                  porPagina={porPaginaVen}
                  opcionesPorPagina={[6, 10, 15, 25]}
                  etiquetaItems="orden(es)"
                  onCambiarPagina={setPaginaVen}
                  onCambiarPorPagina={setPorPaginaVen}
                />
              </section>
            </>
          )}
        </main>
      </div>
    </div>
  );
}
