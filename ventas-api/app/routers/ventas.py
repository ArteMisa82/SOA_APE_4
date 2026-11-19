from typing import Annotated

from fastapi import APIRouter, Depends, Header, Response
from sqlalchemy import select

from app.db import SessionLocal
from app.models import Venta, Comprador
from app.schemas import CrearVenta, CrearComprador, CompradorResponse
from app.security import identidad, vendedor, propietario
from app.errors import ApiError
from app.services import venta_service as service

router = APIRouter(prefix="/api")

@router.post("/compradores", status_code=201)
def crear_comprador(d: CrearComprador, u=Depends(identidad)):
    vendedor(u)
    with SessionLocal.begin() as db:
        c = Comprador(usuario=u["usuario"], **d.model_dump())
        db.add(c); db.flush()
        return {"id": c.id, "nombre": c.nombre, "correo": c.correo}

# ==============================================================================
# ENDPOINT: GET /api/compradores
# MOTIVO: Permite a las aplicaciones cliente (como la aplicación de escritorio JavaFX)
# consultar y listar todos los compradores registrados en la base de datos de ventas.
# 1. Requiere autenticación con JWT: u=Depends(identidad) valida el token
#    contra la API de identidad en Spring Boot (puerto 8080).
# 2. Control de Acceso: Verifica que el usuario tenga un rol autorizado (CONSULTA,
#    OPERADOR o ADMIN). Si no lo tiene, lanza un ApiError 403 (SIN_PERMISO).
# 3. Acceso a Datos: Abre una sesión con SessionLocal(), ejecuta un select(Comprador)
#    mediante SQLAlchemy para obtener los compradores de la tabla 'compradores'.
# 4. Serialización: Retorna los datos con la estructura CompradorResponse para
#    garantizar consistencia y documentación automática en Swagger/OpenAPI.
# ==============================================================================
@router.get("/compradores", response_model=list[CompradorResponse])
def listar_compradores(u=Depends(identidad)):
    # Verificación de roles autorizados para consultar
    roles_usuario = set(u.get("roles", []))
    if not {"ROLE_CONSULTA", "ROLE_OPERADOR", "ROLE_ADMIN"}.intersection(roles_usuario):
        vendedor(u)  # Lanza ApiError(403, "SIN_PERMISO", "Se requiere OPERADOR o ADMIN")

    # Consulta a la base de datos MySQL usando la sesión de SQLAlchemy
    with SessionLocal() as db:
        stmt = select(Comprador).order_by(Comprador.id)
        compradores = db.scalars(stmt).all()
        return [
            {
                "id": c.id,
                "usuario": c.usuario,
                "documento": c.documento,
                "nombre": c.nombre,
                "correo": c.correo
            }
            for c in compradores
        ]


@router.post("/ventas")
def crear(d: CrearVenta, response: Response,
          idempotency_key: Annotated[str, Header(
              min_length=8, max_length=100, pattern=r"^[A-Za-z0-9_-]+$")],
          u=Depends(identidad)):
    vendedor(u)
    resultado, codigo = service.crear(d, idempotency_key, u)
    response.status_code = codigo
    response.headers["Location"] = f"/api/ventas/{resultado['id']}"
    return resultado

@router.get("/ventas/{id}")
def obtener(id: int, u=Depends(identidad)):
    with SessionLocal() as db:
        v = db.get(Venta, id)
        if v is None:
            raise ApiError(404, "VENTA_NO_EXISTE", "Venta inexistente")
        propietario(u, v.usuario)
        return service.salida(v)
