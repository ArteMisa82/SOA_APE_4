import { AlertTriangle, Loader2, Receipt, Search } from 'lucide-react';
import { claseEstadoVenta, formatoFechaCorta, mostrarPrecio } from '../../utils/formatters.js';
import './Ventas.css';

export default function TablaVentas({ ventas, cargando, consultado, errorVen, hayFiltro }) {
  return (
    <div className="table-scroll">
      <table className="table-sales">
        <thead>
          <tr>
            <th scope="col" style={{ width: 70 }}>Venta</th>
            <th scope="col">Vehículo</th>
            <th scope="col">Comprador</th>
            <th scope="col">Precio acordado</th>
            <th scope="col">Estado</th>
            <th scope="col">Notificación</th>
            <th scope="col">Fecha (UTC)</th>
            <th scope="col">Operación ID</th>
          </tr>
        </thead>
        <tbody>
          {ventas.map((venta, idx) => (
            <tr key={venta.id ?? idx}>
              <td>
                <span className="id-pill tabular">#{venta.id ?? '—'}</span>
              </td>
              <td>
                <span className="tabular font-medium">Vehículo #{venta.vehiculoId ?? '—'}</span>
              </td>
              <td>
                <span className="tabular text-secondary">Comprador #{venta.compradorId ?? '—'}</span>
              </td>
              <td className="price-cell tabular">
                <strong>{mostrarPrecio(venta.precioAcordado)}</strong>{' '}
                <span className="moneda-code">{venta.moneda ?? 'USD'}</span>
              </td>
              <td>
                <span className={claseEstadoVenta(venta.estado)}>
                  <span className="dot pulse-dot" />
                  {venta.estado ?? '—'}
                </span>
              </td>
              <td>
                <span
                  className={
                    String(venta.notificacion).toUpperCase() === 'ENVIADA'
                      ? 'status-badge status-available'
                      : 'status-badge status-reserved'
                  }
                >
                  <span className="dot" />
                  {venta.notificacion ?? '—'}
                </span>
              </td>
              <td className="mono tabular">{formatoFechaCorta(venta.fechaUTC)}</td>
              <td className="mono" title={venta.operacionId}>
                <span className="operacion-pill">{venta.operacionId ?? '—'}</span>
              </td>
            </tr>
          ))}

          {ventas.length === 0 && (
            <tr>
              <td colSpan="8" style={{ padding: 32 }}>
                <div className="empty-box">
                  {cargando ? (
                    <>
                      <Loader2 size={24} className="spin empty-icon-color" />
                      <strong>Cargando historial de ventas…</strong>
                      <span>Consultando microservicio FastAPI vía proxy NGINX (:8090)</span>
                    </>
                  ) : errorVen ? (
                    <>
                      <AlertTriangle size={24} color="var(--sys-error-fg)" />
                      <strong>Error al consultar GET /api/ventas</strong>
                      <span>{errorVen}</span>
                    </>
                  ) : hayFiltro ? (
                    <>
                      <Search size={24} className="empty-icon-color" />
                      <strong>Sin coincidencias</strong>
                      <span>No se encontraron ventas para el filtro ingresado.</span>
                    </>
                  ) : consultado ? (
                    <>
                      <Receipt size={24} className="empty-icon-color" />
                      <strong>HTTP 200: Sin ventas registradas</strong>
                      <span>No existen transacciones comerciales registradas en FastAPI todavía.</span>
                    </>
                  ) : (
                    <>
                      <Receipt size={24} className="empty-icon-color" />
                      <span>Presione «Recargar ventas» para obtener el histórico.</span>
                    </>
                  )}
                </div>
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
