using System.Drawing.Drawing2D;

namespace ConsultaVehiculosForms
{
    /// <summary>
    /// Design Tokens compartidos NEXORA Autos — Grafito #0B0F17 · Canvas #F8FAFC · Cobre #EA580C/#C2410C.
    /// Alineado con React Web, PWA Móvil y JavaFX (APE 04).
    /// </summary>
    internal static class Theme
    {
        // Base y superficies
        public static readonly Color Graphite950 = Color.FromArgb(11, 15, 23);     // #0B0F17
        public static readonly Color Graphite920 = Color.FromArgb(13, 17, 25);     // #0D1119 (Login)
        public static readonly Color Graphite900 = Color.FromArgb(17, 24, 39);     // #111827
        public static readonly Color Graphite850 = Color.FromArgb(22, 32, 50);     // #162032
        public static readonly Color Graphite800 = Color.FromArgb(31, 41, 55);     // #1F2937
        public static readonly Color InputDarkBg = Color.FromArgb(18, 23, 34);     // #121722
        public static readonly Color InputDarkBorder = Color.FromArgb(36, 44, 61); // #242C3D

        public static readonly Color Canvas = Color.FromArgb(248, 250, 252);       // #F8FAFC
        public static readonly Color Stone100 = Color.FromArgb(241, 245, 249);     // #F1F5F9
        public static readonly Color Stone150 = Color.FromArgb(226, 232, 240);     // #E2E8F0
        public static readonly Color Stone200 = Color.FromArgb(203, 213, 225);     // #CBD5E1
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(252, 253, 254);   // #FCFDFE

        // Texto
        public static readonly Color Ink950 = Color.FromArgb(9, 13, 22);           // #090D16
        public static readonly Color Ink900 = Color.FromArgb(15, 23, 42);          // #0F172A
        public static readonly Color Ink700 = Color.FromArgb(51, 65, 85);          // #334155
        public static readonly Color Ink500 = Color.FromArgb(100, 116, 139);       // #64748B
        public static readonly Color Ink400 = Color.FromArgb(148, 163, 184);       // #94A3B8
        public static readonly Color TextOnDarkMuted = Color.FromArgb(148, 163, 184);

        // Único acento de acción principal (Cobre Automotriz)
        public static readonly Color Copper400 = Color.FromArgb(251, 146, 60);     // #FB923C
        public static readonly Color Copper600 = Color.FromArgb(234, 88, 12);      // #EA580C
        public static readonly Color Copper700 = Color.FromArgb(194, 65, 12);      // #C2410C
        public static readonly Color Copper800 = Color.FromArgb(154, 52, 18);      // #9A3412
        public static readonly Color CopperSoft = Color.FromArgb(255, 247, 237);   // #FFF7ED

        // Estados semánticos
        public static readonly Color AvailableFg = Color.FromArgb(5, 150, 105);    // #059669 (Verde)
        public static readonly Color AvailableBg = Color.FromArgb(236, 253, 245);  // #ECFDF5

        public static readonly Color ReservedFg = Color.FromArgb(37, 99, 235);     // #2563EB (Azul)
        public static readonly Color ReservedBg = Color.FromArgb(239, 246, 255);   // #EFF6FF

        public static readonly Color SoldFg = Color.FromArgb(100, 116, 139);       // #64748B (Gris apagado)
        public static readonly Color SoldBg = Color.FromArgb(241, 245, 249);       // #F1F5F9

        // Errores y avisos del sistema
        public static readonly Color ErrorFg = Color.FromArgb(220, 38, 38);        // #DC2626 (Rojo)
        public static readonly Color ErrorBg = Color.FromArgb(254, 242, 242);      // #FEF2F2

        public static readonly Color WarnFg = Color.FromArgb(217, 119, 6);         // #D97706 (Ámbar)
        public static readonly Color WarnBg = Color.FromArgb(255, 251, 235);       // #FFFBEB

