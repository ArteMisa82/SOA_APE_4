using System.Drawing;
using System.Net.Http;
using ConsultaVehiculosForms.DTOs;
using ConsultaVehiculosForms.Services;

namespace ConsultaVehiculosForms
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _api = new();

        public Form1()
        {
            InitializeComponent();
            WireEvents();
        }

        private void WireEvents()
        {
            btnCargar.Click += BtnCargar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            btnTokenValido.Click += BtnTokenValido_Click;
            btnTokenInvalido.Click += BtnTokenInvalido_Click;
        }

        /// <summary>
        /// Implementación directa del ejemplo conceptual en la diapositiva 18 del PDF:
        /// Manejo asíncrono para no congelar la pantalla, reporte del estado de carga y captura de errores HTTP / red.
        /// </summary>
        private async void BtnCargar_Click(object? sender, EventArgs e)
        {
            btnCargar.Enabled = false;
            btnLimpiar.Enabled = false;
            lblEstado.ForeColor = Color.DarkSlateBlue;
            lblEstado.Text = "Cargando...";

            try
            {
                var token = txtToken.Text.Trim();
                var autos = await _api.ListarVehiculosAsync(token);

                dgvVehiculos.DataSource = autos;
                FormatearGridVehiculos();

                lblEstado.ForeColor = Color.DarkGreen;
                lblEstado.Text = $"{autos.Count} vehículos cargados correctamente";
            }
            catch (UnauthorizedAccessException ex)
            {
                lblEstado.ForeColor = Color.Crimson;
                lblEstado.Text = "HTTP 401: Token no autorizado o expirado";
                dgvVehiculos.DataSource = null;
                MessageBox.Show(ex.Message, "Error 401 Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HttpRequestException)
            {
                lblEstado.ForeColor = Color.Crimson;
                lblEstado.Text = "No conecta con Java";
                dgvVehiculos.DataSource = null;
                MessageBox.Show("No conecta con el microservicio Java en http://127.0.0.1:8080.\nVerifique que el servicio Spring Boot esté iniciado.", "Error de Red", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                lblEstado.ForeColor = Color.Crimson;
                lblEstado.Text = $"Error: {ex.Message}";
                dgvVehiculos.DataSource = null;
                MessageBox.Show(ex.Message, "Error al Consultar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCargar.Enabled = true;
                btnLimpiar.Enabled = true;
            }
        }

        /// <summary>
        /// Limpia los datos del DataGridView y restablece los mensajes de estado.
        /// </summary>
        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            dgvVehiculos.DataSource = null;
            lblEstado.ForeColor = Color.DarkSlateBlue;
            lblEstado.Text = "Datos limpiados.";
        }

        /// <summary>
        /// Solicita automáticamente un Token JWT válido a Java (POST /oauth/token con operador01).
        /// </summary>
        private async void BtnTokenValido_Click(object? sender, EventArgs e)
        {
            btnTokenValido.Enabled = false;
            lblEstado.ForeColor = Color.DarkSlateBlue;
            lblEstado.Text = "Obteniendo token válido de Java :8080...";

            try
            {
                var token = await _api.ObtenerTokenAsync("operador01", "operador123");
                txtToken.Text = token;
                lblEstado.ForeColor = Color.DarkGreen;
                lblEstado.Text = "Token válido cargado (operador01). Ahora presione 'Consultar Vehículos'.";
            }
            catch (HttpRequestException)
            {
                lblEstado.ForeColor = Color.Crimson;
                lblEstado.Text = "No conecta con Java para generar token.";
                MessageBox.Show("No se pudo conectar con Java :8080 para generar el token automáticamente.\nPuede pegar un token JWT manualmente en la caja de texto si ya dispone de uno.", "Servicio no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblEstado.ForeColor = Color.Crimson;
                lblEstado.Text = $"Error al obtener token: {ex.Message}";
            }
            finally
            {
                btnTokenValido.Enabled = true;
            }
        }

        /// <summary>
        /// Configura un token deliberadamente inválido para verificar que el servicio responda HTTP 401.
        /// </summary>
        private void BtnTokenInvalido_Click(object? sender, EventArgs e)
        {
            txtToken.Text = "token_invalido_prueba_error_401_unauthorized";
            lblEstado.ForeColor = Color.DarkOrange;
            lblEstado.Text = "Token inválido asignado. Presione 'Consultar Vehículos' para validar el error 401.";
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
    }
}
