namespace ConsultaVehiculosForms
{
    /// <summary>
    /// Barra reutilizable de paginación en cliente con identidad NEXORA Autos.
    /// </summary>
    internal sealed class PaginationBar : Panel
    {
        private readonly Label _lblResumen = new();
        private readonly Label _lblPorPagina = new();
        private readonly ComboBox _cmbPorPagina = new();
        private readonly Button _btnAnterior = new();
        private readonly Label _lblPagina = new();
        private readonly Button _btnSiguiente = new();
        private readonly FlowLayoutPanel _pnlControles = new();

        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private bool _actualizandoInterno;

        public event Action<int>? PageChanged;
        public event Action<int>? PageSizeChanged;

        public int PageSize => _cmbPorPagina.SelectedItem is int val ? val : 10;

        public PaginationBar(int defaultPageSize = 10)
        {
            Dock = DockStyle.Bottom;
            Height = 46;
            BackColor = Theme.Surface;
            Padding = new Padding(14, 7, 14, 7);

            Paint += (_, e) =>
            {
                using var pen = new Pen(Theme.Stone150, 1);
                e.Graphics.DrawLine(pen, 0, 0, Width, 0);
            };

            _lblResumen.Dock = DockStyle.Left;
            _lblResumen.AutoSize = false;
            _lblResumen.Width = 280;
            _lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            _lblResumen.Font = Theme.FontSmallBold;
            _lblResumen.ForeColor = Theme.Ink500;
            _lblResumen.Text = "Mostrando 0 de 0 registros";

            _pnlControles.Dock = DockStyle.Right;
            _pnlControles.AutoSize = true;
            _pnlControles.FlowDirection = FlowDirection.LeftToRight;
            _pnlControles.WrapContents = false;
            _pnlControles.BackColor = Color.Transparent;
            _pnlControles.Padding = new Padding(0, 1, 0, 0);

            _lblPorPagina.AutoSize = true;
            _lblPorPagina.Font = Theme.FontSmallBold;
            _lblPorPagina.ForeColor = Theme.Ink700;
            _lblPorPagina.Text = "Por página:";
            _lblPorPagina.Margin = new Padding(0, 7, 6, 0);

            _cmbPorPagina.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPorPagina.Width = 58;
            _cmbPorPagina.Font = Theme.FontSmallBold;
            _cmbPorPagina.Margin = new Padding(0, 3, 14, 0);
            _cmbPorPagina.Items.AddRange(new object[] { 6, 10, 15, 25 });
            _cmbPorPagina.SelectedItem = _cmbPorPagina.Items.Contains(defaultPageSize) ? defaultPageSize : 10;
            _cmbPorPagina.SelectedIndexChanged += (_, _) =>
            {
                if (_actualizandoInterno) return;
                PageSizeChanged?.Invoke(PageSize);
            };

            _btnAnterior.Size = new Size(90, 30);
            _btnAnterior.Text = "‹ Anterior";
            _btnAnterior.Margin = new Padding(0, 0, 8, 0);
            Theme.StyleSecondaryButton(_btnAnterior);
            _btnAnterior.Font = Theme.FontSmallBold;
            _btnAnterior.Click += (_, _) =>
            {
                if (_paginaActual > 1)
                {
                    PageChanged?.Invoke(_paginaActual - 1);
                }
            };

            _lblPagina.AutoSize = false;
            _lblPagina.Size = new Size(104, 30);
            _lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            _lblPagina.Font = Theme.FontSmallBold;
            _lblPagina.BackColor = Theme.Graphite950;
            _lblPagina.ForeColor = Color.White;
            _lblPagina.Margin = new Padding(0, 0, 8, 0);
            _lblPagina.Text = "Página 1 de 1";

            _btnSiguiente.Size = new Size(92, 30);
            _btnSiguiente.Text = "Siguiente ›";
            _btnSiguiente.Margin = new Padding(0);
            Theme.StyleSecondaryButton(_btnSiguiente);
            _btnSiguiente.Font = Theme.FontSmallBold;
            _btnSiguiente.Click += (_, _) =>
            {
                if (_paginaActual < _totalPaginas)
                {
                    PageChanged?.Invoke(_paginaActual + 1);
                }
            };

            _pnlControles.Controls.AddRange(new Control[]
            {
                _lblPorPagina, _cmbPorPagina, _btnAnterior, _lblPagina, _btnSiguiente
            });

            Controls.Add(_pnlControles);
            Controls.Add(_lblResumen);

            UpdateState(0, 1, defaultPageSize, "registros");
        }

        public void UpdateState(int totalItems, int paginaActual, int porPagina, string etiquetaItems)
        {
            _totalPaginas = Math.Max(1, (int)Math.Ceiling(totalItems / (double)Math.Max(1, porPagina)));
            _paginaActual = Math.Min(Math.Max(1, paginaActual), _totalPaginas);

            _actualizandoInterno = true;
            if (_cmbPorPagina.Items.Contains(porPagina))
            {
                _cmbPorPagina.SelectedItem = porPagina;
            }
            _actualizandoInterno = false;

            if (totalItems <= 0)
            {
                _lblResumen.Text = $"Mostrando 0 de 0 {etiquetaItems}";
                _lblPagina.Text = "Página 1 de 1";
                _btnAnterior.Enabled = false;
                _btnSiguiente.Enabled = false;
                return;
            }

            int desde = (_paginaActual - 1) * porPagina + 1;
            int hasta = Math.Min(_paginaActual * porPagina, totalItems);
            _lblResumen.Text = $"Mostrando {desde}–{hasta} de {totalItems} {etiquetaItems}";
            _lblPagina.Text = $"Página {_paginaActual} de {_totalPaginas}";
            _btnAnterior.Enabled = _paginaActual > 1;
            _btnSiguiente.Enabled = _paginaActual < _totalPaginas;
        }
    }
}
