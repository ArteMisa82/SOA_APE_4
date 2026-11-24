import { Menu, RefreshCw, ShieldCheck } from 'lucide-react';
import './Layout.css';

export default function Topbar({ tituloSeccion, subSeccion, cargando, onToggleMenuMovil, onConsultarTodo }) {
  return (
    <header className="topbar">
      <div className="topbar-left">
        <button
          type="button"
          className="btn-mobile-menu"
          onClick={onToggleMenuMovil}
          aria-label="Abrir menú de navegación"
        >
          <Menu size={20} />
        </button>
        <div>
          <div className="topbar-breadcrumb">
            <span>NEXORA</span> / <span className="crumb-active">{tituloSeccion}</span>
          </div>
          <h1 className="topbar-title">{tituloSeccion}</h1>
          <p className="topbar-sub">{subSeccion}</p>
        </div>
      </div>

      <div className="topbar-right">
        <span className="readonly-pill">
          <ShieldCheck size={14} /> Solo consulta · Ventas en PWA
        </span>
        <button
          type="button"
          className="btn-copper"
          disabled={cargando}
          onClick={onConsultarTodo}
        >
          <RefreshCw size={15} className={cargando ? 'spin' : ''} />
          {cargando ? 'Sincronizando…' : 'Actualizar inventario'}
        </button>
      </div>
    </header>
  );
}
