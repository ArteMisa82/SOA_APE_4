package ec.edu.uta.venta_vehiculos_api.repository;

// Importa Optional para manejar búsquedas que pueden no devolver usuario.
import java.util.Optional;

// Importa Spring Data JPA para repositorio CRUD.
import org.springframework.data.jpa.repository.JpaRepository;

// Importa la entidad UsuarioApi.
import ec.edu.uta.venta_vehiculos_api.model.UsuarioApi;

// Repositorio para gestionar usuarios de la API.
public interface UsuarioApiRepository
        extends JpaRepository<UsuarioApi, Integer> {

    // Busca un usuario por su nombre de usuario.
    Optional<UsuarioApi> findByUsername(String username);
}