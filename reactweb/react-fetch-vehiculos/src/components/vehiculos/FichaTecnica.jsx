import { useEffect, useState } from 'react';
import { Calendar, Car, Check, Copy, Hash, Palette, ShieldCheck, Tag } from 'lucide-react';
import { claseEstadoVehiculo, mostrarPrecio, obtenerFotoVehiculo } from '../../utils/formatters.js';
import './Vehiculos.css';

export default function FichaTecnica({ seleccionado }) {
  const [vinCopiado, setVinCopiado] = useState(false);

  useEffect(() => {
    setVinCopiado(false);
  }, [seleccionado]);

  async function copiarVin() {
    if (!seleccionado?.chasis) return;
    try {
      await navigator.clipboard.writeText(String(seleccionado.chasis));
      setVinCopiado(true);
      setTimeout(() => setVinCopiado(false), 1800);
    } catch {
      setVinCopiado(false);
    }
  }

  if (!seleccionado) {
    return (
      <div className="empty-box" style={{ padding: '36px 20px' }}>
        <Car size={32} className="empty-icon-color" />
        <strong>Sin vehículo seleccionado</strong>
        <span>Haga clic en cualquier fila o tarjeta del catálogo para abrir su ficha técnica de auditoría y copiar su VIN.</span>
      </div>
    );
  }

  const esVendido = String(seleccionado.estado).toUpperCase() === 'VENDIDO';

  return (
    <div className="fade-in detail-container">
      <div
        className={`detail-visual ${esVendido ? 'is-sold' : ''}`}
        style={{ backgroundImage: `url(${obtenerFotoVehiculo(seleccionado)})` }}
      >
        <div className="detail-visual-top">
          <span className="plate-tag-dark">
            ID #{seleccionado.idVehiculo ?? seleccionado.id ?? '—'} · {seleccionado.placa ?? 'SIN PLACA'}
          </span>
          <span className={claseEstadoVehiculo(seleccionado.estado)}>
            <span className="dot pulse-dot" />
            {seleccionado.estado ?? '—'}
          </span>
        </div>
        <div className="detail-visual-content">
          <span className="detail-category-badge">Ficha Técnica Oficial</span>
          <h4 className="detail-title">
            {seleccionado.marca ?? '—'} {seleccionado.modelo ?? ''}
          </h4>
          <div className="detail-price-box">
            <span className="detail-price-label">Valor en plaza</span>
            <div className={`detail-price tabular ${esVendido ? 'price-sold' : ''}`}>
              {mostrarPrecio(seleccionado.precio)}
            </div>
          </div>
        </div>
      </div>

      <div className="detail-specs">
        <div className="spec-box">
          <span className="spec-label"><Tag size={13} /> Marca</span>
          <span className="spec-val">{seleccionado.marca ?? '—'}</span>
        </div>
        <div className="spec-box">
          <span className="spec-label"><Car size={13} /> Modelo</span>
          <span className="spec-val">{seleccionado.modelo ?? '—'}</span>
        </div>
        <div className="spec-box">
          <span className="spec-label"><Calendar size={13} /> Año</span>
          <span className="spec-val tabular">{seleccionado.anio ?? '—'}</span>
        </div>
        <div className="spec-box">
          <span className="spec-label"><Palette size={13} /> Color</span>
          <span className="spec-val">{seleccionado.color ?? '—'}</span>
        </div>
        <div className="spec-box">
          <span className="spec-label"><Hash size={13} /> Placa</span>
          <span className="spec-val mono" style={{ fontWeight: 600 }}>{seleccionado.placa ?? '—'}</span>
        </div>
        <div className="spec-box">
          <span className="spec-label"><ShieldCheck size={13} /> Estado</span>
          <span className="spec-val" style={{ fontWeight: 600 }}>{seleccionado.estado ?? '—'}</span>
        </div>
      </div>

      <div className="vin-box">
        <div className="vin-info">
          <span className="vin-label">Número de Chasis (VIN)</span>
          <span className="vin-code tabular">{seleccionado.chasis ?? 'No registrado'}</span>
        </div>
        {seleccionado.chasis && (
          <button
            type="button"
            className={`btn-copy-vin ${vinCopiado ? 'copied' : ''}`}
            onClick={copiarVin}
            title="Copiar número de chasis"
          >
            {vinCopiado ? <Check size={14} /> : <Copy size={14} />}
            <span>{vinCopiado ? 'Copiado' : 'Copiar'}</span>
          </button>
        )}
      </div>
    </div>
  );
}
