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
            // ==================== PANTALLA 1: LOGIN OSCURO NEXORA ====================
            pnlLoginScreen = new Panel();
            pnlLoginLeft = new Panel();
            pnlLoginCard = new Panel();
            picLoginLogo = new PictureBox();
            lblLoginTitle = new Label();
            lblLoginSub = new Label();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            chkMostrarPass = new CheckBox();
            lblGateway = new Label();
            txtGateway = new TextBox();
            lblClientSecret = new Label();
            txtClientSecret = new TextBox();
            lblLoginError = new Label();
            btnIniciarSesion = new Button();
            btnTokenInvalidoLogin = new Button();
            lblLoginFooter = new Label();

            pnlLoginRight = new CoverImagePanel();
            lblVisualTag = new Label();
            lblVisualCaption = new Label();

            // ==================== PANTALLA 2: WORKSPACE ADMINISTRATIVO ====================
            pnlAppWorkspace = new Panel();
            pnlSidebar = new Panel();
            pnlBrand = new Panel();
            picSidebarLogo = new PictureBox();
            lblBrandPill = new Label();
            lblNavTitle = new Label();

            pnlNavVehiculos = new Panel();
            pnlIndVehiculos = new Panel();
            btnNavVehiculos = new Button();

            pnlNavCompradores = new Panel();
            pnlIndCompradores = new Panel();
            btnNavCompradores = new Button();

            pnlNavVentas = new Panel();
            pnlIndVentas = new Panel();
            btnNavVentas = new Button();

            pnlSidebarFooter = new Panel();
            lblSesion = new Label();
            btnTokenInvalido = new Button();
            btnCerrarSesion = new Button();
            lblSidebarNota = new Label();

            pnlRightArea = new Panel();
            pnlTopbar = new Panel();
            lblTopTitle = new Label();
            lblTopSub = new Label();
            lblTopBadge = new Label();
            lblTopKpi = new Label();

            pnlContentHost = new Panel();

            // Módulo 1: Vehículos
            pnlModVehiculos = new Panel();
            pnlFormVehiculo = new Panel();
            lblFormVehTitle = new Label();
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
            pnlGridVehWrap = new Panel();
            dgvVehiculos = new DataGridView();

            // Módulo 2: Compradores
            pnlModCompradores = new Panel();
            pnlFormComprador = new Panel();
            lblFormCompTitle = new Label();
            lblDocumento = new Label();
            txtDocumento = new TextBox();
            lblNombreComp = new Label();
            txtNombreComp = new TextBox();
            lblCorreoComp = new Label();
            txtCorreoComp = new TextBox();
            btnCrearComprador = new Button();
            btnListarCompradores = new Button();
            btnLimpiarCompradores = new Button();
            pnlGridCompWrap = new Panel();
            dgvCompradores = new DataGridView();

            // Módulo 3: Ventas (Solo lectura)
            pnlModVentas = new Panel();
            pnlVentasBar = new Panel();
            lblAvisoVentas = new Label();
            btnListarVentas = new Button();
            btnLimpiarVentas = new Button();
            pnlGridVenWrap = new Panel();
            dgvVentas = new DataGridView();

            // Barra inferior de estado
            pnlEstado = new Panel();
            lblEstado = new Label();
            prgCarga = new ProgressBar();

            ((System.ComponentModel.ISupportInitialize)picLoginLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSidebarLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAnio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCompradores).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            SuspendLayout();

            // ================ PANTALLA 1: LOGIN OSCURO (IZQ: LOGO + DATOS · DER: IMAGEN) ================
            pnlLoginScreen.Dock = DockStyle.Fill;
            pnlLoginScreen.BackColor = Theme.Graphite920;

            pnlLoginLeft.Dock = DockStyle.Left;
            pnlLoginLeft.Width = 500;
            pnlLoginLeft.BackColor = Theme.Graphite920;

            pnlLoginCard.Size = new Size(390, 620);
            pnlLoginCard.Location = new Point(55, 36);
            pnlLoginCard.BackColor = Theme.Graphite920;

            picLoginLogo.Size = new Size(250, 108);
            picLoginLogo.Location = new Point(70, 0);
            picLoginLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLoginLogo.BackColor = Color.Transparent;

            lblLoginTitle.AutoSize = true;
            lblLoginTitle.Font = Theme.FontLoginTitle;
            lblLoginTitle.ForeColor = Color.White;
            lblLoginTitle.Location = new Point(0, 118);
            lblLoginTitle.Text = "Iniciar sesión";

            lblLoginSub.AutoSize = false;
            lblLoginSub.Size = new Size(390, 36);
            lblLoginSub.Font = Theme.FontSmall;
            lblLoginSub.ForeColor = Theme.TextOnDarkMuted;
            lblLoginSub.Location = new Point(0, 154);
            lblLoginSub.Text = "Panel administrativo Windows Forms · Gestión de vehículos, compradores y consulta de ventas.";

            lblUsuario.AutoSize = true;
            lblUsuario.Font = Theme.FontSmallBold;
            lblUsuario.ForeColor = Color.FromArgb(203, 213, 225);
            lblUsuario.Location = new Point(0, 200);
            lblUsuario.Text = "Usuario";
            txtUsuario.Location = new Point(0, 220);
            txtUsuario.Size = new Size(390, 28);
            txtUsuario.PlaceholderText = "Ej. admin01 / operador01";
            Theme.StyleDarkInput(txtUsuario);

            lblPassword.AutoSize = true;
            lblPassword.Font = Theme.FontSmallBold;
            lblPassword.ForeColor = Color.FromArgb(203, 213, 225);
            lblPassword.Location = new Point(0, 258);
            lblPassword.Text = "Contraseña";
            txtPassword.Location = new Point(0, 278);
            txtPassword.Size = new Size(390, 28);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.PlaceholderText = "Ingrese su contraseña";
            Theme.StyleDarkInput(txtPassword);

            chkMostrarPass.AutoSize = true;
            chkMostrarPass.ForeColor = Theme.TextOnDarkMuted;
            chkMostrarPass.Font = Theme.FontSmall;
            chkMostrarPass.Location = new Point(0, 310);
            chkMostrarPass.Text = "Mostrar contraseña";

            lblGateway.AutoSize = true;
            lblGateway.Font = Theme.FontSmallBold;
            lblGateway.ForeColor = Color.FromArgb(203, 213, 225);
            lblGateway.Location = new Point(0, 340);
            lblGateway.Text = "Gateway NGINX";
            txtGateway.Location = new Point(0, 360);
            txtGateway.Size = new Size(188, 28);
            txtGateway.Text = "http://127.0.0.1:8090";
            Theme.StyleDarkInput(txtGateway);

            lblClientSecret.AutoSize = true;
            lblClientSecret.Font = Theme.FontSmallBold;
            lblClientSecret.ForeColor = Color.FromArgb(203, 213, 225);
            lblClientSecret.Location = new Point(202, 340);
            lblClientSecret.Text = "Client Secret (o env)";
            txtClientSecret.Location = new Point(202, 360);
            txtClientSecret.Size = new Size(188, 28);
            txtClientSecret.UseSystemPasswordChar = true;
            txtClientSecret.PlaceholderText = "Opcional si usa env";
            Theme.StyleDarkInput(txtClientSecret);

            lblLoginError.AutoSize = false;
            lblLoginError.Size = new Size(390, 42);
            lblLoginError.Location = new Point(0, 398);
            lblLoginError.Font = Theme.FontSmallBold;
            lblLoginError.ForeColor = Color.FromArgb(254, 202, 202);
            lblLoginError.BackColor = Color.FromArgb(52, 22, 26);
            lblLoginError.Padding = new Padding(10, 6, 10, 6);
            lblLoginError.Visible = false;

            btnIniciarSesion.Location = new Point(0, 450);
            btnIniciarSesion.Size = new Size(390, 44);
            btnIniciarSesion.Text = "Iniciar sesión";
            Theme.StyleCopperButton(btnIniciarSesion);

            btnTokenInvalidoLogin.Location = new Point(0, 504);
            btnTokenInvalidoLogin.Size = new Size(390, 34);
            btnTokenInvalidoLogin.Text = "Probar JWT inválido (HTTP 401)";
            Theme.StyleDarkButton(btnTokenInvalidoLogin, danger: true);

            lblLoginFooter.AutoSize = false;
            lblLoginFooter.Size = new Size(390, 38);
            lblLoginFooter.Location = new Point(0, 550);
            lblLoginFooter.Font = new Font("Segoe UI", 8F);
            lblLoginFooter.ForeColor = Theme.Ink500;
            lblLoginFooter.TextAlign = ContentAlignment.MiddleCenter;
            lblLoginFooter.Text = "POST /oauth/token vía NGINX (:8090) · JWT solo en memoria\nEl registro de ventas es exclusivo de la PWA móvil";

            pnlLoginCard.Controls.AddRange(new Control[]
            {
                picLoginLogo, lblLoginTitle, lblLoginSub,
                lblUsuario, txtUsuario, lblPassword, txtPassword, chkMostrarPass,
                lblGateway, txtGateway, lblClientSecret, txtClientSecret,
                lblLoginError, btnIniciarSesion, btnTokenInvalidoLogin, lblLoginFooter
            });
            pnlLoginLeft.Controls.Add(pnlLoginCard);

            // Lado derecho: Fotografía automotriz Cover + pie editorial
            pnlLoginRight.Dock = DockStyle.Fill;

            lblVisualTag.AutoSize = true;
            lblVisualTag.BackColor = Color.Transparent;
            lblVisualTag.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblVisualTag.ForeColor = Theme.Copper400;
            lblVisualTag.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblVisualTag.Location = new Point(38, 560);
            lblVisualTag.Text = "NEXORA AUTOS · APE 04";

            lblVisualCaption.AutoSize = false;
            lblVisualCaption.BackColor = Color.Transparent;
            lblVisualCaption.Size = new Size(460, 64);
            lblVisualCaption.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            lblVisualCaption.ForeColor = Color.White;
            lblVisualCaption.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblVisualCaption.Location = new Point(36, 586);
            lblVisualCaption.Text = "Administración de inventario, compradores y trazabilidad comercial.";

            pnlLoginRight.Controls.Add(lblVisualTag);
            pnlLoginRight.Controls.Add(lblVisualCaption);

            pnlLoginScreen.Controls.Add(pnlLoginRight);
            pnlLoginScreen.Controls.Add(pnlLoginLeft);

            // ================ PANTALLA 2: WORKSPACE ADMINISTRATIVO ================
            pnlAppWorkspace.Dock = DockStyle.Fill;
            pnlAppWorkspace.BackColor = Theme.Canvas;
            pnlAppWorkspace.Visible = false;

            // Sidebar Grafito #0B0F17
            pnlSidebar.BackColor = Theme.Graphite950;
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 248;

            pnlBrand.Dock = DockStyle.Top;
            pnlBrand.Height = 114;
            pnlBrand.BackColor = Theme.Graphite950;

            picSidebarLogo.Size = new Size(194, 72);
            picSidebarLogo.Location = new Point(26, 12);
            picSidebarLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picSidebarLogo.BackColor = Color.Transparent;

            lblBrandPill.AutoSize = false;
            lblBrandPill.Size = new Size(186, 22);
            lblBrandPill.Location = new Point(30, 86);
            lblBrandPill.Font = new Font("Segoe UI", 7.8F, FontStyle.Bold);
            lblBrandPill.ForeColor = Theme.Copper400;
            lblBrandPill.BackColor = Color.FromArgb(40, 24, 18);
            lblBrandPill.TextAlign = ContentAlignment.MiddleCenter;
            lblBrandPill.Text = "ADMIN WINFORMS · APE 04";

            pnlBrand.Controls.Add(picSidebarLogo);
            pnlBrand.Controls.Add(lblBrandPill);

            lblNavTitle.Dock = DockStyle.Top;
            lblNavTitle.Height = 32;
            lblNavTitle.Padding = new Padding(20, 12, 0, 0);
            lblNavTitle.Font = new Font("Segoe UI", 7.8F, FontStyle.Bold);
            lblNavTitle.ForeColor = Theme.Ink500;
            lblNavTitle.Text = "PLATAFORMA";

            ConfigurarNavItem(pnlNavVehiculos, pnlIndVehiculos, btnNavVehiculos, "  Vehículos", true);
            ConfigurarNavItem(pnlNavCompradores, pnlIndCompradores, btnNavCompradores, "  Compradores", false);
            ConfigurarNavItem(pnlNavVentas, pnlIndVentas, btnNavVentas, "  Ventas (Solo consulta)", false);

            pnlSidebarFooter.Dock = DockStyle.Bottom;
            pnlSidebarFooter.Height = 166;
            pnlSidebarFooter.BackColor = Theme.Graphite950;

            lblSesion.AutoSize = false;
            lblSesion.Size = new Size(216, 30);
            lblSesion.Location = new Point(16, 10);
            lblSesion.Font = Theme.FontSmallBold;
            lblSesion.ForeColor = Color.FromArgb(226, 232, 240);
            lblSesion.BackColor = Theme.Graphite900;
            lblSesion.TextAlign = ContentAlignment.MiddleCenter;
            lblSesion.Text = "Sin sesión activa";

            btnTokenInvalido.Location = new Point(16, 48);
            btnTokenInvalido.Size = new Size(216, 30);
            btnTokenInvalido.Text = "Probar JWT inválido (401)";
            Theme.StyleDarkButton(btnTokenInvalido, danger: true);

            btnCerrarSesion.Location = new Point(16, 84);
            btnCerrarSesion.Size = new Size(216, 30);
            btnCerrarSesion.Text = "Cerrar sesión";
            Theme.StyleDarkButton(btnCerrarSesion, danger: false);

            lblSidebarNota.AutoSize = false;
            lblSidebarNota.Size = new Size(216, 38);
            lblSidebarNota.Location = new Point(16, 122);
            lblSidebarNota.Font = new Font("Segoe UI", 7.8F);
            lblSidebarNota.ForeColor = Theme.Ink500;
            lblSidebarNota.Text = "Gateway único NGINX (:8090)\nVentas exclusivas en PWA móvil";

            pnlSidebarFooter.Controls.AddRange(new Control[]
            {
                lblSesion, btnTokenInvalido, btnCerrarSesion, lblSidebarNota
            });

            pnlSidebar.Controls.Add(pnlSidebarFooter);
            pnlSidebar.Controls.Add(pnlNavVentas);
            pnlSidebar.Controls.Add(pnlNavCompradores);
            pnlSidebar.Controls.Add(pnlNavVehiculos);
            pnlSidebar.Controls.Add(lblNavTitle);
            pnlSidebar.Controls.Add(pnlBrand);

            // Topbar blanca limpia
            pnlRightArea.Dock = DockStyle.Fill;
            pnlRightArea.BackColor = Theme.Canvas;

            pnlTopbar.Dock = DockStyle.Top;
            pnlTopbar.Height = 72;
            pnlTopbar.BackColor = Theme.Surface;
            pnlTopbar.Paint += DibujarBordeInferior;

            lblTopTitle.AutoSize = true;
            lblTopTitle.Font = Theme.FontPageTitle;
            lblTopTitle.ForeColor = Theme.Ink950;
            lblTopTitle.Location = new Point(22, 12);
            lblTopTitle.Text = "Gestión de Vehículos";

            lblTopSub.AutoSize = true;
            lblTopSub.Font = Theme.FontSmall;
            lblTopSub.ForeColor = Theme.Ink500;
            lblTopSub.Location = new Point(24, 42);
            lblTopSub.Text = "POST /api/vehiculos y GET /api/vehiculos (Spring Boot vía NGINX :8090)";

            lblTopBadge.AutoSize = true;
            lblTopBadge.Font = Theme.FontSmallBold;
            lblTopBadge.BackColor = Theme.Graphite950;
            lblTopBadge.ForeColor = Color.White;
            lblTopBadge.Padding = new Padding(10, 5, 10, 5);
            lblTopBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTopBadge.Location = new Point(620, 12);
            lblTopBadge.Text = "Sin registro de ventas";

            lblTopKpi.AutoSize = true;
            lblTopKpi.Font = Theme.FontSmallBold;
            lblTopKpi.BackColor = Theme.Stone100;
            lblTopKpi.ForeColor = Theme.Ink900;
            lblTopKpi.Padding = new Padding(10, 4, 10, 4);
            lblTopKpi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTopKpi.Location = new Point(620, 40);
            lblTopKpi.Text = "0 registros";

            pnlTopbar.Controls.AddRange(new Control[] { lblTopTitle, lblTopSub, lblTopBadge, lblTopKpi });

            // Barra inferior de estado
            pnlEstado.BackColor = Theme.Surface;
            pnlEstado.Dock = DockStyle.Bottom;
            pnlEstado.Height = 38;
            pnlEstado.Padding = new Padding(22, 6, 22, 6);
            pnlEstado.Paint += DibujarBordeSuperior;

            prgCarga.Dock = DockStyle.Right;
            prgCarga.Style = ProgressBarStyle.Marquee;
            prgCarga.Visible = false;
            prgCarga.Width = 130;

            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = Theme.FontSmallBold;
            lblEstado.ForeColor = Theme.Ink700;
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;
            lblEstado.Text = "Listo para operar vía NGINX (:8090).";

            pnlEstado.Controls.Add(lblEstado);
            pnlEstado.Controls.Add(prgCarga);

            pnlContentHost.Dock = DockStyle.Fill;
            pnlContentHost.Padding = new Padding(22, 18, 22, 16);
            pnlContentHost.BackColor = Theme.Canvas;

            // ================ MÓDULO 1: VEHÍCULOS ================
            pnlModVehiculos.Dock = DockStyle.Fill;
            pnlModVehiculos.BackColor = Theme.Canvas;

            pnlFormVehiculo.Dock = DockStyle.Top;
            pnlFormVehiculo.Height = 162;
            pnlFormVehiculo.BackColor = Theme.Surface;
            pnlFormVehiculo.Paint += DibujarBordeTarjeta;

            lblFormVehTitle.AutoSize = true;
            lblFormVehTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblFormVehTitle.ForeColor = Theme.Ink500;
            lblFormVehTitle.Location = new Point(18, 12);
            lblFormVehTitle.Text = "REGISTRAR NUEVO VEHÍCULO (POST /api/vehiculos)";

            lblMarca.AutoSize = true;
            lblMarca.Font = Theme.FontSmallBold;
            lblMarca.ForeColor = Theme.Ink700;
            lblMarca.Location = new Point(18, 36);
            lblMarca.Text = "Marca";
            txtMarca.Location = new Point(18, 54);
            txtMarca.Size = new Size(165, 25);
            txtMarca.PlaceholderText = "Ej. Toyota";

            lblModelo.AutoSize = true;
            lblModelo.Font = Theme.FontSmallBold;
            lblModelo.ForeColor = Theme.Ink700;
            lblModelo.Location = new Point(196, 36);
            lblModelo.Text = "Modelo";
            txtModelo.Location = new Point(196, 54);
            txtModelo.Size = new Size(175, 25);
            txtModelo.PlaceholderText = "Ej. Corolla Cross";

            lblPlaca.AutoSize = true;
            lblPlaca.Font = Theme.FontSmallBold;
            lblPlaca.ForeColor = Theme.Ink700;
            lblPlaca.Location = new Point(384, 36);
            lblPlaca.Text = "Placa";
            txtPlaca.Location = new Point(384, 54);
            txtPlaca.Size = new Size(125, 25);
            txtPlaca.PlaceholderText = "PBB-4567";

            lblChasis.AutoSize = true;
            lblChasis.Font = Theme.FontSmallBold;
            lblChasis.ForeColor = Theme.Ink700;
            lblChasis.Location = new Point(522, 36);
            lblChasis.Text = "Chasis (17 caracteres)";
            txtChasis.Location = new Point(522, 54);
            txtChasis.Size = new Size(235, 25);
            txtChasis.MaxLength = 17;
            txtChasis.PlaceholderText = "1HGCM82633A123499";

            lblAnio.AutoSize = true;
            lblAnio.Font = Theme.FontSmallBold;
            lblAnio.ForeColor = Theme.Ink700;
            lblAnio.Location = new Point(18, 92);
            lblAnio.Text = "Año";
            numAnio.Location = new Point(18, 110);
            numAnio.Size = new Size(92, 25);
            numAnio.Minimum = 1900;
            numAnio.Maximum = 2030;
            numAnio.Value = 2026;

            lblColor.AutoSize = true;
            lblColor.Font = Theme.FontSmallBold;
            lblColor.ForeColor = Theme.Ink700;
            lblColor.Location = new Point(124, 92);
            lblColor.Text = "Color";
            txtColor.Location = new Point(124, 110);
            txtColor.Size = new Size(140, 25);
            txtColor.PlaceholderText = "Ej. Grafito";

            lblPrecio.AutoSize = true;
            lblPrecio.Font = Theme.FontSmallBold;
            lblPrecio.ForeColor = Theme.Ink700;
            lblPrecio.Location = new Point(278, 92);
            lblPrecio.Text = "Precio (USD)";
            numPrecio.Location = new Point(278, 110);
            numPrecio.Size = new Size(140, 25);
            numPrecio.DecimalPlaces = 2;
            numPrecio.Minimum = 1;
            numPrecio.Maximum = 10000000;
            numPrecio.Value = 25000;

            btnCrearVehiculo.Location = new Point(436, 104);
            btnCrearVehiculo.Size = new Size(165, 34);
            btnCrearVehiculo.Text = "Agregar vehículo";
            Theme.StyleCopperButton(btnCrearVehiculo);

            btnListarVehiculos.Location = new Point(611, 104);
            btnListarVehiculos.Size = new Size(145, 34);
            btnListarVehiculos.Text = "Listar inventario";
            Theme.StyleSecondaryButton(btnListarVehiculos);

            btnLimpiarVehiculos.Location = new Point(766, 104);
            btnLimpiarVehiculos.Size = new Size(94, 34);
            btnLimpiarVehiculos.Text = "Limpiar";
            Theme.StyleSecondaryButton(btnLimpiarVehiculos);

            pnlFormVehiculo.Controls.AddRange(new Control[]
            {
                lblFormVehTitle, lblMarca, txtMarca, lblModelo, txtModelo, lblPlaca, txtPlaca, lblChasis, txtChasis,
                lblAnio, numAnio, lblColor, txtColor, lblPrecio, numPrecio,
                btnCrearVehiculo, btnListarVehiculos, btnLimpiarVehiculos
            });

            pnlGridVehWrap.Dock = DockStyle.Fill;
            pnlGridVehWrap.Padding = new Padding(0, 14, 0, 0);
            pnlGridVehWrap.BackColor = Theme.Canvas;
            Theme.StyleGrid(dgvVehiculos, "dgvVehiculos");
            pnlGridVehWrap.Controls.Add(dgvVehiculos);

            pnlModVehiculos.Controls.Add(pnlGridVehWrap);
            pnlModVehiculos.Controls.Add(pnlFormVehiculo);

            // ================ MÓDULO 2: COMPRADORES ================
            pnlModCompradores.Dock = DockStyle.Fill;
            pnlModCompradores.BackColor = Theme.Canvas;
            pnlModCompradores.Visible = false;

            pnlFormComprador.Dock = DockStyle.Top;
            pnlFormComprador.Height = 136;
            pnlFormComprador.BackColor = Theme.Surface;
            pnlFormComprador.Paint += DibujarBordeTarjeta;

            lblFormCompTitle.AutoSize = true;
            lblFormCompTitle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblFormCompTitle.ForeColor = Theme.Ink500;
            lblFormCompTitle.Location = new Point(18, 12);
            lblFormCompTitle.Text = "REGISTRAR NUEVO COMPRADOR (POST /api/compradores)";

            lblDocumento.AutoSize = true;
            lblDocumento.Font = Theme.FontSmallBold;
            lblDocumento.ForeColor = Theme.Ink700;
            lblDocumento.Location = new Point(18, 36);
            lblDocumento.Text = "Documento / Cédula";
            txtDocumento.Location = new Point(18, 54);
            txtDocumento.Size = new Size(185, 25);
            txtDocumento.PlaceholderText = "Ej. 1805551122";

            lblNombreComp.AutoSize = true;
            lblNombreComp.Font = Theme.FontSmallBold;
            lblNombreComp.ForeColor = Theme.Ink700;
            lblNombreComp.Location = new Point(216, 36);
            lblNombreComp.Text = "Nombre completo";
            txtNombreComp.Location = new Point(216, 54);
            txtNombreComp.Size = new Size(255, 25);
            txtNombreComp.PlaceholderText = "Ej. María López";

            lblCorreoComp.AutoSize = true;
            lblCorreoComp.Font = Theme.FontSmallBold;
            lblCorreoComp.ForeColor = Theme.Ink700;
            lblCorreoComp.Location = new Point(484, 36);
            lblCorreoComp.Text = "Correo electrónico";
            txtCorreoComp.Location = new Point(484, 54);
            txtCorreoComp.Size = new Size(275, 25);
            txtCorreoComp.PlaceholderText = "maria.lopez@uta.edu.ec";

            btnCrearComprador.Location = new Point(18, 88);
            btnCrearComprador.Size = new Size(175, 34);
            btnCrearComprador.Text = "Agregar comprador";
            Theme.StyleCopperButton(btnCrearComprador);

            btnListarCompradores.Location = new Point(203, 88);
            btnListarCompradores.Size = new Size(165, 34);
            btnListarCompradores.Text = "Listar compradores";
            Theme.StyleSecondaryButton(btnListarCompradores);

            btnLimpiarCompradores.Location = new Point(378, 88);
            btnLimpiarCompradores.Size = new Size(94, 34);
            btnLimpiarCompradores.Text = "Limpiar";
            Theme.StyleSecondaryButton(btnLimpiarCompradores);

            pnlFormComprador.Controls.AddRange(new Control[]
            {
                lblFormCompTitle, lblDocumento, txtDocumento, lblNombreComp, txtNombreComp, lblCorreoComp, txtCorreoComp,
                btnCrearComprador, btnListarCompradores, btnLimpiarCompradores
            });

            pnlGridCompWrap.Dock = DockStyle.Fill;
            pnlGridCompWrap.Padding = new Padding(0, 14, 0, 0);
            pnlGridCompWrap.BackColor = Theme.Canvas;
            Theme.StyleGrid(dgvCompradores, "dgvCompradores");
            pnlGridCompWrap.Controls.Add(dgvCompradores);

            pnlModCompradores.Controls.Add(pnlGridCompWrap);
            pnlModCompradores.Controls.Add(pnlFormComprador);

            // ================ MÓDULO 3: VENTAS (SOLO CONSULTA) ================
            pnlModVentas.Dock = DockStyle.Fill;
            pnlModVentas.BackColor = Theme.Canvas;
            pnlModVentas.Visible = false;

            pnlVentasBar.Dock = DockStyle.Top;
            pnlVentasBar.Height = 84;
            pnlVentasBar.BackColor = Theme.Surface;
            pnlVentasBar.Paint += DibujarBordeTarjeta;

            lblAvisoVentas.AutoSize = true;
            lblAvisoVentas.Font = Theme.FontSmallBold;
            lblAvisoVentas.ForeColor = Theme.Ink700;
            lblAvisoVentas.Location = new Point(18, 13);
            lblAvisoVentas.Text = "Módulo administrativo de solo consulta (GET /api/ventas): el registro de nuevas ventas es exclusivo de la PWA móvil.";

            btnListarVentas.Location = new Point(18, 38);
            btnListarVentas.Size = new Size(165, 34);
            btnListarVentas.Text = "Consultar ventas";
            Theme.StyleCopperButton(btnListarVentas);

            btnLimpiarVentas.Location = new Point(193, 38);
            btnLimpiarVentas.Size = new Size(94, 34);
            btnLimpiarVentas.Text = "Limpiar";
            Theme.StyleSecondaryButton(btnLimpiarVentas);

            pnlVentasBar.Controls.AddRange(new Control[] { lblAvisoVentas, btnListarVentas, btnLimpiarVentas });

            pnlGridVenWrap.Dock = DockStyle.Fill;
            pnlGridVenWrap.Padding = new Padding(0, 14, 0, 0);
            pnlGridVenWrap.BackColor = Theme.Canvas;
            Theme.StyleGrid(dgvVentas, "dgvVentas");
            pnlGridVenWrap.Controls.Add(dgvVentas);

            pnlModVentas.Controls.Add(pnlGridVenWrap);
            pnlModVentas.Controls.Add(pnlVentasBar);

            pnlContentHost.Controls.Add(pnlModVentas);
            pnlContentHost.Controls.Add(pnlModCompradores);
            pnlContentHost.Controls.Add(pnlModVehiculos);

            pnlRightArea.Controls.Add(pnlContentHost);
            pnlRightArea.Controls.Add(pnlTopbar);
            pnlRightArea.Controls.Add(pnlEstado);

            pnlAppWorkspace.Controls.Add(pnlRightArea);
            pnlAppWorkspace.Controls.Add(pnlSidebar);

            // ================ FORMULARIO RAÍZ ================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Theme.Graphite920;
            ClientSize = new Size(1220, 720);
            MinimumSize = new Size(1120, 650);
            Font = Theme.FontBody;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NEXORA Autos Admin — Windows Forms (Vehículos, Compradores y Consulta de Ventas)";

            Controls.Add(pnlAppWorkspace);
            Controls.Add(pnlLoginScreen);

            ((System.ComponentModel.ISupportInitialize)picLoginLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSidebarLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAnio).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrecio).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCompradores).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            ResumeLayout(false);
        }

        private static void ConfigurarNavItem(Panel container, Panel indicator, Button button, string text, bool active)
        {
            container.Dock = DockStyle.Top;
            container.Height = 44;
            container.BackColor = active ? Theme.Graphite900 : Theme.Graphite950;

            indicator.Dock = DockStyle.Left;
            indicator.Width = 4;
            indicator.BackColor = active ? Theme.Copper600 : Color.Transparent;

            button.Dock = DockStyle.Fill;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Theme.Graphite850;
            button.FlatAppearance.MouseDownBackColor = Theme.Graphite800;
            button.BackColor = active ? Theme.Graphite900 : Theme.Graphite950;
            button.ForeColor = active ? Color.White : Theme.TextOnDarkMuted;
            button.Font = Theme.FontBodyBold;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(14, 0, 0, 0);
            button.Text = text;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            container.Controls.Add(button);
            container.Controls.Add(indicator);
        }

        private static void DibujarBordeTarjeta(object sender, PaintEventArgs e)
        {
            if (sender is Control ctrl)
            {
                using var pen = new Pen(Theme.Stone150, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, ctrl.Width - 1, ctrl.Height - 1);
            }
        }

        private static void DibujarBordeInferior(object sender, PaintEventArgs e)
        {
            if (sender is Control ctrl)
            {
                using var pen = new Pen(Theme.Stone150, 1);
                e.Graphics.DrawLine(pen, 0, ctrl.Height - 1, ctrl.Width, ctrl.Height - 1);
            }
        }

        private static void DibujarBordeSuperior(object sender, PaintEventArgs e)
        {
            if (sender is Control ctrl)
            {
                using var pen = new Pen(Theme.Stone150, 1);
                e.Graphics.DrawLine(pen, 0, 0, ctrl.Width, 0);
            }
        }

        #endregion

        // Pantalla 1: Login oscuro
        private Panel pnlLoginScreen = null!;
        private Panel pnlLoginLeft = null!;
        private Panel pnlLoginCard = null!;
        private PictureBox picLoginLogo = null!;
        private Label lblLoginTitle = null!;
        private Label lblLoginSub = null!;
        private Label lblUsuario = null!;
        private TextBox txtUsuario = null!;
        private Label lblPassword = null!;
        private TextBox txtPassword = null!;
        private CheckBox chkMostrarPass = null!;
        private Label lblGateway = null!;
        private TextBox txtGateway = null!;
        private Label lblClientSecret = null!;
        private TextBox txtClientSecret = null!;
        private Label lblLoginError = null!;
        private Button btnIniciarSesion = null!;
        private Button btnTokenInvalidoLogin = null!;
        private Label lblLoginFooter = null!;
        private CoverImagePanel pnlLoginRight = null!;
        private Label lblVisualTag = null!;
        private Label lblVisualCaption = null!;

        // Pantalla 2: Workspace
        private Panel pnlAppWorkspace = null!;
        private Panel pnlSidebar = null!;
        private Panel pnlBrand = null!;
        private PictureBox picSidebarLogo = null!;
        private Label lblBrandPill = null!;
        private Label lblNavTitle = null!;
        private Panel pnlNavVehiculos = null!;
        private Panel pnlIndVehiculos = null!;
        private Button btnNavVehiculos = null!;
        private Panel pnlNavCompradores = null!;
        private Panel pnlIndCompradores = null!;
        private Button btnNavCompradores = null!;
        private Panel pnlNavVentas = null!;
        private Panel pnlIndVentas = null!;
        private Button btnNavVentas = null!;
        private Panel pnlSidebarFooter = null!;
        private Label lblSesion = null!;
        private Button btnTokenInvalido = null!;
        private Button btnCerrarSesion = null!;
        private Label lblSidebarNota = null!;

        private Panel pnlRightArea = null!;
        private Panel pnlTopbar = null!;
        private Label lblTopTitle = null!;
        private Label lblTopSub = null!;
        private Label lblTopBadge = null!;
        private Label lblTopKpi = null!;
        private Panel pnlContentHost = null!;

        // Módulo Vehículos
        private Panel pnlModVehiculos = null!;
        private Panel pnlFormVehiculo = null!;
        private Label lblFormVehTitle = null!;
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
        private Panel pnlGridVehWrap = null!;
        private DataGridView dgvVehiculos = null!;

        // Módulo Compradores
        private Panel pnlModCompradores = null!;
        private Panel pnlFormComprador = null!;
        private Label lblFormCompTitle = null!;
        private Label lblDocumento = null!;
        private TextBox txtDocumento = null!;
        private Label lblNombreComp = null!;
        private TextBox txtNombreComp = null!;
        private Label lblCorreoComp = null!;
        private TextBox txtCorreoComp = null!;
        private Button btnCrearComprador = null!;
        private Button btnListarCompradores = null!;
        private Button btnLimpiarCompradores = null!;
        private Panel pnlGridCompWrap = null!;
        private DataGridView dgvCompradores = null!;

        // Módulo Ventas (Solo consulta)
        private Panel pnlModVentas = null!;
        private Panel pnlVentasBar = null!;
        private Label lblAvisoVentas = null!;
        private Button btnListarVentas = null!;
        private Button btnLimpiarVentas = null!;
        private Panel pnlGridVenWrap = null!;
        private DataGridView dgvVentas = null!;

        // Status bar
        private Panel pnlEstado = null!;
        private Label lblEstado = null!;
        private ProgressBar prgCarga = null!;
    }
}
