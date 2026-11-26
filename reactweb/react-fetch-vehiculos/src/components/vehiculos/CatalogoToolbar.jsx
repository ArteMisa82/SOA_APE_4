import { LayoutGrid, Search, Table2, X } from 'lucide-react';

export default function CatalogoToolbar({
  marca,
  filtroEstado,
  ordenVehiculos,
  modoVista,
  cargando,
  hayFiltroVeh,
  onChangeMarca,
  onChangeFiltroEstado,
  onChangeOrden,
  onChangeModoVista,
  onLimpiar,
}) {
  return (
    <section className="toolbar-card" aria-label="Búsqueda y filtros de inventario">
      <div className="search-box">
        <Search size={16} aria-hidden="true" />
        <input
          id="filtroMarca"
          type="search"
          maxLength={60}
          aria-label="Buscar vehículo por marca, modelo o placa"
          placeholder="Buscar por marca, modelo o placa (Toyota, Mazda, Chevrolet, etc.)…"
          value={marca}
          onChange={(e) => onChangeMarca(e.target.value)}
          disabled={cargando}
        />
        {marca && (
          <button
            type="button"
            className="btn-clear-input"
            onClick={() => onChangeMarca('')}
            aria-label="Limpiar campo de búsqueda"
          >
            <X size={14} />
          </button>
        )}
      </div>

      <div className="chips-group" role="group" aria-label="Filtrar por estado">
        {['TODOS', 'DISPONIBLE', 'RESERVADO', 'VENDIDO'].map((est) => (
          <button
            key={est}
            type="button"
            className={`chip ${filtroEstado === est ? 'active' : ''}`}
            onClick={() => onChangeFiltroEstado(est)}
          >
            {est === 'TODOS' ? 'Todos' : est.charAt(0) + est.slice(1).toLowerCase()}
          </button>
        ))}
      </div>

      <div className="filter-controls-right">
        <select
          className="sort-select"
          aria-label="Ordenar inventario"
          value={ordenVehiculos}
          onChange={(e) => onChangeOrden(e.target.value)}
        >
          <option value="id-desc">Orden: Más recientes</option>
          <option value="precio-asc">Precio: Menor a mayor</option>
          <option value="precio-desc">Precio: Mayor a menor</option>
          <option value="anio-desc">Año: Más nuevo</option>
          <option value="estado">Estado (A–Z)</option>
        </select>

        <div className="view-toggle" role="group" aria-label="Cambiar modo de vista">
          <button
            type="button"
            className={modoVista === 'tabla' ? 'active' : ''}
            onClick={() => onChangeModoVista('tabla')}
            title="Vista en tabla"
          >
            <Table2 size={15} /> <span>Tabla</span>
          </button>
          <button
            type="button"
            className={modoVista === 'tarjetas' ? 'active' : ''}
            onClick={() => onChangeModoVista('tarjetas')}
            title="Vista en tarjetas"
          >
            <LayoutGrid size={15} /> <span>Tarjetas</span>
          </button>
        </div>

        {hayFiltroVeh && (
          <button type="button" className="btn-secondary" onClick={onLimpiar}>
            <X size={14} /> Limpiar
          </button>
        )}
      </div>
    </section>
  );
}
