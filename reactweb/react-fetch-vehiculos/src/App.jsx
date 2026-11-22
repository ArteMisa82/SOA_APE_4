import { useMemo, useState } from 'react';
import { iniciarSesionWeb, listarVehiculos, listarVentas } from './Api/vehiculosApi.js';
import './App.css';

const dinero = new Intl.NumberFormat('es-EC', {
  style: 'currency',
  currency: 'USD',
});

function mostrarPrecio(precio) {
  if (precio === null || precio === undefined || precio === '') {
    return '—';
  }
  const numero = Number(precio);
  return Number.isFinite(numero) ? dinero.format(numero) : String(precio);
}

function claseBadge(estado = '') {
  const e = String(estado).toUpperCase();
  if (e === 'DISPONIBLE' || e === 'COMPLETADA') return 'badge badge-ok';
  if (e === 'RESERVADO' || e === 'PENDIENTE' || e === 'EN_REVISION') return 'badge badge-warn';
  if (e === 'VENDIDO' || e === 'FALLIDA') return 'badge badge-muted';
  return 'badge';
}

function ResumenIndicadores({ vehiculos, ventas }) {
  const kpis = useMemo(() => {
    const totalVehiculos = vehiculos.length;
    const disponibles = vehiculos.filter((v) => String(v.estado).toUpperCase() === 'DISPONIBLE').length;
    const vendidos = vehiculos.filter((v) => String(v.estado).toUpperCase() === 'VENDIDO').length;
    const totalVentas = ventas.length;
    const completadas = ventas.filter((v) => String(v.estado).toUpperCase() === 'COMPLETADA');
    const montoTotal = completadas.reduce((acc, v) => {
      const val = Number(v.precioAcordado);
      return Number.isFinite(val) ? acc + val : acc;
    }, 0);

    return {
      totalVehiculos,
      disponibles,
      vendidos,
      totalVentas,
      ventasCompletadas: completadas.length,
      montoTotal,
    };
  }, [vehiculos, ventas]);

  return (
    <div className="resumen-grid">
      <div className="kpi-card">
        <span className="kpi-label">Vehículos en catálogo</span>
        <strong className="kpi-valor">{kpis.totalVehiculos}</strong>
        <span className="kpi-sub">{kpis.disponibles} disponibles · {kpis.vendidos} vendidos</span>
      </div>
      <div className="kpi-card">
        <span className="kpi-label">Ventas consultadas</span>
        <strong className="kpi-valor">{kpis.totalVentas}</strong>
        <span className="kpi-sub">{kpis.ventasCompletadas} completadas</span>
      </div>
      <div className="kpi-card">
        <span className="kpi-label">Monto vendido (real)</span>
        <strong className="kpi-valor">{mostrarPrecio(kpis.montoTotal)}</strong>
        <span className="kpi-sub">Calculado desde FastAPI</span>
      </div>
    </div>
  );
}

