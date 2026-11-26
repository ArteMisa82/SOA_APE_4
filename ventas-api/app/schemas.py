from pydantic import BaseModel, ConfigDict, Field

class CrearComprador(BaseModel):
    model_config = ConfigDict(extra="forbid", str_strip_whitespace=True)
    documento: str = Field(min_length=3, max_length=30,
                           pattern=r"^[A-Za-z0-9-]+$")
    nombre: str = Field(min_length=2, max_length=120,
                        pattern=r"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü0-9 .'-]+$")
    correo: str = Field(min_length=5, max_length=200,
                        pattern=r"^[^@\s]+@[^@\s]+\.[^@\s]+$")

# Esquema de salida para listar compradores
class CompradorResponse(BaseModel):
    id: int
    usuario: str
    documento: str
    nombre: str
    correo: str

class CrearVenta(BaseModel):
    model_config = ConfigDict(extra="forbid")
    vehiculoId: int = Field(gt=0, strict=True)
    compradorId: int = Field(gt=0, strict=True)

class VentaResponse(BaseModel):
    id: int
    operacionId: str
    vehiculoId: int
    compradorId: int
    precioAcordado: str | None = None
    moneda: str | None = None
    estado: str
    notificacion: str
    fechaUTC: str

