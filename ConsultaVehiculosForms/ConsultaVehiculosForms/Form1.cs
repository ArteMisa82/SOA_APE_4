using ConsultaVehiculosForms.DTOs;
using ConsultaVehiculosForms.Services;

namespace ConsultaVehiculosForms
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _api = new();
        private string _jwtToken = string.Empty;
        private string _usuarioActual = string.Empty;

        public Form1()
        {
            InitializeComponent();
            txtGateway.Text = _api.BaseUrl;
            WireEvents();
        }

        private void WireEvents()
        {
            btnIniciarSesion.Click += BtnIniciarSesion_Click;
            btnCerrarSesion.Click += BtnCerrarSesion_Click;
            btnTokenInvalido.Click += BtnTokenInvalido_Click;

            btnCrearVehiculo.Click += BtnCrearVehiculo_Click;
            btnListarVehiculos.Click += BtnListarVehiculos_Click;
            btnLimpiarVehiculos.Click += (_, _) =>
            {
                dgvVehiculos.DataSource = null;
                LimpiarFormularioVehiculo();
                ActualizarEstado("Formulario y listado de vehículos limpiados.", Color.DarkSlateBlue);
            };

            btnCrearComprador.Click += BtnCrearComprador_Click;
            btnListarCompradores.Click += BtnListarCompradores_Click;
            btnLimpiarCompradores.Click += (_, _) =>
            {
                dgvCompradores.DataSource = null;
                LimpiarFormularioComprador();
                ActualizarEstado("Formulario y listado de compradores limpiados.", Color.DarkSlateBlue);
            };

            btnListarVentas.Click += BtnListarVentas_Click;
            btnLimpiarVentas.Click += (_, _) =>
            {
                dgvVentas.DataSource = null;
                ActualizarEstado("Listado de ventas limpiado.", Color.DarkSlateBlue);
            };
        }

        // ==================== AUTENTICACION JWT ====================

        private async void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            var usuario = txtUsuario.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                ActualizarEstado("Validación: Ingrese usuario y contraseña para iniciar sesión.", Color.DarkOrange);
                MessageBox.Show("Debe ingresar usuario y contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await EjecutarOperacionAsync("Autenticando vía NGINX (/oauth/token)...", async () =>
            {
                SincronizarConfigGateway();
                var (token, status) = await _api.ObtenerTokenAsync(usuario, password, txtClientSecret.Text);
                _jwtToken = token;
                _usuarioActual = usuario;
                txtPassword.Clear();

                lblSesion.ForeColor = Color.DarkGreen;
                lblSesion.Text = $"Sesión activa: {_usuarioActual} (JWT en memoria)";
                ActualizarEstado($"HTTP {status}: Autenticación exitosa como '{_usuarioActual}'. Listo para operar.", Color.DarkGreen);
            });
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            _jwtToken = string.Empty;
            _usuarioActual = string.Empty;
            dgvVehiculos.DataSource = null;
            dgvCompradores.DataSource = null;
            dgvVentas.DataSource = null;
            lblSesion.ForeColor = Color.DarkSlateBlue;
            lblSesion.Text = "Sesión: No autenticado";
            ActualizarEstado("Sesión cerrada y token eliminado de memoria.", Color.DarkSlateBlue);
        }

        private void BtnTokenInvalido_Click(object? sender, EventArgs e)
        {
            _jwtToken = "jwt_invalido_prueba_error_401_no_autorizado";
            _usuarioActual = "(token inválido de prueba)";
            lblSesion.ForeColor = Color.DarkOrange;
            lblSesion.Text = "Sesión: Token inválido cargado (prueba 401)";
            ActualizarEstado("Token inválido asignado en memoria. Ejecute cualquier operación para verificar HTTP 401.", Color.DarkOrange);
        }

        // ==================== OPERACIONES DE VEHICULOS ====================

        private async void BtnCrearVehiculo_Click(object? sender, EventArgs e)
        {
            var marca = txtMarca.Text.Trim();
            var modelo = txtModelo.Text.Trim();
            var placa = txtPlaca.Text.Trim().ToUpperInvariant();
            var chasis = txtChasis.Text.Trim().ToUpperInvariant();
            var color = txtColor.Text.Trim();

            if (string.IsNullOrWhiteSpace(marca) || string.IsNullOrWhiteSpace(modelo) ||
                string.IsNullOrWhiteSpace(placa) || string.IsNullOrWhiteSpace(color))
            {
                ActualizarEstado("Validación: Complete marca, modelo, placa y color.", Color.DarkOrange);
                MessageBox.Show("Todos los campos del vehículo son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chasis.Length != 17)
            {
                ActualizarEstado("Validación: El número de chasis debe tener exactamente 17 caracteres.", Color.DarkOrange);
                MessageBox.Show("El chasis debe tener exactamente 17 caracteres alfanuméricos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await EjecutarOperacionAsync("Registrando vehículo en Spring Boot vía NGINX...", async () =>
            {
                SincronizarConfigGateway();
                var dto = new CrearVehiculoDto
                {
                    Marca = marca,
                    Modelo = modelo,
                    Placa = placa,
                    Chasis = chasis,
                    Anio = (int)numAnio.Value,
                    Color = color,
                    Precio = numPrecio.Value
                };

                var (creado, status) = await _api.CrearVehiculoAsync(_jwtToken, dto);
                LimpiarFormularioVehiculo();

                var (lista, _) = await _api.ListarVehiculosAsync(_jwtToken);
                dgvVehiculos.DataSource = lista;
                FormatearGridVehiculos();

                ActualizarEstado(
                    $"HTTP {status}: Vehículo ID {creado.IdVehiculo} ({creado.Marca} {creado.Modelo} - {creado.Placa}) registrado en estado {creado.Estado}.",
                    Color.DarkGreen);
            });
        }

        private async void BtnListarVehiculos_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando vehículos (GET /api/vehiculos vía NGINX)...", async () =>
            {
                SincronizarConfigGateway();
                var (autos, status) = await _api.ListarVehiculosAsync(_jwtToken);
                dgvVehiculos.DataSource = autos;
                FormatearGridVehiculos();

                ActualizarEstado($"HTTP {status}: {autos.Count} vehículos cargados desde Spring Boot vía NGINX.", Color.DarkGreen);
            });
        }

        // ==================== OPERACIONES DE COMPRADORES ====================

        private async void BtnCrearComprador_Click(object? sender, EventArgs e)
        {
            var documento = txtDocumento.Text.Trim();
            var nombre = txtNombreComp.Text.Trim();
            var correo = txtCorreoComp.Text.Trim();

            if (documento.Length < 3 || nombre.Length < 2 || !correo.Contains('@'))
            {
                ActualizarEstado("Validación: Ingrese documento (mín. 3), nombre (mín. 2) y correo válido.", Color.DarkOrange);
                MessageBox.Show("Verifique documento, nombre completo y correo electrónico del comprador.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await EjecutarOperacionAsync("Registrando comprador en FastAPI vía NGINX...", async () =>
            {
                SincronizarConfigGateway();
                var dto = new CrearCompradorDto
                {
                    Documento = documento,
                    Nombre = nombre,
                    Correo = correo
                };

                var (creado, status) = await _api.CrearCompradorAsync(_jwtToken, dto);
                LimpiarFormularioComprador();

                var (lista, _) = await _api.ListarCompradoresAsync(_jwtToken);
                dgvCompradores.DataSource = lista;

                ActualizarEstado(
                    $"HTTP {status}: Comprador ID {creado.Id} ({creado.Nombre} - {creado.Documento}) registrado en FastAPI.",
                    Color.DarkGreen);
            });
        }

        private async void BtnListarCompradores_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando compradores (GET /api/compradores vía NGINX)...", async () =>
            {
                SincronizarConfigGateway();
                var (compradores, status) = await _api.ListarCompradoresAsync(_jwtToken);
                dgvCompradores.DataSource = compradores;

                ActualizarEstado($"HTTP {status}: {compradores.Count} compradores cargados desde FastAPI vía NGINX.", Color.DarkGreen);
            });
        }

        // ==================== CONSULTA DE VENTAS (SOLO LECTURA) ====================

        private async void BtnListarVentas_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando ventas (GET /api/ventas vía NGINX)...", async () =>
            {
                SincronizarConfigGateway();
                var (ventas, status) = await _api.ListarVentasAsync(_jwtToken);
                dgvVentas.DataSource = ventas;
                FormatearGridVentas();

                ActualizarEstado($"HTTP {status}: {ventas.Count} ventas consultadas desde FastAPI vía NGINX (solo lectura).", Color.DarkGreen);
            });
        }

        // ==================== UTILIDADES Y MANEJO DE ERRORES ====================

        private void SincronizarConfigGateway()
        {
            if (!string.IsNullOrWhiteSpace(txtGateway.Text))
            {
                _api.BaseUrl = txtGateway.Text.Trim();
            }
        }

        private async Task EjecutarOperacionAsync(string mensajeCarga, Func<Task> accion)
        {
            SetBusy(true, mensajeCarga);
            try
            {
                await accion();
            }
            catch (ApiHttpException ex)
            {
                ActualizarEstado(ex.Message, Color.Crimson);
                MessageBox.Show(ex.Message, $"Error HTTP {(int)ex.StatusCode}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HttpRequestException)
            {
                var msg = $"Error de conexión: No se pudo contactar con NGINX en {_api.BaseUrl}. Verifique que los servicios estén iniciados.";
                ActualizarEstado(msg, Color.Crimson);
                MessageBox.Show(msg, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                const string msg = "Tiempo de espera agotado (timeout) al comunicarse con el servicio.";
                ActualizarEstado(msg, Color.Crimson);
                MessageBox.Show(msg, "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error: {ex.Message}", Color.Crimson);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy, string? mensaje = null)
        {
            prgCarga.Visible = busy;
            btnIniciarSesion.Enabled = !busy;
            btnCrearVehiculo.Enabled = !busy;
            btnListarVehiculos.Enabled = !busy;
            btnCrearComprador.Enabled = !busy;
            btnListarCompradores.Enabled = !busy;
            btnListarVentas.Enabled = !busy;
            if (!string.IsNullOrEmpty(mensaje))
            {
                ActualizarEstado(mensaje, Color.DarkSlateBlue);
            }
        }

        private void ActualizarEstado(string texto, Color color)
        {
            lblEstado.ForeColor = color;
            lblEstado.Text = texto;
        }

        private void LimpiarFormularioVehiculo()
        {
            txtMarca.Clear();
            txtModelo.Clear();
            txtPlaca.Clear();
            txtChasis.Clear();
            txtColor.Clear();
            numAnio.Value = 2026;
            numPrecio.Value = 25000;
        }

        private void LimpiarFormularioComprador()
        {
            txtDocumento.Clear();
            txtNombreComp.Clear();
            txtCorreoComp.Clear();
        }

        private void FormatearGridVehiculos()
        {
            var colPrecio = dgvVehiculos.Columns["Precio"];
            if (colPrecio != null)
            {
                colPrecio.DefaultCellStyle.Format = "C2";
                colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            foreach (DataGridViewRow row in dgvVehiculos.Rows)
            {
                if (row.DataBoundItem is VehiculoDto v)
                {
                    if (v.Estado.Equals("DISPONIBLE", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(235, 247, 238);
                    }
                    else if (v.Estado.Equals("RESERVADO", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 230);
                    }
                    else if (v.Estado.Equals("VENDIDO", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                        row.DefaultCellStyle.ForeColor = Color.DarkGray;
                    }
                }
            }
        }

        private void FormatearGridVentas()
        {
            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                if (row.DataBoundItem is VentaDto v)
                {
                    if (v.Estado.Equals("COMPLETADA", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(235, 247, 238);
                    }
                    else if (v.Estado.Equals("FALLIDA", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(253, 236, 234);
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 230);
                    }
                }
            }
        }
    }
}
