import { ChevronLeft, ChevronRight } from 'lucide-react';
import './Pagination.css';

function construirPaginas(paginaActual, totalPaginas) {
  if (totalPaginas <= 7) {
    return Array.from({ length: totalPaginas }, (_, i) => i + 1);
  }

  const paginas = [1];
  const inicio = Math.max(2, paginaActual - 1);
  const fin = Math.min(totalPaginas - 1, paginaActual + 1);

  if (inicio > 2) paginas.push('ellipsis-start');
  for (let p = inicio; p <= fin; p += 1) {
    paginas.push(p);
  }
  if (fin < totalPaginas - 1) paginas.push('ellipsis-end');

  paginas.push(totalPaginas);
  return paginas;
}

export default function Pagination({
  paginaActual,
  totalItems,
  porPagina,
  opcionesPorPagina = [6, 10, 15, 25],
  etiquetaItems = 'registros',
  onCambiarPagina,
  onCambiarPorPagina,
}) {
  if (totalItems === 0) return null;

  const totalPaginas = Math.max(1, Math.ceil(totalItems / porPagina));
  const paginaSegura = Math.min(Math.max(1, paginaActual), totalPaginas);
  const desde = (paginaSegura - 1) * porPagina + 1;
  const hasta = Math.min(paginaSegura * porPagina, totalItems);
  const paginas = construirPaginas(paginaSegura, totalPaginas);

  return (
    <nav className="pagination-bar" aria-label={`Paginación de ${etiquetaItems}`}>
      <div className="pagination-info">
        <span className="pagination-summary tabular">
          Mostrando <strong>{desde}–{hasta}</strong> de <strong>{totalItems}</strong> {etiquetaItems}
        </span>

        <label className="pagination-size">
          <span>Por página:</span>
          <select
            className="pagination-select"
            value={porPagina}
            onChange={(e) => onCambiarPorPagina(Number(e.target.value))}
            aria-label={`Cantidad de ${etiquetaItems} por página`}
          >
            {opcionesPorPagina.map((opt) => (
              <option key={opt} value={opt}>
                {opt}
              </option>
            ))}
          </select>
        </label>
      </div>

      <div className="pagination-controls">
        <button
          type="button"
          className="pagination-btn pagination-nav"
          disabled={paginaSegura <= 1}
          onClick={() => onCambiarPagina(paginaSegura - 1)}
          aria-label="Página anterior"
        >
          <ChevronLeft size={15} />
          <span>Anterior</span>
        </button>

        <div className="pagination-pages">
          {paginas.map((item) =>
            typeof item === 'string' ? (
              <span key={item} className="pagination-ellipsis" aria-hidden="true">
                …
              </span>
            ) : (
              <button
                key={item}
                type="button"
                className={`pagination-btn pagination-num tabular ${item === paginaSegura ? 'active' : ''}`}
                onClick={() => onCambiarPagina(item)}
                aria-current={item === paginaSegura ? 'page' : undefined}
                aria-label={`Ir a la página ${item}`}
              >
                {item}
              </button>
            ),
          )}
        </div>

        <button
          type="button"
          className="pagination-btn pagination-nav"
          disabled={paginaSegura >= totalPaginas}
          onClick={() => onCambiarPagina(paginaSegura + 1)}
          aria-label="Página siguiente"
        >
          <span>Siguiente</span>
          <ChevronRight size={15} />
        </button>
      </div>
    </nav>
  );
}