function TablaVehiculos({ vehiculos, cargando, consultado, hayFiltro, onSeleccionar }) {
  return (
    <div className="tabla-scroll">
      <table>
        <thead>
          <tr>
            <th scope="col">ID</th>
            <th scope="col">Marca</th>
            <th scope="col">Modelo</th>
            <th scope="col">Placa</th>
            <th scope="col">Año</th>
            <th scope="col">Precio</th>
            <th scope="col">Estado</th>
          </tr>
        </thead>
        <tbody>
          {vehiculos.map((vehiculo, indice) => (
            <tr key={vehiculo.idVehiculo ?? vehiculo.id ?? indice}>
              <td>
                <button
                  className="enlace-id"
                  type="button"
                  onClick={() => onSeleccionar(vehiculo)}
                  aria-label={`Ver detalle del vehículo ${vehiculo.idVehiculo ?? vehiculo.id ?? ''}`}
                >
                  {vehiculo.idVehiculo ?? vehiculo.id ?? '—'}
                </button>
              </td>
              <td>{vehiculo.marca ?? '—'}</td>
              <td>{vehiculo.modelo ?? '—'}</td>
              <td>{vehiculo.placa ?? '—'}</td>
              <td>{vehiculo.anio ?? '—'}</td>
              <td>{mostrarPrecio(vehiculo.precio)}</td>
              <td>
                <span className={claseBadge(vehiculo.estado)}>{vehiculo.estado ?? '—'}</span>
              </td>
            </tr>
          ))}

          {vehiculos.length === 0 && (
            <tr>
              <td className="sin-datos" colSpan="7">
                {cargando
                  ? 'Cargando catálogo de vehículos desde Spring Boot...'
                  : hayFiltro
                    ? 'No hay vehículos que coincidan con la marca ingresada.'
                    : consultado
                      ? 'HTTP 200: El catálogo se encuentra vacío.'
                      : 'Aún no hay vehículos para mostrar. Inicie sesión o presione Consultar catálogo y ventas.'}
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

function DetalleVehiculo({ seleccionado }) {
  if (!seleccionado) {
    return <p className="ayuda">Seleccione el ID de un vehículo en la tabla para visualizar su ficha detallada.</p>;
  }

  return (
    <dl className="detalle">
      <div><dt>ID</dt><dd>{seleccionado.idVehiculo ?? seleccionado.id ?? '—'}</dd></div>
      <div><dt>Marca</dt><dd>{seleccionado.marca ?? '—'}</dd></div>
      <div><dt>Modelo</dt><dd>{seleccionado.modelo ?? '—'}</dd></div>
      <div><dt>Placa</dt><dd>{seleccionado.placa ?? '—'}</dd></div>
      <div><dt>Chasis</dt><dd>{seleccionado.chasis ?? '—'}</dd></div>
      <div><dt>Año</dt><dd>{seleccionado.anio ?? '—'}</dd></div>
      <div><dt>Color</dt><dd>{seleccionado.color ?? '—'}</dd></div>
      <div><dt>Precio</dt><dd>{mostrarPrecio(seleccionado.precio)}</dd></div>
      <div>
        <dt>Estado</dt>
        <dd><span className={claseBadge(seleccionado.estado)}>{seleccionado.estado ?? '—'}</span></dd>
      </div>
    </dl>
  );
}

function TablaVentas({ ventas, cargando, consultado }) {
  return (
    <div className="tabla-scroll">
      <table>
        <thead>
          <tr>
            <th scope="col">Venta ID</th>
            <th scope="col">Vehículo ID</th>
            <th scope="col">Comprador ID</th>
            <th scope="col">Precio Acordado</th>
            <th scope="col">Estado</th>
            <th scope="col">Notificación</th>
            <th scope="col">Fecha UTC</th>
            <th scope="col">Operación ID</th>
          </tr>
        </thead>
        <tbody>
          {ventas.map((venta, idx) => (
            <tr key={venta.id ?? idx}>
              <td><strong>#{venta.id ?? '—'}</strong></td>
              <td>{venta.vehiculoId ?? '—'}</td>
              <td>{venta.compradorId ?? '—'}</td>
              <td>{mostrarPrecio(venta.precioAcordado)} {venta.moneda ?? ''}</td>
              <td><span className={claseBadge(venta.estado)}>{venta.estado ?? '—'}</span></td>
              <td>{venta.notificacion ?? '—'}</td>
              <td>{venta.fechaUTC ?? '—'}</td>
              <td className="mono">{venta.operacionId ?? '—'}</td>
            </tr>
          ))}

          {ventas.length === 0 && (
            <tr>
              <td className="sin-datos" colSpan="8">
                {cargando
                  ? 'Cargando ventas desde FastAPI...'
                  : consultado
                    ? 'HTTP 200: No existen ventas registradas.'
                    : 'Aún no se han consultado ventas.'}
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}

export default function App() {
  const [usuario, setUsuario] = useState('');
  const [password, setPassword] = useState('');
  const [usuarioActivo, setUsuarioActivo] = useState('');
  const [token, setToken] = useState('');

  const [vehiculos, setVehiculos] = useState([]);
  const [ventas, setVentas] = useState([]);
  const [marca, setMarca] = useState('');
  const [seleccionado, setSeleccionado] = useState(null);

  const [cargando, setCargando] = useState(false);
  const [consultadoVeh, setConsultadoVeh] = useState(false);
  const [consultadoVen, setConsultadoVen] = useState(false);

  const [mensaje, setMensaje] = useState('Inicie sesión para consultar vehículos y ventas vía NGINX (:8090).');
  const [errorVeh, setErrorVeh] = useState('');
  const [errorVen, setErrorVen] = useState('');
  const [errorAuth, setErrorAuth] = useState('');
  const [codigoHttp, setCodigoHttp] = useState(null);

  async function cargarVehiculosInterno(jwtActual) {
    setErrorVeh('');
    try {
      const res = await listarVehiculos(jwtActual);
      setVehiculos(res.vehiculos);
      setConsultadoVeh(true);
      setCodigoHttp(res.codigoHttp);
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
      setErrorAuth('Ingrese usuario y contraseña para autenticar su consulta.');
      return;
    }
    setCargando(true);
    setErrorAuth('');
    setErrorVeh('');
    setErrorVen('');
    setMensaje('Autenticando vía POST /oauth/token (proxy Vite -> NGINX :8090)...');
    try {
      const res = await iniciarSesionWeb(usuario, password);
      setToken(res.accessToken);
      setUsuarioActivo(usuario.trim());
      setPassword('');
      setCodigoHttp(res.codigoHttp);
      setMensaje(`Sesión activa como '${usuario.trim()}'. Cargando catálogo y ventas...`);

      const [rVeh, rVen] = await Promise.all([
        cargarVehiculosInterno(res.accessToken),
        cargarVentasInterno(res.accessToken),
      ]);
      if (rVeh || rVen) {
        setMensaje(
          `Consulta completada: ${rVeh?.vehiculos.length ?? 0} vehículo(s) y ${rVen?.ventas.length ?? 0} venta(s).`,
        );
      }
    } catch (e) {
      setToken('');
      setUsuarioActivo('');
      setCodigoHttp(e.status ?? null);
      setErrorAuth(e.message);
      setMensaje('No se pudo iniciar sesión.');
    } finally {
      setCargando(false);
    }
  }

  async function consultarTodo() {
    setCargando(true);
    setErrorAuth('');
    setSeleccionado(null);
    setMensaje('Consultando GET /api/vehiculos y GET /api/ventas vía NGINX (:8090)...');
    const [rVeh, rVen] = await Promise.all([
      cargarVehiculosInterno(token),
      cargarVentasInterno(token),
    ]);
    if (rVeh && rVen) {
      setMensaje(`HTTP 200: ${rVeh.vehiculos.length} vehículo(s) y ${rVen.ventas.length} venta(s) actualizados.`);
    } else if (rVeh || rVen) {
      setMensaje('Consulta parcial completada (revise el estado de cada servicio).');
    } else {
      setMensaje('No se pudo completar la consulta.');
    }
    setCargando(false);
  }

  async function consultarSoloVehiculos() {
    setCargando(true);
    setErrorAuth('');
    setSeleccionado(null);
    const rVeh = await cargarVehiculosInterno(token);
    if (rVeh) setMensaje(`HTTP ${rVeh.codigoHttp}: ${rVeh.vehiculos.length} vehículo(s) cargados.`);
    setCargando(false);
  }

  async function consultarSoloVentas() {
    setCargando(true);
    setErrorAuth('');
    const rVen = await cargarVentasInterno(token);
    if (rVen) {
      setCodigoHttp(rVen.codigoHttp);
      setMensaje(`HTTP ${rVen.codigoHttp}: ${rVen.ventas.length} venta(s) consultadas desde FastAPI.`);
    }
    setCargando(false);
  }

  function simularTokenInvalido() {
    setToken('jwt_invalido_react_prueba_401');
    setUsuarioActivo('prueba-401');
    setErrorAuth('');
    setMensaje('Token inválido cargado en memoria. Presione Consultar para comprobar HTTP 401.');
  }

  function cerrarSesion() {
    setToken('');
    setUsuarioActivo('');
    setVehiculos([]);
    setVentas([]);
    setSeleccionado(null);
    setConsultadoVeh(false);
    setConsultadoVen(false);
    setErrorAuth('');
    setErrorVeh('');
    setErrorVen('');
    setCodigoHttp(null);
    setMensaje('Sesión cerrada y token eliminado de memoria.');
  }

  const visibles = useMemo(() => {
    const texto = marca.trim().toLowerCase();
    if (!texto) return vehiculos;
    return vehiculos.filter((vehiculo) =>
      String(vehiculo.marca ?? '').toLowerCase().includes(texto),
    );
  }, [vehiculos, marca]);

  return (
    <main className="contenedor">
      <header className="cabecera">
        <div className="marca-producto">APE 04 · PLATAFORMA WEB DE CONSULTA (REACT + FETCH)</div>
        <h1>Catálogo de Vehículos y Seguimiento de Ventas</h1>
        <p>
          Cliente web de solo consulta conectado mediante proxy Vite hacia NGINX (<code>127.0.0.1:8090</code>).
          El registro de ventas es exclusivo de la PWA móvil.
        </p>
      </header>

      {/* SECCION 1: AUTENTICACION Y CONEXION */}
      <section className="panel" aria-labelledby="titulo-acceso">
        <div className="titulo-listado">
          <div>
            <h2 id="titulo-acceso">1. Sesión y conexión vía NGINX (:8090)</h2>
            <p className="ayuda">
              Autentique con su cuenta (por ejemplo <code>consulta01</code>, <code>operador01</code> o <code>admin01</code>).
              El JWT se guarda únicamente en memoria de React sin exponer claves en el navegador.
            </p>
          </div>
          <span className={usuarioActivo ? 'contador contador-activo' : 'contador'}>
            {usuarioActivo ? `Sesión: ${usuarioActivo}` : 'Sin sesión'}
          </span>
        </div>

        <form onSubmit={manejarLogin}>
          <div className="grid-login">
            <div>
              <label htmlFor="usuario">Usuario</label>
              <input
                id="usuario"
                type="text"
                autoComplete="username"
                placeholder="Ej. consulta01"
                value={usuario}
                onChange={(e) => setUsuario(e.target.value)}
                disabled={cargando}
              />
            </div>
            <div>
              <label htmlFor="password">Contraseña</label>
              <input
                id="password"
                type="password"
                autoComplete="current-password"
                placeholder="Contraseña"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={cargando}
              />
            </div>
          </div>

          <div className="acciones">
            <button type="submit" disabled={cargando}>
              {cargando ? 'Procesando...' : 'Iniciar sesión y consultar'}
            </button>
            <button type="button" className="secundario" disabled={cargando} onClick={consultarTodo}>
              Consultar catálogo y ventas
            </button>
            <button type="button" className="secundario" disabled={cargando} onClick={consultarSoloVehiculos}>
              Solo vehículos
            </button>
            <button type="button" className="secundario" disabled={cargando} onClick={consultarSoloVentas}>
              Solo ventas
            </button>
            <button type="button" className="secundario" disabled={cargando} onClick={simularTokenInvalido}>
              Probar JWT inválido (401)
            </button>
            <button type="button" className="secundario" disabled={cargando} onClick={cerrarSesion}>
              Cerrar sesión
            </button>
          </div>
        </form>

        <div
          className={errorAuth ? 'estado estado-error' : 'estado estado-ok'}
          role="status"
          aria-live="polite"
        >
          <span>{errorAuth || mensaje}</span>
          {codigoHttp !== null && <strong>HTTP {codigoHttp}</strong>}
        </div>
        <p className="nota">El JWT permanece solo en memoria; no se almacena en localStorage ni en el código fuente.</p>
      </section>

      {/* SECCION 2: RESUMEN DE INDICADORES REALES */}
      <section className="panel" aria-labelledby="titulo-resumen">
        <h2 id="titulo-resumen">2. Resumen con indicadores reales</h2>
        <ResumenIndicadores vehiculos={vehiculos} ventas={ventas} />
      </section>

      {/* SECCION 3: CATALOGO DE VEHICULOS + FILTRO POR MARCA */}
      <section className="panel" aria-labelledby="titulo-listado">
        <div className="titulo-listado">
          <div>
            <h2 id="titulo-listado">3. Catálogo de vehículos (GET /api/vehiculos)</h2>
            <p className="ayuda">Listado real desde Spring Boot vía NGINX con filtrado reactivo por marca.</p>
          </div>
          <span className="contador">{visibles.length} visibles</span>
        </div>

        {errorVeh && <div className="estado estado-error" role="alert"><span>{errorVeh}</span></div>}

        <label htmlFor="filtroMarca">Filtrar por marca</label>
        <input
          id="filtroMarca"
          type="search"
          placeholder="Ejemplo: Toyota, Mazda, Chevrolet..."
          value={marca}
          onChange={(evento) => setMarca(evento.target.value)}
          disabled={cargando}
        />

        <TablaVehiculos
          vehiculos={visibles}
          cargando={cargando}
          consultado={consultadoVeh}
          hayFiltro={marca.trim().length > 0}
          onSeleccionar={setSeleccionado}
        />
      </section>

      {/* SECCION 4: DETALLE DEL VEHICULO */}
      <section className="panel" aria-labelledby="titulo-detalle">
        <h2 id="titulo-detalle">4. Detalle del vehículo seleccionado</h2>
        <DetalleVehiculo seleccionado={seleccionado} />
      </section>

      {/* SECCION 5: CONSULTA DE VENTAS (SOLO LECTURA) */}
      <section className="panel" aria-labelledby="titulo-ventas">
        <div className="titulo-listado">
          <div>
            <h2 id="titulo-ventas">5. Consulta y seguimiento de ventas (GET /api/ventas)</h2>
            <p className="ayuda">
              Historial real desde FastAPI vía NGINX. <strong>Solo lectura:</strong> esta plataforma web no permite registrar ventas.
            </p>
          </div>
          <span className="contador">{ventas.length} venta(s)</span>
        </div>

        {errorVen && <div className="estado estado-error" role="alert"><span>{errorVen}</span></div>}

        <TablaVentas ventas={ventas} cargando={cargando} consultado={consultadoVen} />
      </section>
    </main>
  );
}
