package ec.edu.uta.notificaciones.security;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

@Component
public class NotificacionKeyFilter extends OncePerRequestFilter {

    private final String key;

    public NotificacionKeyFilter(@Value("${app.notificacion-key}") String key) {
        if (key == null || key.length() < 32) {
            throw new IllegalStateException("Clave de servicio corta");
        }
        this.key = key;
    }

    @Override
    protected boolean shouldNotFilter(HttpServletRequest request) {
        return "GET".equalsIgnoreCase(request.getMethod())
                && request.getRequestURI().startsWith("/api/notificaciones");
    }

    @Override
    protected void doFilterInternal(HttpServletRequest req, HttpServletResponse res, FilterChain chain)
            throws IOException, ServletException {
        String providedKey = req.getHeader("X-Notificacion-Key");
        if (providedKey == null || !MessageDigest.isEqual(
                key.getBytes(StandardCharsets.UTF_8),
                providedKey.getBytes(StandardCharsets.UTF_8))) {
            res.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            res.setContentType("application/problem+json");
            res.setCharacterEncoding(StandardCharsets.UTF_8.name());
            res.getWriter().write("{\"status\":401,\"title\":\"Servicio no autorizado\"}");
            return;
        }

        chain.doFilter(req, res);
    }
}
