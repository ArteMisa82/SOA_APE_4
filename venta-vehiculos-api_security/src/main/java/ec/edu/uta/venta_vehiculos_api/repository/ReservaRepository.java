package ec.edu.uta.venta_vehiculos_api.repository;

// Importa el tipo Optional para manejar valores vacíos.
import java.util.Optional;

// Importa la base de Spring Data JPA para repositorios CRUD.
import org.springframework.data.jpa.repository.JpaRepository;

// Importa la entidad Reserva.
import ec.edu.uta.venta_vehiculos_api.model.Reserva;

// Repositorio para acceder a reservas en la base de datos.
public interface ReservaRepository
 extends JpaRepository<Reserva,String> {

 // Busca una reserva por el identificador de operación externo.
 Optional<Reserva> findByOperacionId(String operacionId);

 // Verifica si existe al menos una reserva asociada a un vehículo.
 boolean existsByVehiculo_IdVehiculo(Integer id);
}