        // Tipografía
        public static readonly Font FontLoginTitle = new("Segoe UI Semibold", 18F, FontStyle.Bold);
        public static readonly Font FontPageTitle = new("Segoe UI Semibold", 14.5F, FontStyle.Bold);
        public static readonly Font FontBody = new("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font FontBodyBold = new("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font FontSmall = new("Segoe UI", 8.5F, FontStyle.Regular);
        public static readonly Font FontSmallBold = new("Segoe UI", 8.5F, FontStyle.Bold);
        public static readonly Font FontStrike = new("Segoe UI", 9.5F, FontStyle.Strikeout);

        public static void StyleCopperButton(Button btn)
        {
            btn.BackColor = Copper600;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Copper700;
            btn.FlatAppearance.MouseDownBackColor = Copper800;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void StyleSecondaryButton(Button btn)
        {
            btn.BackColor = Surface;
            btn.ForeColor = Ink900;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Stone200;
            btn.FlatAppearance.MouseOverBackColor = Stone100;
            btn.FlatAppearance.MouseDownBackColor = Stone150;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void StyleDarkButton(Button btn, bool danger = false)
        {
            btn.BackColor = danger ? Color.FromArgb(52, 22, 26) : Graphite900;
            btn.ForeColor = danger ? Color.FromArgb(252, 165, 165) : Color.FromArgb(226, 232, 240);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = danger ? Color.FromArgb(127, 29, 29) : Graphite800;
            btn.FlatAppearance.MouseOverBackColor = danger ? Color.FromArgb(78, 28, 30) : Graphite850;
            btn.Font = FontSmallBold;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void StyleDarkInput(TextBox txt)
        {
            txt.BackColor = InputDarkBg;
            txt.ForeColor = Color.White;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = FontBody;
        }

        public static void StyleGrid(DataGridView dgv, string name)
        {
            dgv.Name = name;
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Stone100;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Canvas;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Ink500;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontSmallBold;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Canvas;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Ink500;

            dgv.RowTemplate.Height = 36;
            dgv.DefaultCellStyle.BackColor = Surface;
            dgv.DefaultCellStyle.ForeColor = Ink900;
            dgv.DefaultCellStyle.Font = FontBody;
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgv.DefaultCellStyle.SelectionBackColor = CopperSoft;
            dgv.DefaultCellStyle.SelectionForeColor = Ink900;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = SurfaceAlt;

            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
        }

        public static Image? LoadAsset(string fileName)
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Assets", fileName);
                if (!File.Exists(path)) return null;
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                return Image.FromStream(fs);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Panel que dibuja una imagen en modo Cover (sin deformar) con degradado oscuro lateral e inferior.
    /// </summary>
    internal sealed class CoverImagePanel : Panel
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Image? CoverImage { get; set; }

        public CoverImagePanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Theme.Graphite950;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (CoverImage != null && Width > 0 && Height > 0)
            {
                float imgRatio = (float)CoverImage.Width / CoverImage.Height;
                float boxRatio = (float)Width / Height;
                int drawW, drawH, drawX, drawY;

                if (boxRatio > imgRatio)
                {
                    drawW = Width;
                    drawH = (int)(Width / imgRatio);
                    drawX = 0;
                    drawY = (Height - drawH) / 2;
                }
                else
                {
                    drawH = Height;
                    drawW = (int)(Height * imgRatio);
                    drawX = (Width - drawW) / 2;
                    drawY = 0;
                }

                g.DrawImage(CoverImage, new Rectangle(drawX, drawY, drawW, drawH));

                // Fusión oscura desde la izquierda hacia la imagen
                using var leftFade = new LinearGradientBrush(
                    new Rectangle(0, 0, Math.Max(1, Width / 2), Height),
                    Color.FromArgb(245, 13, 17, 25),
                    Color.FromArgb(0, 13, 17, 25),
                    LinearGradientMode.Horizontal);
                g.FillRectangle(leftFade, new Rectangle(0, 0, Math.Max(1, Width / 2), Height));

                // Degradado inferior para legibilidad del pie editorial
                int bottomH = Math.Max(1, Height / 2);
                using var bottomFade = new LinearGradientBrush(
                    new Rectangle(0, Height - bottomH, Width, bottomH),
                    Color.FromArgb(0, 9, 12, 18),
                    Color.FromArgb(230, 9, 12, 18),
                    LinearGradientMode.Vertical);
                g.FillRectangle(bottomFade, new Rectangle(0, Height - bottomH, Width, bottomH));
            }
        }
    }
}
