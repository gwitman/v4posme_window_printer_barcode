using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using v4posme_PrinterBarCode.Models;
using v4posme_PrinterBarCode.Services;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Ventana principal: lista productos en un grid con busqueda, mantiene la
    /// seleccion en memoria entre busquedas e imprime codigos de barra.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly AppConfig _config;
        private readonly ProductService _productService;

        /// <summary>Listado completo descargado.</summary>
        private List<Product> _allProducts = new List<Product>();

        /// <summary>Vista filtrada actualmente visible en el grid.</summary>
        private List<Product> _filtered = new List<Product>();

        /// <summary>
        /// Seleccion persistente en memoria. Se conserva entre busquedas y solo
        /// se limpia con el boton "Limpiar seleccion". La clave es el codigo del producto.
        /// </summary>
        private readonly HashSet<string> _selectedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private const string SelCol = "colSelect";
        private bool _updatingGrid;

        /// <summary>Impresoras instaladas, usadas por el dialogo de impresion.</summary>
        private readonly List<string> _printers = new List<string>();

        /// <summary>Impresora preseleccionada (config o la por defecto del sistema).</summary>
        private string _preselectedPrinter;

        /// <summary>Ultima impresora elegida por el usuario (se recuerda entre impresiones).</summary>
        private string _lastUsedPrinter;

        public MainForm(AppConfig config)
        {
            InitializeComponent();
            _config = config;
            _productService = new ProductService(config);
            BuildColumns();
            WireGridEvents();
            LoadPrinters();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await LoadProductsAsync();
        }

        // ------------------------------------------------------- Impresoras

        /// <summary>
        /// Enumera las impresoras instaladas y define la preseleccion (la del
        /// config.json o, si no esta definida, la por defecto del sistema).
        /// La seleccion real se hace en el dialogo de impresion.
        /// </summary>
        private void LoadPrinters()
        {
            _printers.Clear();
            string defaultPrinter = null;

            try
            {
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    _printers.Add(printer);

                using (var doc = new System.Drawing.Printing.PrintDocument())
                    defaultPrinter = doc.PrinterSettings.PrinterName;
            }
            catch (Exception ex)
            {
                Logger.Error("No se pudieron enumerar las impresoras instaladas.", ex);
            }

            _preselectedPrinter = !string.IsNullOrWhiteSpace(_config.PrinterName)
                ? _config.PrinterName
                : defaultPrinter;

            Logger.Info($"Impresoras cargadas: {_printers.Count}. Preseleccionada: '{_preselectedPrinter}'.");
        }

        // ---------------------------------------------------------------- Grid

        private void BuildColumns()
        {
            grid.AutoGenerateColumns = false;
            grid.MultiSelect = true;

            var colSel = new DataGridViewCheckBoxColumn
            {
                Name = SelCol,
                HeaderText = "",
                Width = 40,
                Resizable = DataGridViewTriState.False
            };
            var colCode = new DataGridViewTextBoxColumn
            {
                Name = "colCode",
                HeaderText = "Codigo",
                Width = 140,
                ReadOnly = true
            };
            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Nombre del producto",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colBarcode = new DataGridViewTextBoxColumn
            {
                Name = "colBarcode",
                HeaderText = "Codigo de barra",
                Width = 220,
                ReadOnly = true
            };
            var colPrice = new DataGridViewTextBoxColumn
            {
                Name = "colPrice",
                HeaderText = "Precio",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C", Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            grid.Columns.AddRange(colSel, colCode, colName, colBarcode, colPrice);

            // Estilos profesionales.
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 36;
            grid.RowTemplate.Height = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 228, 247);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.GridColor = Color.FromArgb(230, 230, 230);
        }

        private void WireGridEvents()
        {
            grid.CellValueChanged += Grid_CellValueChanged;
            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            // Doble clic sobre una fila alterna su seleccion.
            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var cell = grid.Rows[e.RowIndex].Cells[SelCol] as DataGridViewCheckBoxCell;
                if (cell != null)
                {
                    bool current = cell.Value is bool b && b;
                    cell.Value = !current;
                }
            };
        }

        private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_updatingGrid || e.RowIndex < 0) return;
            if (grid.Columns[e.ColumnIndex].Name != SelCol) return;

            var product = _filtered[e.RowIndex];
            bool isChecked = grid.Rows[e.RowIndex].Cells[SelCol].Value is bool b && b;

            var key = SelectionKey(product);
            if (isChecked) _selectedKeys.Add(key);
            else _selectedKeys.Remove(key);

            UpdateSelectedCount();
        }

        private static string SelectionKey(Product p) =>
            !string.IsNullOrWhiteSpace(p.Code) ? p.Code : p.EffectiveBarcode ?? p.Name ?? Guid.NewGuid().ToString();

        // ------------------------------------------------------------- Carga

        private async Task LoadProductsAsync()
        {
            using (var loading = new LoadingForm())
            {
                loading.Show(this);
                loading.SetStatus("Conectando con el servicio...");
                Application.DoEvents();

                try
                {
                    loading.SetStatus("Descargando informacion de productos...");
                    _allProducts = await _productService.GetProductsAsync();

                    loading.SetStatus("Preparando el listado...");
                    ApplyFilter(txtSearch.Text);

                    SetStatus($"Se cargaron {_allProducts.Count} productos.");
                }
                catch (Exception ex)
                {
                    Logger.Error("Fallo al cargar los productos.", ex);
                    SetStatus("Error al cargar productos. Revise el log.");
                    MessageBox.Show(this,
                        "No se pudo cargar la informacion de productos.\n\n" + ex.Message +
                        "\n\nRevise el archivo de log para mas detalles.",
                        "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loading.Close();
                }
            }
        }

        // ----------------------------------------------------------- Filtro

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(txtSearch.Text);
        }

        /// <summary>
        /// Filtra por nombre, codigo o codigo de barra. La seleccion en memoria
        /// se preserva: al re-pintar el grid se marcan las filas ya seleccionadas.
        /// </summary>
        private void ApplyFilter(string term)
        {
            term = (term ?? string.Empty).Trim();

            if (term.Length == 0)
            {
                _filtered = _allProducts.ToList();
            }
            else
            {
                _filtered = _allProducts.Where(p =>
                    (p.Name != null && p.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Code != null && p.Code.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Barcode != null && p.Barcode.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            RenderGrid();
        }

        private void RenderGrid()
        {
            _updatingGrid = true;
            grid.Rows.Clear();

            foreach (var p in _filtered)
            {
                bool selected = _selectedKeys.Contains(SelectionKey(p));
                int idx = grid.Rows.Add(selected, p.Code, p.Name, p.Barcode, p.Price);
                grid.Rows[idx].Cells["colBarcode"].ToolTipText = p.EffectiveBarcode;
            }

            _updatingGrid = false;
            UpdateSelectedCount();
            SetStatus($"Mostrando {_filtered.Count} de {_allProducts.Count} productos.");
        }

        private void UpdateSelectedCount()
        {
            lblSelectedCount.Text = $"Seleccionados: {_selectedKeys.Count}";
            btnPrint.Enabled = _selectedKeys.Count > 0;
        }

        // -------------------------------------------------------- Botones

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            _selectedKeys.Clear();
            Logger.Info("Seleccion en memoria limpiada por el usuario.");
            RenderGrid();
        }

        private async void btnReload_Click(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            // Resolvemos los productos seleccionados desde el listado completo.
            var selected = _allProducts
                .Where(p => _selectedKeys.Contains(SelectionKey(p)))
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(this, "No hay productos seleccionados.", "Imprimir",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_printers.Count == 0)
            {
                MessageBox.Show(this, "No hay impresoras instaladas en el sistema.", "Imprimir",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int quantity;
            string printerName;
            double widthMm, heightMm;
            // Preselecciona la ultima impresora usada, o la del config/por defecto.
            var preselect = _lastUsedPrinter ?? _preselectedPrinter;
            using (var dlg = new QuantityDialog(selected.Count, _printers, preselect,
                _config.Barcode.WidthMm, _config.Barcode.HeightMm))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                quantity = dlg.Quantity;
                printerName = dlg.SelectedPrinter;
                widthMm = dlg.PageWidthMm;
                heightMm = dlg.PageHeightMm;
            }

            _lastUsedPrinter = printerName;

            foreach (var p in selected)
                p.PrintQuantity = quantity;

            await PrintAsync(selected, quantity, printerName, widthMm, heightMm);
        }

        // ------------------------------------------------------- Impresion

        private async Task PrintAsync(List<Product> products, int quantity, string printerName,
            double widthMm, double heightMm)
        {
            SetBusy(true, $"Imprimiendo en '{printerName}'... ({products.Count * quantity} etiquetas)");
            Logger.Info($"Impresion solicitada: {products.Count} productos x {quantity} = {products.Count * quantity} " +
                $"etiquetas en '{printerName}'. Tamano pagina: {widthMm}x{heightMm} mm.");

            try
            {
                var printer = new BarcodePrinter(_config);
                await Task.Run(() => printer.Print(products, printerName, widthMm, heightMm));

                SetStatus("Impresion completada.");
                MessageBox.Show(this, "Impresion enviada correctamente.", "Imprimir",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Error("Error durante la impresion.", ex);
                SetStatus("Error al imprimir. Revise el log.");
                MessageBox.Show(this,
                    "Ocurrio un error al imprimir.\n\n" + ex.Message +
                    "\n\nRevise el archivo de log para mas detalles.",
                    "Error de impresion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        /// <summary>
        /// Activa/desactiva el estado "ocupado": muestra el boton de espera y
        /// bloquea las acciones mientras la impresion sigue en curso.
        /// </summary>
        private void SetBusy(bool busy, string statusText)
        {
            txtSearch.Enabled = !busy;
            btnClearSelection.Enabled = !busy;
            btnReload.Enabled = !busy;
            grid.Enabled = !busy;

            if (busy)
            {
                btnPrint.Enabled = false;
                btnPrint.Text = "Esperando...";
                btnPrint.BackColor = Color.FromArgb(150, 150, 150);
                Cursor = Cursors.WaitCursor;
                if (statusText != null) SetStatus(statusText);
            }
            else
            {
                btnPrint.Text = "Imprimir";
                btnPrint.BackColor = Color.FromArgb(0, 120, 215);
                btnPrint.Enabled = _selectedKeys.Count > 0;
                Cursor = Cursors.Default;
            }
        }

        private void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatus), text);
                return;
            }
            lblStatus.Text = text;
        }
    }
}
