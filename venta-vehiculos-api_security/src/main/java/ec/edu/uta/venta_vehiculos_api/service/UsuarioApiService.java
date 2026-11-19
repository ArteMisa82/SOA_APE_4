package ec.edu.uta.venta_vehiculos_api.service;

// Importa el codificador de contraseñas de Spring Security.
import org.springframework.security.crypto.password.PasswordEncoder;

// Marca la clase como un servicio gestionado por Spring.
import org.springframework.stereotype.Service;

// Importa la entidad de usuario.
import ec.edu.uta.venta_vehiculos_api.model.UsuarioApi;

// Importa el repositorio de usuarios.
import ec.edu.uta.venta_vehiculos_api.repository.UsuarioApiRepository;

// Servicio de autenticación para usuarios de la API.
@Service
public class UsuarioApiService {

    // Repositorio para consultar usuarios.
    private final UsuarioApiRepository repository;

    // Encoder para comparar contraseñas de forma segura.
    private final PasswordEncoder passwordEncoder;

    // Constructor con inyección de dependencias.
    public UsuarioApiService(
            UsuarioApiRepository repository,
            PasswordEncoder passwordEncoder) {

        this.repository = repository;
        this.passwordEncoder = passwordEncoder;
    }

    // Valida las credenciales del usuario y devuelve la entidad autenticada.
    public UsuarioApi autenticar(
            String username,
            String password) {

        // Busca el usuario por nombre de usuario; si no existe, lanza error.
        UsuarioApi usuario =
                repository.findByUsername(username)
                        .orElseThrow(() ->
                                new IllegalArgumentException(
                                        "Usuario o contraseña incorrectos"
                                )
                        );

        // Compara la contraseña ingresada con la almacenada en base de datos.
        if (!passwordEncoder.matches(
                password,
                usuario.getPassword())) {

            // Si no coincide, lanza el mismo error genérico por seguridad.
            throw new IllegalArgumentException(
                    "Usuario o contraseña incorrectos"
            );
        }

        // Devuelve el usuario autenticado.
        return usuario;
    }
}