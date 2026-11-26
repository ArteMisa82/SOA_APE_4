import { Car, LayoutDashboard, LogOut, Receipt, Wifi, WifiOff } from 'lucide-react';
import nexoraLogo from '../../assets/nexora-logo.png';
import './Layout.css';

export default function Sidebar({
  seccion,
  vehiculosCount,
  ventasCount,
  tieneError,
  usuarioActivo,
  ultimaSync,
  expiraSeg,
  cargando,
  menuMovilAbierto,
  onNavegar,
  onCerrarSesion,
  onCerrarMenuMovil,
}) {
  return (
    <>
      <div
        className={`sidebar-backdrop ${menuMovilAbierto ? 'open' : ''}`}
        onClick={onCerrarMenuMovil}
      />

      <aside className={`sidebar ${menuMovilAbierto ? 'open' : ''}`} aria-label="Navegación principal">
        <div className="sidebar-brand">
          <div className="sidebar-logo-container">
            <img src={nexoraLogo} alt="NEXORA Autos" className="sidebar-logo-img" />
          </div>
          <span className="sidebar-badge-pro">ENTERPRISE</span>
        </div>

        <nav className="sidebar-nav">
          <span className="sidebar-section-label">Navegación</span>

          <button
            type="button"
            className={`sidebar-link ${seccion === 'dashboard' ? 'active' : ''}`}
            onClick={() => onNavegar('dashboard')}
          >
            <span className="sidebar-link-left">
              <LayoutDashboard size={18} />
              <span>Panel de control</span>
            </span>
          </button>

          <button
            type="button"
            className={`sidebar-link ${seccion === 'catalogo' ? 'active' : ''}`}
            onClick={() => onNavegar('catalogo')}
          >
            <span className="sidebar-link-left">
              <Car size={18} />
              <span>Catálogo</span>
            </span>
            <span className="sidebar-count tabular">{vehiculosCount}</span>
          </button>

          <button
            type="button"
            className={`sidebar-link ${seccion === 'ventas' ? 'active' : ''}`}
            onClick={() => onNavegar('ventas')}
          >
            <span className="sidebar-link-left">
              <Receipt size={18} />
              <span>Seguimiento ventas</span>
            </span>
            <span className="sidebar-count tabular">{ventasCount}</span>
          </button>
        </nav>

        <div className="sidebar-footer">
          <div className={`gateway-status ${tieneError ? 'err' : 'ok'}`}>
            <span className="gateway-status-left">
              <span className={`dot ${tieneError ? '' : 'pulse-dot'}`} />
              {tieneError ? (
                <>
                  <WifiOff size={14} /> Error servicio
                </>
              ) : (
                <>
                  <Wifi size={14} /> NGINX en línea
                </>
              )}
            </span>
            <span className="gateway-port">:8090</span>
          </div>

          <div className="sidebar-user">
            <div className="user-avatar" aria-hidden="true">
              {usuarioActivo.slice(0, 2).toUpperCase()}
            </div>
            <div className="user-info">
              <span className="user-name">{usuarioActivo}</span>
              <span className="user-meta">
                {ultimaSync ? `Sync ${ultimaSync}` : expiraSeg ? `JWT · ${Math.round(expiraSeg / 60)} min` : 'Sesión activa'}
              </span>
            </div>
          </div>

          <div className="sidebar-actions">
            <button
              type="button"
              className="btn-dark-ghost"
              disabled={cargando}
              onClick={onCerrarSesion}
              title="Cerrar sesión y limpiar token"
            >
              <LogOut size={14} /> Cerrar sesión
            </button>
          </div>
        </div>
      </aside>
    </>
  );
}
