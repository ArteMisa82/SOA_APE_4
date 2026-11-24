import { AlertTriangle, Car, Loader2, Search } from 'lucide-react';
import { claseEstadoVehiculo, mostrarPrecio, obtenerFotoVehiculo } from '../../utils/formatters.js';
import './Vehiculos.css';

export default function TarjetasVehiculos({
  vehiculos,
  cargando,
  consultado,
  errorVeh,
  hayFiltro,
  seleccionado,
  onSeleccionar,
}) {
  const idSel = seleccionado ? (seleccionado.idVehiculo ?? seleccionado.id) : null;

  if (vehiculos.length === 0) {
    return (
      <div className="empty-box" style={{ padding: 40 }}>
        {cargando ? (
          <>
            <Loader2 size={24} className="spin empty-icon-color" />
            <strong>Cargando tarjetas del inventario…</strong>
            <span>Obteniendo detalles visuales de la flota</span>
          </>
        ) : errorVeh ? (
          <>
            <AlertTriangle size={24} color="var(--sys-error-fg)" />
            <strong>No se pudo cargar el catálogo</strong>
            <span>{errorVeh}</span>
          </>
        ) : hayFiltro ? (
          <>
            <Search size={24} className="empty-icon-color" />
            <strong>Sin resultados para los filtros</strong>
            <span>Pruebe con otra marca, modelo o cambie el estado de búsqueda.</span>
          </>
        ) : consultado ? (
          <>
            <Car size={24} className="empty-icon-color" />
            <strong>HTTP 200: Catálogo vacío.</strong>
          </>
        ) : (
          <span>Sin datos cargados todavía.</span>
        )}
      </div>
    );
  }

  return (
    <div className="cards-grid">
      {vehiculos.map((v, idx) => {
        const id = v.idVehiculo ?? v.id ?? idx;
        const esVendido = String(v.estado).toUpperCase() === 'VENDIDO';
        const esSel = idSel !== null && idSel === (v.idVehiculo ?? v.id);

        return (
          <article
            key={id}
            className={`vehicle-card ${esSel ? 'selected' : ''} ${esVendido ? 'sold' : ''}`}
            onClick={() => onSeleccionar(v)}
          >
            <div
              className="vehicle-card-media"
              style={{ backgroundImage: `url(${obtenerFotoVehiculo(v)})` }}
            >
              <span className="plate-tag-dark">{v.placa ?? `ID #${id}`}</span>
              <span className={claseEstadoVehiculo(v.estado)}>
                <span className="dot pulse-dot" />
                {v.estado ?? '—'}
              </span>
            </div>
            <div className="vehicle-card-body">
              <h4 className="vehicle-card-title">{v.marca ?? '—'} {v.modelo ?? ''}</h4>
              <div className="vehicle-card-tags">
                <span className="v-tag tabular">{v.anio ?? '—'}</span>
                <span className="v-tag">{v.color ?? 'Sin color'}</span>
                <span className="v-tag tabular">ID #{v.idVehiculo ?? v.id ?? '—'}</span>
              </div>
              <div className="vehicle-card-foot">
                <div>
                  <span className="price-label">Precio sugerido</span>
                  <strong className={`vehicle-card-price tabular ${esVendido ? 'price-sold' : ''}`}>
                    {mostrarPrecio(v.precio)}
                  </strong>
                </div>
                <button
                  type="button"
                  className={`btn-select-row ${esSel ? 'selected' : ''}`}
                  onClick={(e) => {
                    e.stopPropagation();
                    onSeleccionar(v);
                  }}
                >
                  {esSel ? 'Seleccionado' : 'Ver ficha'}
                </button>
              </div>
            </div>
          </article>
        );
      })}
    </div>
  );
}
