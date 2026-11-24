import { AlertTriangle, Car, Eye, Loader2, Search } from 'lucide-react';
import { claseEstadoVehiculo, mostrarPrecio } from '../../utils/formatters.js';
import './Vehiculos.css';

export default function TablaVehiculos({
  vehiculos,
  cargando,
  consultado,
  errorVeh,
  hayFiltro,
  seleccionado,
  onSeleccionar,
}) {
  const idSel = seleccionado ? (seleccionado.idVehiculo ?? seleccionado.id) : null;

  return (
    <div className="table-scroll">
      <table className="table-catalog">
        <thead>
          <tr>
            <th scope="col" style={{ width: 80 }}>ID</th>
            <th scope="col">Vehículo y Placa</th>
            <th scope="col" style={{ width: 100 }}>Año</th>
            <th scope="col" style={{ width: 140 }}>Precio</th>
            <th scope="col" style={{ width: 130 }}>Estado</th>
            <th scope="col" style={{ width: 110, textAlign: 'right' }}>Ficha</th>
          </tr>
        </thead>
        <tbody>
          {vehiculos.map((v, idx) => {
            const id = v.idVehiculo ?? v.id ?? idx;
            const esVendido = String(v.estado).toUpperCase() === 'VENDIDO';
            const esSel = idSel !== null && idSel === (v.idVehiculo ?? v.id);

            return (
              <tr
                key={id}
                className={`${esVendido ? 'row-sold' : ''} ${esSel ? 'row-selected' : ''}`}
                onClick={() => onSeleccionar(v)}
                style={{ cursor: 'pointer' }}
              >
                <td>
                  <span className="id-pill tabular">#{v.idVehiculo ?? v.id ?? '—'}</span>
                </td>
                <td>
                  <div className="veh-name-wrap">
                    <span className="veh-title-cell">{v.marca ?? '—'} {v.modelo ?? ''}</span>
                    <span className="veh-plate-cell">{v.placa ?? 'Sin placa'}</span>
                  </div>
                </td>
                <td className="tabular font-medium">{v.anio ?? '—'}</td>
                <td className={`price-cell tabular ${esVendido ? 'price-sold' : ''}`}>
                  {mostrarPrecio(v.precio)}
                </td>
                <td>
                  <span className={claseEstadoVehiculo(v.estado)}>
                    <span className="dot pulse-dot" />
                    {v.estado ?? '—'}
                  </span>
                </td>
                <td style={{ textAlign: 'right' }}>
                  <button
                    type="button"
                    className={`btn-select-row ${esSel ? 'selected' : ''}`}
                    onClick={(e) => {
                      e.stopPropagation();
                      onSeleccionar(v);
                    }}
                    aria-pressed={esSel}
                    aria-label={`Ver ficha técnica del vehículo ${v.marca ?? ''} ${v.modelo ?? ''}`}
                  >
                    {esSel ? (
                      <>
                        <Eye size={13} /> Viendo
                      </>
                    ) : (
                      'Ver ficha'
                    )}
                  </button>
                </td>
              </tr>
            );
          })}

          {vehiculos.length === 0 && (
            <tr>
              <td colSpan="6" style={{ padding: 32 }}>
                <div className="empty-box">
                  {cargando ? (
                    <>
                      <Loader2 size={24} className="spin empty-icon-color" />
                      <strong>Cargando catálogo automotriz…</strong>
                      <span>Sincronizando inventario desde Spring Boot vía NGINX (:8090)</span>
                    </>
                  ) : errorVeh ? (
                    <>
                      <AlertTriangle size={24} color="var(--sys-error-fg)" />
                      <strong>Error al consultar el catálogo</strong>
                      <span>{errorVeh}</span>
                    </>
                  ) : hayFiltro ? (
                    <>
                      <Search size={24} className="empty-icon-color" />
                      <strong>Sin coincidencias</strong>
                      <span>No se encontraron vehículos que coincidan con los filtros aplicados.</span>
                    </>
                  ) : consultado ? (
                    <>
                      <Car size={24} className="empty-icon-color" />
                      <strong>Catálogo vacío</strong>
                      <span>HTTP 200: El servicio no tiene vehículos registrados actualmente.</span>
                    </>
                  ) : (
                    <>
                      <Car size={24} className="empty-icon-color" />
                      <span>Presione «Actualizar inventario» para consultar los vehículos disponibles.</span>
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
