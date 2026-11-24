import { Car, Receipt } from 'lucide-react';
import heroShowroomImg from '../../assets/hero-showroom.jpg';
import './Dashboard.css';

export default function ShowroomBanner({ cargando, onConsultarSoloVehiculos, onConsultarSoloVentas }) {
  return (
    <section
      className="showroom-banner"
      style={{ backgroundImage: `url(${heroShowroomImg})` }}
      aria-label="Resumen NEXORA Autos"
    >
      <div className="showroom-copy">
        <span className="showroom-tag">CONCESIONARIO MULTICANAL CONECTADO</span>
        <h2>Inventario Sincronizado y Telemetría Comercial</h2>
        <p>
          Arquitectura orientada a servicios: Catálogo en Spring Boot y Órdenes en FastAPI a través de Gateway NGINX (:8090).
        </p>
      </div>
      <div className="showroom-actions">
        <button type="button" className="btn-hero-action" disabled={cargando} onClick={onConsultarSoloVehiculos}>
          <Car size={14} /> Solo catálogo
        </button>
        <button type="button" className="btn-hero-action" disabled={cargando} onClick={onConsultarSoloVentas}>
          <Receipt size={14} /> Solo ventas
        </button>
      </div>
    </section>
  );
}
