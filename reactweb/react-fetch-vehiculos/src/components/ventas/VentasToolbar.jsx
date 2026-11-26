import { RefreshCw, Search, X } from 'lucide-react';

export default function VentasToolbar({
  busquedaVenta,
  filtroEstadoVenta,
  cargando,
  hayFiltroVen,
  onChangeBusqueda,
  onChangeFiltroEstado,
  onRecargar,
  onLimpiar,
}) {
  return (
    <section className="toolbar-card" aria-label="Filtros de seguimiento de ventas">
      <div className="search-box">
        <Search size={16} aria-hidden="true" />
        <input
          type="search"
          maxLength={60}
          aria-label="Buscar ventas por ID, vehículo, comprador u operación"
          placeholder="Buscar por ID de venta, vehículo, comprador u operación…"
          value={busquedaVenta}
          onChange={(e) => onChangeBusqueda(e.target.value)}
        />
        {busquedaVenta && (
          <button
            type="button"
            className="btn-clear-input"
            onClick={() => onChangeBusqueda('')}
            aria-label="Limpiar búsqueda de ventas"
          >
            <X size={14} />
          </button>
        )}
      </div>

      <div className="chips-group" role="group" aria-label="Filtrar ventas por estado">
        {[
          { id: 'TODAS', label: 'Todas' },
          { id: 'COMPLETADA', label: 'Completadas' },
          { id: 'PENDIENTE', label: 'Pendientes' },
          { id: 'FALLIDA', label: 'Fallidas' },
        ].map((item) => (
          <button
            key={item.id}
            type="button"
            className={`chip ${filtroEstadoVenta === item.id ? 'active' : ''}`}
            onClick={() => onChangeFiltroEstado(item.id)}
          >
            {item.label}
          </button>
        ))}
      </div>

      <div className="filter-controls-right">
        <button type="button" className="btn-secondary" disabled={cargando} onClick={onRecargar}>
          <RefreshCw size={14} className={cargando ? 'spin' : ''} /> Recargar ventas
        </button>

        {hayFiltroVen && (
          <button type="button" className="btn-secondary" onClick={onLimpiar}>
            <X size={14} /> Limpiar
          </button>
        )}
      </div>
    </section>
  );
}
