namespace ConsultaVehiculosForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            grpAuth = new GroupBox();
            lblToken = new Label();
            txtToken = new TextBox();
            btnTokenValido = new Button();
            btnTokenInvalido = new Button();
            btnCopiar = new Button();
            pnlAcciones = new Panel();
            btnCargar = new Button();
            btnLimpiar = new Button();
            lblEstado = new Label();
            dgvVehiculos = new DataGridView();
            pnlHeader.SuspendLayout();
            grpAuth.SuspendLayout();
            pnlAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(24, 43, 73);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(15, 12, 15, 12);
            pnlHeader.Size = new Size(900, 55);
            pnlHeader.TabIndex = 3;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(567, 28);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "🚗 Consulta de Vehículos - Consumo de Servicios con JWT";
            // 
            // grpAuth
            // 
            grpAuth.Controls.Add(lblToken);
            grpAuth.Controls.Add(txtToken);
            grpAuth.Controls.Add(btnTokenValido);
            grpAuth.Controls.Add(btnTokenInvalido);
            grpAuth.Dock = DockStyle.Top;
            grpAuth.Location = new Point(0, 55);
            grpAuth.Name = "grpAuth";
            grpAuth.Padding = new Padding(15, 10, 15, 10);
            grpAuth.Size = new Size(900, 135);
            grpAuth.TabIndex = 2;
            grpAuth.TabStop = false;
            grpAuth.Text = "Configuración del Token JWT (Endpoint GET /api/vehiculos en :8080)";
            // 
            // lblToken
            // 
            lblToken.AutoSize = true;
            lblToken.Location = new Point(15, 25);
            lblToken.Name = "lblToken";
            lblToken.Size = new Size(102, 21);
            lblToken.TabIndex = 0;
            lblToken.Text = "Token Bearer:";
            // 
            // txtToken
            // 
            txtToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtToken.Font = new Font("Consolas", 8.5F);
            txtToken.Location = new Point(15, 48);
            txtToken.Multiline = true;
            txtToken.Name = "txtToken";
            txtToken.ScrollBars = ScrollBars.Vertical;
            txtToken.Size = new Size(867, 70);
            txtToken.TabIndex = 1;
            // 
            // btnTokenValido
            // 
            btnTokenValido.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTokenValido.BackColor = Color.FromArgb(25, 135, 84);
            btnTokenValido.FlatStyle = FlatStyle.Flat;
            btnTokenValido.ForeColor = Color.White;
            btnTokenValido.Location = new Point(1380, 48);
            btnTokenValido.Name = "btnTokenValido";
            btnTokenValido.Size = new Size(195, 32);
            btnTokenValido.TabIndex = 2;
            btnTokenValido.Text = "🔑 Token Válido";
            btnTokenValido.UseVisualStyleBackColor = false;
            // 
            // btnTokenInvalido
            // 
            btnTokenInvalido.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTokenInvalido.BackColor = Color.FromArgb(220, 53, 69);
            btnTokenInvalido.FlatStyle = FlatStyle.Flat;
            btnTokenInvalido.ForeColor = Color.White;
            btnTokenInvalido.Location = new Point(1380, 85);
            btnTokenInvalido.Name = "btnTokenInvalido";
            btnTokenInvalido.Size = new Size(195, 32);
            btnTokenInvalido.TabIndex = 3;
            btnTokenInvalido.Text = "❌ Token Inválido (401)";
            btnTokenInvalido.UseVisualStyleBackColor = false;
            // 
            // btnCopiar
            // 
            btnCopiar.Location = new Point(0, 0);
            btnCopiar.Name = "btnCopiar";
            btnCopiar.Size = new Size(75, 23);
            btnCopiar.TabIndex = 0;
            // 
            // pnlAcciones
            // 
            pnlAcciones.Controls.Add(btnCargar);
            pnlAcciones.Controls.Add(btnLimpiar);
            pnlAcciones.Controls.Add(lblEstado);
            pnlAcciones.Dock = DockStyle.Top;
            pnlAcciones.Location = new Point(0, 190);
            pnlAcciones.Name = "pnlAcciones";
            pnlAcciones.Padding = new Padding(15, 10, 15, 10);
            pnlAcciones.Size = new Size(900, 55);
            pnlAcciones.TabIndex = 1;
            // 
            // btnCargar
            // 
            btnCargar.BackColor = Color.FromArgb(13, 110, 253);
            btnCargar.FlatStyle = FlatStyle.Flat;
            btnCargar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCargar.ForeColor = Color.White;
            btnCargar.Location = new Point(15, 10);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(185, 35);
            btnCargar.TabIndex = 0;
            btnCargar.Text = "📥 Consultar Vehículos";
            btnCargar.UseVisualStyleBackColor = false;
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.FromArgb(108, 117, 125);
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.ForeColor = Color.White;
            btnLimpiar.Location = new Point(210, 10);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(110, 35);
            btnLimpiar.TabIndex = 1;
            btnLimpiar.Text = "\U0001f9f9 Limpiar";
            btnLimpiar.UseVisualStyleBackColor = false;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEstado.ForeColor = Color.DarkSlateBlue;
            lblEstado.Location = new Point(340, 18);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(171, 23);
            lblEstado.TabIndex = 2;
            lblEstado.Text = "Listo para consultar.";
            // 
            // dgvVehiculos
            // 
            dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVehiculos.BackgroundColor = Color.White;
            dgvVehiculos.ColumnHeadersHeight = 29;
            dgvVehiculos.Dock = DockStyle.Fill;
            dgvVehiculos.Location = new Point(0, 245);
            dgvVehiculos.MultiSelect = false;
            dgvVehiculos.Name = "dgvVehiculos";
            dgvVehiculos.ReadOnly = true;
            dgvVehiculos.RowHeadersVisible = false;
            dgvVehiculos.RowHeadersWidth = 51;
            dgvVehiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVehiculos.Size = new Size(900, 335);
            dgvVehiculos.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 580);
            Controls.Add(dgvVehiculos);
            Controls.Add(pnlAcciones);
            Controls.Add(grpAuth);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9.5F);
            MinimumSize = new Size(800, 500);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Consulta de Vehículos con JWT (Tema 4.1 - Consumo desde Escritorio)";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            grpAuth.ResumeLayout(false);
            grpAuth.PerformLayout();
            pnlAcciones.ResumeLayout(false);
            pnlAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVehiculos).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;

        private System.Windows.Forms.GroupBox grpAuth;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Button btnTokenValido;
        private System.Windows.Forms.Button btnTokenInvalido;
        private System.Windows.Forms.Button btnCopiar;

        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblEstado;

        private System.Windows.Forms.DataGridView dgvVehiculos;
    }
}
