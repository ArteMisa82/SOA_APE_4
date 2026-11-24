using ConsultaVehiculosForms.DTOs;
using ConsultaVehiculosForms.Services;

namespace ConsultaVehiculosForms
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _api = new();
        private string _jwtToken = string.Empty;
        private string _usuarioActual = string.Empty;
        private string _moduloActual = "vehiculos";
        private string _kpiVehiculosTexto = "0 vehículos";
        private string _kpiCompradoresTexto = "0 compradores";
        private string _kpiVentasTexto = "0 ventas";

        public Form1()
        {
            InitializeComponent();
            txtGateway.Text = _api.BaseUrl;
            CargarImagenesVisuales();
            WireEvents();
            CentrarTarjetaLogin();
            SeleccionarModulo("vehiculos");
            MostrarPantallaLogin(true);
        }

        private void CargarImagenesVisuales()
        {
            var logo = Theme.LoadAsset("nexora-logo.png");
            if (logo != null)
            {
                picLoginLogo.Image = logo;
                picSidebarLogo.Image = (Image)logo.Clone();
            }

            pnlLoginRight.CoverImage = Theme.LoadAsset("login-cars.jpg");
        }

        private void CentrarTarjetaLogin()
        {
            int x = Math.Max(24, (pnlLoginLeft.ClientSize.Width - pnlLoginCard.Width) / 2);
            int y = Math.Max(18, (pnlLoginLeft.ClientSize.Height - pnlLoginCard.Height) / 2);
            pnlLoginCard.Location = new Point(x, y);
        }

        private void MostrarPantallaLogin(bool mostrarLogin)
        {
            pnlLoginScreen.Visible = mostrarLogin;
            pnlAppWorkspace.Visible = !mostrarLogin;
            BackColor = mostrarLogin ? Theme.Graphite920 : Theme.Canvas;
            AcceptButton = mostrarLogin ? btnIniciarSesion : null;
            if (mostrarLogin)
            {
                CentrarTarjetaLogin();
                txtUsuario.Focus();
            }
        }

        private void WireEvents()
        {
            pnlLoginLeft.Resize += (_, _) => CentrarTarjetaLogin();
            chkMostrarPass.CheckedChanged += (_, _) =>
                txtPassword.UseSystemPasswordChar = !chkMostrarPass.Checked;

            btnNavVehiculos.Click += (_, _) => SeleccionarModulo("vehiculos");
            btnNavCompradores.Click += (_, _) => SeleccionarModulo("compradores");
            btnNavVentas.Click += (_, _) => SeleccionarModulo("ventas");

            btnIniciarSesion.Click += BtnIniciarSesion_Click;
            btnCerrarSesion.Click += BtnCerrarSesion_Click;
            btnTokenInvalidoLogin.Click += BtnTokenInvalido_Click;
            btnTokenInvalido.Click += BtnTokenInvalido_Click;

            btnCrearVehiculo.Click += BtnCrearVehiculo_Click;
            btnListarVehiculos.Click += BtnListarVehiculos_Click;
            btnLimpiarVehiculos.Click += (_, _) =>
            {
                dgvVehiculos.DataSource = null;
                _kpiVehiculosTexto = "0 vehículos";
                ActualizarKpiTopbar();
                LimpiarFormularioVehiculo();
                ActualizarEstado("Formulario y listado de vehículos limpiados.", Theme.Ink700);
            };

            btnCrearComprador.Click += BtnCrearComprador_Click;
            btnListarCompradores.Click += BtnListarCompradores_Click;
            btnLimpiarCompradores.Click += (_, _) =>
            {
                dgvCompradores.DataSource = null;
                _kpiCompradoresTexto = "0 compradores";
                ActualizarKpiTopbar();
                LimpiarFormularioComprador();
                ActualizarEstado("Formulario y listado de compradores limpiados.", Theme.Ink700);
            };

            btnListarVentas.Click += BtnListarVentas_Click;
            btnLimpiarVentas.Click += (_, _) =>
            {
                dgvVentas.DataSource = null;
                _kpiVentasTexto = "0 ventas";
                ActualizarKpiTopbar();
                ActualizarEstado("Listado de ventas limpiado.", Theme.Ink700);
            };

            dgvVehiculos.DataBindingComplete += (_, _) => FormatearGridVehiculos();
            dgvVentas.DataBindingComplete += (_, _) => FormatearGridVentas();
        }

        private void SeleccionarModulo(string modulo)
        {
            _moduloActual = modulo;
            pnlModVehiculos.Visible = modulo == "vehiculos";
            pnlModCompradores.Visible = modulo == "compradores";
            pnlModVentas.Visible = modulo == "ventas";

            ActualizarNavVisual(pnlNavVehiculos, pnlIndVehiculos, btnNavVehiculos, modulo == "vehiculos");
            ActualizarNavVisual(pnlNavCompradores, pnlIndCompradores, btnNavCompradores, modulo == "compradores");
            ActualizarNavVisual(pnlNavVentas, pnlIndVentas, btnNavVentas, modulo == "ventas");

            if (modulo == "vehiculos")
            {
                lblTopTitle.Text = "Gestión de Vehículos";
                lblTopSub.Text = "POST /api/vehiculos y GET /api/vehiculos (Spring Boot vía NGINX :8090)";
            }
            else if (modulo == "compradores")
            {
                lblTopTitle.Text = "Gestión de Compradores";
                lblTopSub.Text = "POST /api/compradores y GET /api/compradores (FastAPI vía NGINX :8090)";
            }
            else
            {
                lblTopTitle.Text = "Consulta de Ventas (Solo lectura)";
                lblTopSub.Text = "GET /api/ventas (FastAPI vía NGINX :8090) · Sin registro de ventas";
            }

            ActualizarKpiTopbar();
        }

        private void ActualizarKpiTopbar()
        {
            lblTopKpi.Text = _moduloActual switch
            {
                "vehiculos" => _kpiVehiculosTexto,
                "compradores" => _kpiCompradoresTexto,
                _ => _kpiVentasTexto
            };
        }

        private static void ActualizarNavVisual(Panel container, Panel indicator, Button button, bool active)
        {
            container.BackColor = active ? Theme.Graphite900 : Theme.Graphite950;
            button.BackColor = active ? Theme.Graphite900 : Theme.Graphite950;
            button.ForeColor = active ? Color.White : Theme.TextOnDarkMuted;
            indicator.BackColor = active ? Theme.Copper600 : Color.Transparent;
        }

        // ==================== AUTENTICACIÓN JWT ====================

        private async void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            var usuario = txtUsuario.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                MostrarErrorLogin("Ingrese usuario y contraseña para iniciar sesión.");
                return;
            }

            lblLoginError.Visible = false;
            btnIniciarSesion.Enabled = false;
            btnIniciarSesion.Text = "Autenticando…";

            try
            {
                SincronizarConfigGateway();
                var (token, status) = await _api.ObtenerTokenAsync(usuario, password, txtClientSecret.Text);
                _jwtToken = token;
                _usuarioActual = usuario;
                txtPassword.Clear();
                chkMostrarPass.Checked = false;

                lblSesion.BackColor = Color.FromArgb(14, 58, 42);
                lblSesion.ForeColor = Color.FromArgb(167, 243, 208);
                lblSesion.Text = $"Activa: {_usuarioActual}";

                MostrarPantallaLogin(false);
                SeleccionarModulo("vehiculos");
                ActualizarEstado($"HTTP {status}: Autenticación exitosa como '{_usuarioActual}' (JWT en memoria).", Theme.AvailableFg);
            }
            catch (ApiHttpException ex)
            {
                MostrarErrorLogin(ex.Message);
            }
            catch (HttpRequestException)
            {
                MostrarErrorLogin($"No se pudo contactar con NGINX en {_api.BaseUrl}. Verifique los servicios.");
            }
            catch (Exception ex)
            {
                MostrarErrorLogin(ex.Message);
            }
            finally
            {
                btnIniciarSesion.Enabled = true;
                btnIniciarSesion.Text = "Iniciar sesión";
            }
        }

        private void MostrarErrorLogin(string mensaje)
        {
            lblLoginError.Text = mensaje;
            lblLoginError.Visible = true;
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            _jwtToken = string.Empty;
            _usuarioActual = string.Empty;
            dgvVehiculos.DataSource = null;
            dgvCompradores.DataSource = null;
            dgvVentas.DataSource = null;
            _kpiVehiculosTexto = "0 vehículos";
            _kpiCompradoresTexto = "0 compradores";
            _kpiVentasTexto = "0 ventas";
            ActualizarKpiTopbar();

            lblSesion.BackColor = Theme.Graphite900;
            lblSesion.ForeColor = Color.FromArgb(226, 232, 240);
            lblSesion.Text = "Sin sesión activa";
            lblLoginError.Visible = false;

            MostrarPantallaLogin(true);
        }

        private void BtnTokenInvalido_Click(object? sender, EventArgs e)
        {
            _jwtToken = "jwt_invalido_prueba_error_401_no_autorizado";
            _usuarioActual = "prueba-401";
            lblSesion.BackColor = Color.FromArgb(78, 28, 30);
            lblSesion.ForeColor = Color.FromArgb(254, 202, 202);
            lblSesion.Text = "JWT inválido (prueba 401)";
            MostrarPantallaLogin(false);
            ActualizarEstado("Token inválido asignado en memoria. Ejecute cualquier operación para verificar HTTP 401.", Theme.WarnFg);
        }

        // ==================== OPERACIONES DE VEHÍCULOS ====================

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
                ActualizarEstado("Validación: Complete marca, modelo, placa y color.", Theme.WarnFg);
                MessageBox.Show("Todos los campos del vehículo son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chasis.Length != 17)
            {
                ActualizarEstado("Validación: El número de chasis debe tener exactamente 17 caracteres.", Theme.WarnFg);
                MessageBox.Show("El chasis debe tener exactamente 17 caracteres alfanuméricos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await EjecutarOperacionAsync("Registrando vehículo en Spring Boot vía NGINX…", async () =>
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
                ActualizarKpiVehiculos(lista);

                ActualizarEstado(
                    $"HTTP {status}: Vehículo ID {creado.IdVehiculo} ({creado.Marca} {creado.Modelo} - {creado.Placa}) registrado en estado {creado.Estado}.",
                    Theme.AvailableFg);
            });
        }

        private async void BtnListarVehiculos_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando vehículos (GET /api/vehiculos vía NGINX)…", async () =>
            {
                SincronizarConfigGateway();
                var (autos, status) = await _api.ListarVehiculosAsync(_jwtToken);
                dgvVehiculos.DataSource = autos;
                ActualizarKpiVehiculos(autos);

                ActualizarEstado($"HTTP {status}: {autos.Count} vehículo(s) cargados desde Spring Boot vía NGINX.", Theme.AvailableFg);
            });
        }

        private void ActualizarKpiVehiculos(List<VehiculoDto> autos)
        {
            var disp = autos.Count(v => v.Estado.Equals("DISPONIBLE", StringComparison.OrdinalIgnoreCase));
            var res = autos.Count(v => v.Estado.Equals("RESERVADO", StringComparison.OrdinalIgnoreCase));
            var vend = autos.Count(v => v.Estado.Equals("VENDIDO", StringComparison.OrdinalIgnoreCase));
            _kpiVehiculosTexto = $"{autos.Count} total · {disp} disp. · {res} res. · {vend} vend.";
            ActualizarKpiTopbar();
        }

        // ==================== OPERACIONES DE COMPRADORES ====================

        private async void BtnCrearComprador_Click(object? sender, EventArgs e)
        {
            var documento = txtDocumento.Text.Trim();
            var nombre = txtNombreComp.Text.Trim();
            var correo = txtCorreoComp.Text.Trim();

            if (documento.Length < 3 || nombre.Length < 2 || !correo.Contains('@'))
            {
                ActualizarEstado("Validación: Ingrese documento (mín. 3), nombre (mín. 2) y correo válido.", Theme.WarnFg);
                MessageBox.Show("Verifique documento, nombre completo y correo electrónico del comprador.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await EjecutarOperacionAsync("Registrando comprador en FastAPI vía NGINX…", async () =>
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
                _kpiCompradoresTexto = $"{lista.Count} comprador(es)";
                ActualizarKpiTopbar();

                ActualizarEstado(
                    $"HTTP {status}: Comprador ID {creado.Id} ({creado.Nombre} - {creado.Documento}) registrado en FastAPI.",
                    Theme.AvailableFg);
            });
        }

        private async void BtnListarCompradores_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando compradores (GET /api/compradores vía NGINX)…", async () =>
            {
                SincronizarConfigGateway();
                var (compradores, status) = await _api.ListarCompradoresAsync(_jwtToken);
                dgvCompradores.DataSource = compradores;
                _kpiCompradoresTexto = $"{compradores.Count} comprador(es)";
                ActualizarKpiTopbar();

                ActualizarEstado($"HTTP {status}: {compradores.Count} comprador(es) cargados desde FastAPI vía NGINX.", Theme.AvailableFg);
            });
        }

        // ==================== CONSULTA DE VENTAS (SOLO LECTURA) ====================

        private async void BtnListarVentas_Click(object? sender, EventArgs e)
        {
            await EjecutarOperacionAsync("Consultando ventas (GET /api/ventas vía NGINX)…", async () =>
            {
                SincronizarConfigGateway();
                var (ventas, status) = await _api.ListarVentasAsync(_jwtToken);
                dgvVentas.DataSource = ventas;
                var comp = ventas.Count(v => v.Estado.Equals("COMPLETADA", StringComparison.OrdinalIgnoreCase));
                _kpiVentasTexto = $"{ventas.Count} total · {comp} completadas";
                ActualizarKpiTopbar();

                ActualizarEstado($"HTTP {status}: {ventas.Count} venta(s) consultadas desde FastAPI vía NGINX (solo lectura).", Theme.AvailableFg);
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
                ActualizarEstado(ex.Message, Theme.ErrorFg);
                MessageBox.Show(ex.Message, $"Error HTTP {(int)ex.StatusCode}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HttpRequestException)
            {
                var msg = $"Error de conexión: No se pudo contactar con NGINX en {_api.BaseUrl}. Verifique que los servicios estén iniciados.";
                ActualizarEstado(msg, Theme.ErrorFg);
                MessageBox.Show(msg, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                const string msg = "Tiempo de espera agotado (timeout) al comunicarse con el servicio.";
                ActualizarEstado(msg, Theme.WarnFg);
                MessageBox.Show(msg, "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error: {ex.Message}", Theme.ErrorFg);
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
            btnCrearVehiculo.Enabled = !busy;
            btnListarVehiculos.Enabled = !busy;
            btnCrearComprador.Enabled = !busy;
            btnListarCompradores.Enabled = !busy;
            btnListarVentas.Enabled = !busy;
            if (!string.IsNullOrEmpty(mensaje))
            {
                ActualizarEstado(mensaje, Theme.Ink700);
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
                colPrecio.DefaultCellStyle.Font = Theme.FontBodyBold;
            }

            foreach (DataGridViewRow row in dgvVehiculos.Rows)
            {
                if (row.DataBoundItem is not VehiculoDto v) continue;

                var cellEstado = row.Cells["Estado"];
                if (v.Estado.Equals("DISPONIBLE", StringComparison.OrdinalIgnoreCase))
                {
                    cellEstado.Style.BackColor = Theme.AvailableBg;
                    cellEstado.Style.ForeColor = Theme.AvailableFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;
                }
                else if (v.Estado.Equals("RESERVADO", StringComparison.OrdinalIgnoreCase))
                {
                    cellEstado.Style.BackColor = Theme.ReservedBg;
                    cellEstado.Style.ForeColor = Theme.ReservedFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;
                }
                else if (v.Estado.Equals("VENDIDO", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Theme.Canvas;
                    row.DefaultCellStyle.ForeColor = Theme.Ink400;
                    cellEstado.Style.BackColor = Theme.SoldBg;
                    cellEstado.Style.ForeColor = Theme.SoldFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;

                    if (colPrecio != null)
                    {
                        var cellPrecio = row.Cells["Precio"];
                        cellPrecio.Style.Font = Theme.FontStrike;
                        cellPrecio.Style.ForeColor = Theme.Ink400;
                    }
                }
            }
        }

        private void FormatearGridVentas()
        {
            var colPrecio = dgvVentas.Columns["PrecioAcordado"];
            if (colPrecio != null)
            {
                colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colPrecio.DefaultCellStyle.Font = Theme.FontBodyBold;
            }

            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                if (row.DataBoundItem is not VentaDto v) continue;

                var cellEstado = row.Cells["Estado"];
                if (v.Estado.Equals("COMPLETADA", StringComparison.OrdinalIgnoreCase))
                {
                    cellEstado.Style.BackColor = Theme.AvailableBg;
                    cellEstado.Style.ForeColor = Theme.AvailableFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;
                }
                else if (v.Estado.Equals("FALLIDA", StringComparison.OrdinalIgnoreCase))
                {
                    cellEstado.Style.BackColor = Theme.ErrorBg;
                    cellEstado.Style.ForeColor = Theme.ErrorFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;
                }
                else
                {
                    cellEstado.Style.BackColor = Theme.ReservedBg;
                    cellEstado.Style.ForeColor = Theme.ReservedFg;
                    cellEstado.Style.Font = Theme.FontSmallBold;
                }
            }
        }
    }
}
