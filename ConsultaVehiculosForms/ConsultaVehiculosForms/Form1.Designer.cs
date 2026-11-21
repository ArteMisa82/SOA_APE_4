namespace ConsultaVehiculosForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitulo = new Label();
            lblSubtitulo = new Label();

            grpAuth = new GroupBox();
            lblGateway = new Label();
            txtGateway = new TextBox();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblClientSecret = new Label();
            txtClientSecret = new TextBox();
            btnIniciarSesion = new Button();
            btnCerrarSesion = new Button();
            btnTokenInvalido = new Button();
            lblSesion = new Label();

            pnlEstado = new Panel();
            lblEstado = new Label();
            prgCarga = new ProgressBar();

            tabPrincipal = new TabControl();

            // Tab 1: Vehiculos
            tabVehiculos = new TabPage();
            grpCrearVehiculo = new GroupBox();
            lblMarca = new Label();
            txtMarca = new TextBox();
            lblModelo = new Label();
            txtModelo = new TextBox();
            lblPlaca = new Label();
            txtPlaca = new TextBox();
            lblChasis = new Label();
            txtChasis = new TextBox();
            lblAnio = new Label();
            numAnio = new NumericUpDown();
            lblColor = new Label();
            txtColor = new TextBox();
            lblPrecio = new Label();
            numPrecio = new NumericUpDown();
            btnCrearVehiculo = new Button();
            btnListarVehiculos = new Button();
            btnLimpiarVehiculos = new Button();
            dgvVehiculos = new DataGridView();

            // Tab 2: Compradores
            tabCompradores = new TabPage();
            grpCrearComprador = new GroupBox();
            lblDocumento = new Label();
            txtDocumento = new TextBox();
            lblNombreComp = new Label();
            txtNombreComp = new TextBox();
            lblCorreoComp = new Label();
            txtCorreoComp = new TextBox();
            btnCrearComprador = new Button();
            btnListarCompradores = new Button();
            btnLimpiarCompradores = new Button();
            dgvCompradores = new DataGridView();

            // Tab 3: Ventas (Solo lectura)
            tabVentas = new TabPage();
            pnlVentasTop = new Panel();
            lblAvisoVentas = new Label();
            btnListarVentas = new Button();
            btnLimpiarVentas = new Button();
            dgvVentas = new DataGridView();

            pnlHeader.SuspendLayout();
            grpAuth.SuspendLayout();
            pnlEstado.SuspendLayout();
            tabPrincipal.SuspendLayout();
            tabVehiculos.SuspendLayout();
            grpCrearVehiculo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAnio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).BeginInit();
            tabCompradores.SuspendLayout();
            grpCrearComprador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompradores).BeginInit();
            tabVentas.SuspendLayout();
            pnlVentasTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            SuspendLayout();

            // ================ HEADER ================
            pnlHeader.BackColor = Color.FromArgb(24, 43, 73);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 62;
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblSubtitulo);

            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(16, 8);
            lblTitulo.Text = "Administración de Vehículos, Compradores y Consulta de Ventas";

            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = new Font("Segoe UI", 8.8F, FontStyle.Regular);
            lblSubtitulo.ForeColor = Color.FromArgb(205, 220, 240);
            lblSubtitulo.Location = new Point(16, 35);
            lblSubtitulo.Text = "Cliente Windows Forms (.NET) · Puerta única NGINX (:8090) · Autenticación JWT · Sin registro de ventas";

            // ================ AUTH PANEL ================
            grpAuth.Dock = DockStyle.Top;
            grpAuth.Height = 112;
            grpAuth.Padding = new Padding(10);
            grpAuth.Text = "Autenticación JWT (POST /oauth/token vía NGINX :8090 — el token se conserva solo en memoria)";

            lblGateway.AutoSize = true;
            lblGateway.Location = new Point(14, 28);
            lblGateway.Text = "Gateway NGINX:";

            txtGateway.Location = new Point(122, 24);
            txtGateway.Size = new Size(190, 24);
            txtGateway.Text = "http://127.0.0.1:8090";

            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(328, 28);
            lblUsuario.Text = "Usuario:";

            txtUsuario.Location = new Point(388, 24);
            txtUsuario.Size = new Size(140, 24);
            txtUsuario.PlaceholderText = "admin01 / operador01";

            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(542, 28);
            lblPassword.Text = "Contraseña:";

            txtPassword.Location = new Point(622, 24);
            txtPassword.Size = new Size(140, 24);
            txtPassword.UseSystemPasswordChar = true;

            lblClientSecret.AutoSize = true;
            lblClientSecret.Location = new Point(776, 28);
            lblClientSecret.Text = "Client Secret:";

            txtClientSecret.Location = new Point(866, 24);
            txtClientSecret.Size = new Size(190, 24);
            txtClientSecret.UseSystemPasswordChar = true;
            txtClientSecret.PlaceholderText = "(o env APE04_OAUTH_CLIENT_SECRET)";

            btnIniciarSesion.BackColor = Color.FromArgb(25, 135, 84);
            btnIniciarSesion.FlatStyle = FlatStyle.Flat;
            btnIniciarSesion.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnIniciarSesion.ForeColor = Color.White;
            btnIniciarSesion.Location = new Point(14, 62);
            btnIniciarSesion.Size = new Size(145, 32);
            btnIniciarSesion.Text = "Iniciar Sesión (JWT)";
            btnIniciarSesion.UseVisualStyleBackColor = false;

            btnCerrarSesion.BackColor = Color.FromArgb(108, 117, 125);
            btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            btnCerrarSesion.ForeColor = Color.White;
            btnCerrarSesion.Location = new Point(168, 62);
            btnCerrarSesion.Size = new Size(118, 32);
            btnCerrarSesion.Text = "Cerrar Sesión";
            btnCerrarSesion.UseVisualStyleBackColor = false;

            btnTokenInvalido.BackColor = Color.FromArgb(220, 53, 69);
            btnTokenInvalido.FlatStyle = FlatStyle.Flat;
            btnTokenInvalido.ForeColor = Color.White;
            btnTokenInvalido.Location = new Point(295, 62);
            btnTokenInvalido.Size = new Size(165, 32);
            btnTokenInvalido.Text = "Probar Token Inválido (401)";
            btnTokenInvalido.UseVisualStyleBackColor = false;

            lblSesion.AutoSize = true;
            lblSesion.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblSesion.ForeColor = Color.DarkSlateBlue;
            lblSesion.Location = new Point(478, 69);
            lblSesion.Text = "Sesión: No autenticado";

            grpAuth.Controls.AddRange(new Control[]
            {
                lblGateway, txtGateway, lblUsuario, txtUsuario,
                lblPassword, txtPassword, lblClientSecret, txtClientSecret,
                btnIniciarSesion, btnCerrarSesion, btnTokenInvalido, lblSesion
            });

            // ================ STATUS BAR ================
            pnlEstado.BackColor = Color.FromArgb(245, 247, 250);
            pnlEstado.Dock = DockStyle.Bottom;
            pnlEstado.Height = 36;
            pnlEstado.Padding = new Padding(12, 6, 12, 6);

            prgCarga.Dock = DockStyle.Right;
            prgCarga.Style = ProgressBarStyle.Marquee;
            prgCarga.Visible = false;
            prgCarga.Width = 140;

            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblEstado.ForeColor = Color.DarkSlateBlue;
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            lblEstado.Text = "Listo. Inicie sesión con sus credenciales para operar sobre los servicios vía NGINX (:8090).";

            pnlEstado.Controls.Add(lblEstado);
            pnlEstado.Controls.Add(prgCarga);

            // ================ TAB 1: VEHICULOS ================
            tabVehiculos.Padding = new Padding(10);
            tabVehiculos.Text = "1. Vehículos (Crear y Listar)";

            grpCrearVehiculo.Dock = DockStyle.Top;
            grpCrearVehiculo.Height = 145;
            grpCrearVehiculo.Text = "Registrar Vehículo (POST /api/vehiculos -> Spring Boot vía NGINX) y Consultar Catálogo (GET /api/vehiculos)";

            lblMarca.AutoSize = true;
            lblMarca.Location = new Point(14, 28);
            lblMarca.Text = "Marca:";
            txtMarca.Location = new Point(66, 24);
            txtMarca.Size = new Size(140, 24);
            txtMarca.PlaceholderText = "Ej. Toyota";

            lblModelo.AutoSize = true;
            lblModelo.Location = new Point(220, 28);
            lblModelo.Text = "Modelo:";
            txtModelo.Location = new Point(280, 24);
            txtModelo.Size = new Size(150, 24);
            txtModelo.PlaceholderText = "Ej. Corolla Cross";

            lblPlaca.AutoSize = true;
            lblPlaca.Location = new Point(446, 28);
            lblPlaca.Text = "Placa:";
            txtPlaca.Location = new Point(494, 24);
            txtPlaca.Size = new Size(110, 24);
            txtPlaca.PlaceholderText = "Ej. PBB4567";

            lblChasis.AutoSize = true;
            lblChasis.Location = new Point(618, 28);
            lblChasis.Text = "Chasis (17 chars):";
            txtChasis.Location = new Point(738, 24);
            txtChasis.Size = new Size(210, 24);
            txtChasis.MaxLength = 17;
            txtChasis.PlaceholderText = "1HGCM82633A123499";

            lblAnio.AutoSize = true;
            lblAnio.Location = new Point(14, 66);
            lblAnio.Text = "Año:";
            numAnio.Location = new Point(66, 62);
            numAnio.Size = new Size(90, 24);
            numAnio.Minimum = 1900;
            numAnio.Maximum = 2030;
            numAnio.Value = 2026;

            lblColor.AutoSize = true;
            lblColor.Location = new Point(176, 66);
            lblColor.Text = "Color:";
            txtColor.Location = new Point(226, 62);
            txtColor.Size = new Size(130, 24);
            txtColor.PlaceholderText = "Ej. Gris";

            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(374, 66);
            lblPrecio.Text = "Precio (USD):";
            numPrecio.Location = new Point(468, 62);
            numPrecio.Size = new Size(135, 24);
            numPrecio.DecimalPlaces = 2;
            numPrecio.Minimum = 1;
            numPrecio.Maximum = 10000000;
            numPrecio.Value = 25000;

            btnCrearVehiculo.BackColor = Color.FromArgb(25, 135, 84);
            btnCrearVehiculo.FlatStyle = FlatStyle.Flat;
            btnCrearVehiculo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCrearVehiculo.ForeColor = Color.White;
            btnCrearVehiculo.Location = new Point(14, 100);
            btnCrearVehiculo.Size = new Size(165, 32);
            btnCrearVehiculo.Text = "Crear Vehículo (POST)";
            btnCrearVehiculo.UseVisualStyleBackColor = false;

            btnListarVehiculos.BackColor = Color.FromArgb(13, 110, 253);
            btnListarVehiculos.FlatStyle = FlatStyle.Flat;
            btnListarVehiculos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnListarVehiculos.ForeColor = Color.White;
            btnListarVehiculos.Location = new Point(190, 100);
            btnListarVehiculos.Size = new Size(175, 32);
            btnListarVehiculos.Text = "Listar Vehículos (GET)";
            btnListarVehiculos.UseVisualStyleBackColor = false;

            btnLimpiarVehiculos.BackColor = Color.FromArgb(108, 117, 125);
            btnLimpiarVehiculos.FlatStyle = FlatStyle.Flat;
            btnLimpiarVehiculos.ForeColor = Color.White;
            btnLimpiarVehiculos.Location = new Point(376, 100);
            btnLimpiarVehiculos.Size = new Size(110, 32);
            btnLimpiarVehiculos.Text = "Limpiar";
            btnLimpiarVehiculos.UseVisualStyleBackColor = false;

            grpCrearVehiculo.Controls.AddRange(new Control[]
            {
                lblMarca, txtMarca, lblModelo, txtModelo, lblPlaca, txtPlaca, lblChasis, txtChasis,
                lblAnio, numAnio, lblColor, txtColor, lblPrecio, numPrecio,
                btnCrearVehiculo, btnListarVehiculos, btnLimpiarVehiculos
            });

            ConfigurarGrid(dgvVehiculos, "dgvVehiculos");
            tabVehiculos.Controls.Add(dgvVehiculos);
            tabVehiculos.Controls.Add(grpCrearVehiculo);

            // ================ TAB 2: COMPRADORES ================
            tabCompradores.Padding = new Padding(10);
            tabCompradores.Text = "2. Compradores (Crear y Listar)";

            grpCrearComprador.Dock = DockStyle.Top;
            grpCrearComprador.Height = 112;
            grpCrearComprador.Text = "Registrar Comprador (POST /api/compradores -> FastAPI vía NGINX) y Consultar Compradores (GET /api/compradores)";

            lblDocumento.AutoSize = true;
            lblDocumento.Location = new Point(14, 30);
            lblDocumento.Text = "Documento / Cédula:";
            txtDocumento.Location = new Point(150, 26);
            txtDocumento.Size = new Size(165, 24);
            txtDocumento.PlaceholderText = "Ej. 1805551122";

            lblNombreComp.AutoSize = true;
            lblNombreComp.Location = new Point(332, 30);
            lblNombreComp.Text = "Nombre completo:";
            txtNombreComp.Location = new Point(455, 26);
            txtNombreComp.Size = new Size(210, 24);
            txtNombreComp.PlaceholderText = "Ej. María López";

            lblCorreoComp.AutoSize = true;
            lblCorreoComp.Location = new Point(682, 30);
            lblCorreoComp.Text = "Correo:";
            txtCorreoComp.Location = new Point(738, 26);
            txtCorreoComp.Size = new Size(235, 24);
            txtCorreoComp.PlaceholderText = "maria.lopez@uta.edu.ec";

            btnCrearComprador.BackColor = Color.FromArgb(25, 135, 84);
            btnCrearComprador.FlatStyle = FlatStyle.Flat;
            btnCrearComprador.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCrearComprador.ForeColor = Color.White;
            btnCrearComprador.Location = new Point(14, 66);
            btnCrearComprador.Size = new Size(180, 32);
            btnCrearComprador.Text = "Crear Comprador (POST)";
            btnCrearComprador.UseVisualStyleBackColor = false;

            btnListarCompradores.BackColor = Color.FromArgb(13, 110, 253);
            btnListarCompradores.FlatStyle = FlatStyle.Flat;
            btnListarCompradores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnListarCompradores.ForeColor = Color.White;
            btnListarCompradores.Location = new Point(206, 66);
            btnListarCompradores.Size = new Size(185, 32);
            btnListarCompradores.Text = "Listar Compradores (GET)";
            btnListarCompradores.UseVisualStyleBackColor = false;

            btnLimpiarCompradores.BackColor = Color.FromArgb(108, 117, 125);
            btnLimpiarCompradores.FlatStyle = FlatStyle.Flat;
            btnLimpiarCompradores.ForeColor = Color.White;
            btnLimpiarCompradores.Location = new Point(402, 66);
            btnLimpiarCompradores.Size = new Size(110, 32);
            btnLimpiarCompradores.Text = "Limpiar";
            btnLimpiarCompradores.UseVisualStyleBackColor = false;

            grpCrearComprador.Controls.AddRange(new Control[]
            {
                lblDocumento, txtDocumento, lblNombreComp, txtNombreComp, lblCorreoComp, txtCorreoComp,
                btnCrearComprador, btnListarCompradores, btnLimpiarCompradores
            });

            ConfigurarGrid(dgvCompradores, "dgvCompradores");
            tabCompradores.Controls.Add(dgvCompradores);
            tabCompradores.Controls.Add(grpCrearComprador);

            // ================ TAB 3: VENTAS (SOLO LECTURA) ================
            tabVentas.Padding = new Padding(10);
            tabVentas.Text = "3. Ventas (Solo Consulta)";

            pnlVentasTop.Dock = DockStyle.Top;
            pnlVentasTop.Height = 64;

            lblAvisoVentas.AutoSize = true;
            lblAvisoVentas.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblAvisoVentas.ForeColor = Color.FromArgb(120, 40, 40);
            lblAvisoVentas.Location = new Point(6, 4);
            lblAvisoVentas.Text = "Módulo administrativo de solo lectura (GET /api/ventas -> FastAPI vía NGINX). El registro de ventas es exclusivo de la PWA móvil.";

            btnListarVentas.BackColor = Color.FromArgb(13, 110, 253);
            btnListarVentas.FlatStyle = FlatStyle.Flat;
            btnListarVentas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnListarVentas.ForeColor = Color.White;
            btnListarVentas.Location = new Point(6, 26);
            btnListarVentas.Size = new Size(175, 32);
            btnListarVentas.Text = "Listar Ventas (GET)";
            btnListarVentas.UseVisualStyleBackColor = false;

            btnLimpiarVentas.BackColor = Color.FromArgb(108, 117, 125);
            btnLimpiarVentas.FlatStyle = FlatStyle.Flat;
            btnLimpiarVentas.ForeColor = Color.White;
            btnLimpiarVentas.Location = new Point(192, 26);
            btnLimpiarVentas.Size = new Size(110, 32);
            btnLimpiarVentas.Text = "Limpiar";
            btnLimpiarVentas.UseVisualStyleBackColor = false;

            pnlVentasTop.Controls.AddRange(new Control[] { lblAvisoVentas, btnListarVentas, btnLimpiarVentas });

            ConfigurarGrid(dgvVentas, "dgvVentas");
            tabVentas.Controls.Add(dgvVentas);
            tabVentas.Controls.Add(pnlVentasTop);

            // ================ TAB CONTROL ================
            tabPrincipal.Dock = DockStyle.Fill;
            tabPrincipal.Font = new Font("Segoe UI", 9.5F);
            tabPrincipal.Controls.Add(tabVehiculos);
            tabPrincipal.Controls.Add(tabCompradores);
            tabPrincipal.Controls.Add(tabVentas);

            // ================ FORM ================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1090, 680);
            MinimumSize = new Size(1020, 620);
            Font = new Font("Segoe UI", 9.5F);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "APE 04 — Administración Windows Forms (Vehículos, Compradores y Consulta de Ventas)";

            Controls.Add(tabPrincipal);
            Controls.Add(grpAuth);
            Controls.Add(pnlHeader);
            Controls.Add(pnlEstado);

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            grpAuth.ResumeLayout(false);
            grpAuth.PerformLayout();
            pnlEstado.ResumeLayout(false);
            tabPrincipal.ResumeLayout(false);
            tabVehiculos.ResumeLayout(false);
            grpCrearVehiculo.ResumeLayout(false);
            grpCrearVehiculo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAnio).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).EndInit();
            tabCompradores.ResumeLayout(false);
            grpCrearComprador.ResumeLayout(false);
            grpCrearComprador.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCompradores).EndInit();
            tabVentas.ResumeLayout(false);
            pnlVentasTop.ResumeLayout(false);
            pnlVentasTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            ResumeLayout(false);
        }

        private static void ConfigurarGrid(DataGridView dgv, string name)
        {
            dgv.Name = name;
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeight = 30;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
        }

        #endregion

        private Panel pnlHeader = null!;
        private Label lblTitulo = null!;
        private Label lblSubtitulo = null!;

        private GroupBox grpAuth = null!;
        private Label lblGateway = null!;
        private TextBox txtGateway = null!;
        private Label lblUsuario = null!;
        private TextBox txtUsuario = null!;
        private Label lblPassword = null!;
        private TextBox txtPassword = null!;
        private Label lblClientSecret = null!;
        private TextBox txtClientSecret = null!;
        private Button btnIniciarSesion = null!;
        private Button btnCerrarSesion = null!;
        private Button btnTokenInvalido = null!;
        private Label lblSesion = null!;

        private Panel pnlEstado = null!;
        private Label lblEstado = null!;
        private ProgressBar prgCarga = null!;

        private TabControl tabPrincipal = null!;

        private TabPage tabVehiculos = null!;
        private GroupBox grpCrearVehiculo = null!;
        private Label lblMarca = null!;
        private TextBox txtMarca = null!;
        private Label lblModelo = null!;
        private TextBox txtModelo = null!;
        private Label lblPlaca = null!;
        private TextBox txtPlaca = null!;
        private Label lblChasis = null!;
        private TextBox txtChasis = null!;
        private Label lblAnio = null!;
        private NumericUpDown numAnio = null!;
        private Label lblColor = null!;
        private TextBox txtColor = null!;
        private Label lblPrecio = null!;
        private NumericUpDown numPrecio = null!;
        private Button btnCrearVehiculo = null!;
        private Button btnListarVehiculos = null!;
        private Button btnLimpiarVehiculos = null!;
        private DataGridView dgvVehiculos = null!;

        private TabPage tabCompradores = null!;
        private GroupBox grpCrearComprador = null!;
        private Label lblDocumento = null!;
        private TextBox txtDocumento = null!;
        private Label lblNombreComp = null!;
        private TextBox txtNombreComp = null!;
        private Label lblCorreoComp = null!;
        private TextBox txtCorreoComp = null!;
        private Button btnCrearComprador = null!;
        private Button btnListarCompradores = null!;
        private Button btnLimpiarCompradores = null!;
        private DataGridView dgvCompradores = null!;

        private TabPage tabVentas = null!;
        private Panel pnlVentasTop = null!;
        private Label lblAvisoVentas = null!;
        private Button btnListarVentas = null!;
        private Button btnLimpiarVentas = null!;
        private DataGridView dgvVentas = null!;
    }
}
