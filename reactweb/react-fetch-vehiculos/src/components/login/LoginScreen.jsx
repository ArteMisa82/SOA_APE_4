import { AlertTriangle, Eye, EyeOff, Loader2, Lock, User } from 'lucide-react';
import nexoraLogo from '../../assets/nexora-logo.png';
import loginCarsImg from '../../assets/login-cars.jpg';
import './LoginScreen.css';

export default function LoginScreen({
  usuario,
  password,
  mostrarPassword,
  cargando,
  errorAuth,
  codigoHttp,
  onChangeUsuario,
  onChangePassword,
  onTogglePassword,
  onSubmit,
}) {
  return (
    <div className="luxury-login-page">
      {/* PANEL IZQUIERDO: FONDO OSCURO + LOGO + TÍTULOS + FORMULARIO + FOOTER */}
      <div className="luxury-login-main">
        <header className="luxury-login-topbar">
          <span className="luxury-sys-badge">
            <span className="dot pulse-dot" style={{ background: '#10B981' }} />
            NGINX :8090 · Solo consulta
          </span>
        </header>

        <main className="luxury-login-center">
          <div className="luxury-login-box fade-in">
            <div className="luxury-logo-stack">
              <img src={nexoraLogo} alt="NEXORA Autos" className="luxury-logo-hero" />
            </div>

            <div className="luxury-titles">
              <span className="luxury-brand-tag">NEXORA AUTOS · APE 04</span>
              <h1 className="luxury-login-title">Iniciar sesión</h1>
              <p className="luxury-login-sub">
                Acceda al panel de inventario, fichas técnicas y seguimiento comercial en tiempo real.
              </p>
            </div>

            <form className="luxury-login-form" onSubmit={onSubmit}>
              <div className="luxury-field">
                <label className="luxury-label" htmlFor="usuario">
                  Usuario
                </label>
                <div className="luxury-input-wrap">
                  <User size={16} className="luxury-input-icon" aria-hidden="true" />
                  <input
                    id="usuario"
                    className="luxury-input with-icon"
                    type="text"
                    maxLength={60}
                    autoComplete="username"
                    placeholder="Ej. consulta01"
                    value={usuario}
                    onChange={(e) => onChangeUsuario(e.target.value)}
                    disabled={cargando}
                    autoFocus
                  />
                </div>
              </div>

              <div className="luxury-field">
                <label className="luxury-label" htmlFor="password">
                  Contraseña
                </label>
                <div className="luxury-input-wrap">
                  <Lock size={16} className="luxury-input-icon" aria-hidden="true" />
                  <input
                    id="password"
                    className="luxury-input with-icon with-eye"
                    type={mostrarPassword ? 'text' : 'password'}
                    maxLength={100}
                    autoComplete="current-password"
                    placeholder="Ingrese su contraseña"
                    value={password}
                    onChange={(e) => onChangePassword(e.target.value)}
                    disabled={cargando}
                  />
                  <button
                    type="button"
                    className="luxury-eye-btn"
                    onClick={onTogglePassword}
                    aria-label={mostrarPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                  >
                    {mostrarPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                  </button>
                </div>
              </div>

              <div className="luxury-links-row">
                <span className="luxury-hint-text">
                  Cuentas: <code>consulta01</code> · <code>operador01</code>
                </span>
              </div>

              {errorAuth && (
                <div className="luxury-error-banner" role="alert">
                  <AlertTriangle size={15} />
                  <span>{errorAuth}</span>
                  {codigoHttp !== null && <span className="luxury-http-code">HTTP {codigoHttp}</span>}
                </div>
              )}

              <button type="submit" className="luxury-btn-copper" disabled={cargando}>
                {cargando ? (
                  <>
                    <Loader2 size={16} className="spin" />
                    <span>Autenticando…</span>
                  </>
                ) : (
                  'Iniciar sesión'
                )}
              </button>
            </form>
          </div>
        </main>

        <footer className="luxury-login-footer">
          <span>Arquitectura SOA · JWT en memoria · Ventas exclusivas en PWA</span>
        </footer>
      </div>

      {/* PANEL DERECHO: SOLO FOTOGRAFÍA AUTOMOTRIZ */}
      <aside
        className="luxury-login-visual"
        style={{ backgroundImage: `url(${loginCarsImg})` }}
        aria-hidden="true"
      >
        <div className="luxury-visual-fade" />
      </aside>
    </div>
  );
}
