import { useMemo } from 'react';
import { Car, CheckCircle2, DollarSign, Sparkles, TrendingUp } from 'lucide-react';
import { mostrarPrecio } from '../../utils/formatters.js';
import './Dashboard.css';

export default function ResumenKpis({ vehiculos, ventas }) {
  const kpis = useMemo(() => {
    const total = vehiculos.length;
    const disponibles = vehiculos.filter((v) => String(v.estado).toUpperCase() === 'DISPONIBLE').length;
    const reservados = vehiculos.filter((v) => String(v.estado).toUpperCase() === 'RESERVADO').length;
    const vendidos = vehiculos.filter((v) => String(v.estado).toUpperCase() === 'VENDIDO').length;

    const totalVentas = ventas.length;
    const completadas = ventas.filter((v) => String(v.estado).toUpperCase() === 'COMPLETADA');
    const montoTotal = completadas.reduce((acc, v) => {
      const val = Number(v.precioAcordado);
      return Number.isFinite(val) ? acc + val : acc;
    }, 0);

    const pctDisp = total > 0 ? (disponibles / total) * 100 : 0;
    const pctRes = total > 0 ? (reservados / total) * 100 : 0;
    const pctVend = total > 0 ? (vendidos / total) * 100 : 0;

    return {
      total,
      disponibles,
      reservados,
      vendidos,
      totalVentas,
      ventasCompletadas: completadas.length,
      montoTotal,
      pctDisp,
      pctRes,
      pctVend,
    };
  }, [vehiculos, ventas]);

  return (
    <>
      <section className="kpi-grid" aria-label="Indicadores reales del sistema">
        <article className="kpi-card card-kpi-total">
          <div className="kpi-top">
            <span className="kpi-label">Inventario total</span>
            <span className="kpi-icon graphite" aria-hidden="true"><Car size={18} /></span>
          </div>
          <div className="kpi-middle">
            <strong className="kpi-value tabular">{kpis.total}</strong>
            <span className="kpi-pill-sub">Unidades activas</span>
          </div>
          <span className="kpi-sub">Registros reales en Spring Boot</span>
        </article>

        <article className="kpi-card card-kpi-avail">
          <div className="kpi-top">
            <span className="kpi-label">Disponibles</span>
            <span className="kpi-icon green" aria-hidden="true"><CheckCircle2 size={18} /></span>
          </div>
          <div className="kpi-middle">
            <strong className="kpi-value tabular" style={{ color: 'var(--status-available-fg)' }}>
              {kpis.disponibles}
            </strong>
            <span className="kpi-pill-avail tabular">{kpis.pctDisp.toFixed(0)}% del stock</span>
          </div>
          <span className="kpi-sub">{kpis.reservados} reservado(s) · {kpis.vendidos} vendido(s)</span>
        </article>

        <article className="kpi-card card-kpi-ventas">
          <div className="kpi-top">
            <span className="kpi-label">Ventas consultadas</span>
            <span className="kpi-icon blue" aria-hidden="true"><TrendingUp size={18} /></span>
          </div>
          <div className="kpi-middle">
            <strong className="kpi-value tabular">{kpis.totalVentas}</strong>
            <span className="kpi-pill-blue tabular">{kpis.ventasCompletadas} cerradas</span>
          </div>
          <span className="kpi-sub">{kpis.ventasCompletadas} completada(s) en FastAPI</span>
        </article>

        <article className="kpi-card card-kpi-monto">
          <div className="kpi-top">
            <span className="kpi-label">Volumen vendido</span>
            <span className="kpi-icon copper" aria-hidden="true"><DollarSign size={18} /></span>
          </div>
          <div className="kpi-middle">
            <strong className="kpi-value tabular">{mostrarPrecio(kpis.montoTotal)}</strong>
          </div>
          <span className="kpi-sub">Monto verificado en órdenes (USD)</span>
        </article>
      </section>

      <section className="dist-card" aria-label="Distribución actual del inventario">
        <div className="dist-head">
          <div className="dist-title-box">
            <Sparkles size={16} className="dist-icon-sparkle" />
            <span className="dist-title">Distribución de estados del catálogo automotriz</span>
          </div>
          <div className="dist-legend tabular">
            <span className="legend-item legend-avail">
              <span className="legend-dot" style={{ background: 'var(--status-available-fg)' }} />
              Disponibles: <strong>{kpis.disponibles}</strong> ({kpis.pctDisp.toFixed(0)}%)
            </span>
            <span className="legend-item legend-res">
              <span className="legend-dot" style={{ background: 'var(--status-reserved-fg)' }} />
              Reservados: <strong>{kpis.reservados}</strong> ({kpis.pctRes.toFixed(0)}%)
            </span>
            <span className="legend-item legend-sold">
              <span className="legend-dot" style={{ background: '#94A3B8' }} />
              Vendidos: <strong>{kpis.vendidos}</strong> ({kpis.pctVend.toFixed(0)}%)
            </span>
          </div>
        </div>
        <div
          className="dist-bar"
          role="img"
          aria-label={`Disponibles ${kpis.disponibles}, Reservados ${kpis.reservados}, Vendidos ${kpis.vendidos}`}
        >
          <div className="dist-seg-avail" style={{ width: `${kpis.pctDisp}%` }} title={`Disponibles: ${kpis.disponibles}`} />
          <div className="dist-seg-res" style={{ width: `${kpis.pctRes}%` }} title={`Reservados: ${kpis.reservados}`} />
          <div className="dist-seg-sold" style={{ width: `${kpis.pctVend}%` }} title={`Vendidos: ${kpis.vendidos}`} />
        </div>
      </section>
    </>
  );
}
