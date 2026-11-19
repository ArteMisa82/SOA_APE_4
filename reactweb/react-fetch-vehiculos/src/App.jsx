import { useMemo, useState } from 'react';
import { listarVehiculos } from './api/vehiculosApi.js';
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

function TablaVehiculos({ vehiculos, cargando, onSeleccionar }) {
  return (
    <div className="tabla-scroll">
      <table>
        <thead>
          <tr>
            <th scope="col">ID</th>
            <th scope="col">Marca</th>
            <th scope="col">Modelo</th>
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
              <td>{mostrarPrecio(vehiculo.precio)}</td>
              <td>{vehiculo.estado ?? '—'}</td>
            </tr>
          ))}

          {vehiculos.length === 0 && (
            <tr>
              <td className="sin-datos" colSpan="5">
                {cargando
                  ? 'Cargando vehículos...'
                  : 'Aún no hay vehículos para mostrar. Presione Consultar vehículos.'}
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
    return <p className="ayuda">Después de consultar, seleccione el ID de una fila para ver sus datos.</p>;
  }

  return (
    <dl className="detalle">
      <div>
        <dt>ID</dt>
        <dd>{seleccionado.idVehiculo ?? seleccionado.id ?? '—'}</dd>
      </div>
      <div>
        <dt>Marca</dt>
        <dd>{seleccionado.marca ?? '—'}</dd>
      </div>
      <div>
        <dt>Modelo</dt>
        <dd>{seleccionado.modelo ?? '—'}</dd>
      </div>
      <div>
        <dt>Precio</dt>
        <dd>{mostrarPrecio(seleccionado.precio)}</dd>
      </div>
      <div>
        <dt>Estado</dt>
        <dd>{seleccionado.estado ?? '—'}</dd>
      </div>
    </dl>
  );
}

export default function App() {
  const [token, setToken] = useState('');
  const [mostrarToken, setMostrarToken] = useState(false);
  const [vehiculos, setVehiculos] = useState([]);
  const [marca, setMarca] = useState('');
  const [seleccionado, setSeleccionado] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [mensaje, setMensaje] = useState('Listo para consultar.');
  const [error, setError] = useState('');
  const [codigoHttp, setCodigoHttp] = useState(null);

  async function consultarVehiculos(evento) {
    evento.preventDefault();
    setCargando(true);
    setError('');
    setMensaje('Consultando la API de vehículos...');
    setCodigoHttp(null);
    setVehiculos([]);
    setSeleccionado(null);

    try {
      const resultado = await listarVehiculos(token);
      setVehiculos(resultado.vehiculos);
      setCodigoHttp(resultado.codigoHttp);
      setMensaje(`Consulta correcta: ${resultado.vehiculos.length} vehículos.`);
    } catch (e) {
      setCodigoHttp(e.status ?? null);
      setError(e.message);
      setMensaje('La consulta no pudo completarse.');
    } finally {
      setCargando(false);
    }
  }

  const visibles = useMemo(() => {
    const texto = marca.trim().toLowerCase();

    if (!texto) {
      return vehiculos;
    }

    return vehiculos.filter((vehiculo) =>
      String(vehiculo.marca ?? '').toLowerCase().includes(texto),
    );
  }, [vehiculos, marca]);

  return (
    <main className="contenedor">
      <header className="cabecera">
        <div className="marca-producto">PRÁCTICA WEB · REACT + FETCH</div>
        <h1>Consulta de vehículos</h1>
        <p>Cliente web de la API REST existente en Java Spring Boot.</p>
      </header>

      <section className="panel" aria-labelledby="titulo-acceso">
        <h2 id="titulo-acceso">1. Conexión con la API</h2>
        <p className="ayuda">
          La aplicación consulta <code>GET /api/vehiculos</code> mediante el
          proxy local hacia <code>127.0.0.1:8080</code>.
        </p>

        <form onSubmit={consultarVehiculos}>
          <label htmlFor="jwt">Token JWT de Postman (sin escribir Bearer)</label>
          <div className="linea-token">
            <input
              id="jwt"
              type={mostrarToken ? 'text' : 'password'}
              autoComplete="off"
              spellCheck="false"
              placeholder="Pegar token JWT obtenido previamente"
              value={token}
              onChange={(evento) => setToken(evento.target.value)}
            />
            <button
              className="secundario"
              type="button"
              onClick={() => setMostrarToken((actual) => !actual)}
              aria-label={mostrarToken ? 'Ocultar token' : 'Mostrar token'}
            >
              {mostrarToken ? 'Ocultar' : 'Mostrar'}
            </button>
          </div>

          <div className="acciones">
            <button type="submit" disabled={cargando}>
              {cargando ? 'Consultando...' : 'Consultar vehículos'}
            </button>
            <button
              className="secundario"
              type="button"
              disabled={cargando}
              onClick={() => {
                setToken('');
                setMostrarToken(false);
              }}
            >
              Limpiar token
            </button>
          </div>
        </form>

        <div
          className={error ? 'estado estado-error' : 'estado estado-ok'}
          role="status"
          aria-live="polite"
        >
          <span>{error || mensaje}</span>
          {codigoHttp !== null && <strong>HTTP {codigoHttp}</strong>}
        </div>
        <p className="nota">El JWT permanece en memoria: no se guarda en localStorage ni en archivos.</p>
      </section>

      <section className="panel" aria-labelledby="titulo-listado">
        <div className="titulo-listado">
          <div>
            <h2 id="titulo-listado">2. Catálogo de vehículos</h2>
            <p className="ayuda">Datos recibidos del backend; filtro ejecutado en el navegador.</p>
          </div>
          <span className="contador">{visibles.length} visibles</span>
        </div>

        <label htmlFor="filtroMarca">Filtrar por marca</label>
        <input
          id="filtroMarca"
          type="search"
          placeholder="Ejemplo: Toyota"
          value={marca}
          onChange={(evento) => setMarca(evento.target.value)}
          disabled={cargando}
        />

        <TablaVehiculos
          vehiculos={visibles}
          cargando={cargando}
          onSeleccionar={setSeleccionado}
        />
      </section>

      <section className="panel" aria-labelledby="titulo-detalle">
        <h2 id="titulo-detalle">3. Detalle del vehículo</h2>
        <DetalleVehiculo seleccionado={seleccionado} />
      </section>
    </main>
  );
}

