namespace v4posme_PrinterBarCode.Forms
{
    partial class QuantityDialog
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

        private void InitializeComponent()
        {
            this.panelAccent = new System.Windows.Forms.Panel();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblPrinter = new System.Windows.Forms.Label();
            this.cboPrinter = new System.Windows.Forms.ComboBox();
            this.grpBarcode = new System.Windows.Forms.GroupBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.lblMargin = new System.Windows.Forms.Label();
            this.numMargin = new System.Windows.Forms.NumericUpDown();
            this.lblBorderThickness = new System.Windows.Forms.Label();
            this.numBorderThickness = new System.Windows.Forms.NumericUpDown();
            this.lblLabelFont = new System.Windows.Forms.Label();
            this.txtLabelFont = new System.Windows.Forms.TextBox();
            this.lblLabelFontSize = new System.Windows.Forms.Label();
            this.numLabelFontSize = new System.Windows.Forms.NumericUpDown();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.chkShowProductName = new System.Windows.Forms.CheckBox();
            this.chkShowPrice = new System.Windows.Forms.CheckBox();
            this.chkShowBorder = new System.Windows.Forms.CheckBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBorderThickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelFontSize)).BeginInit();
            this.grpBarcode.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelAccent
            // 
            this.panelAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.panelAccent.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAccent.Size = new System.Drawing.Size(470, 6);
            this.panelAccent.Name = "panelAccent";
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPrompt.Location = new System.Drawing.Point(20, 18);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(220, 21);
            this.lblPrompt.Text = "Imprimir codigos de barra";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.lblInfo.Location = new System.Drawing.Point(22, 42);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(180, 15);
            this.lblInfo.Text = "Se aplica a cada producto seleccionado.";
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQuantity.Location = new System.Drawing.Point(22, 66);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Text = "Cantidad por producto:";
            // 
            // numQuantity
            // 
            this.numQuantity.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.numQuantity.Location = new System.Drawing.Point(170, 62);
            this.numQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(90, 27);
            this.numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQuantity.ValueChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // lblPrinter
            // 
            this.lblPrinter.AutoSize = true;
            this.lblPrinter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPrinter.Location = new System.Drawing.Point(22, 98);
            this.lblPrinter.Name = "lblPrinter";
            this.lblPrinter.Text = "Impresora:";
            // 
            // cboPrinter
            // 
            this.cboPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrinter.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cboPrinter.Location = new System.Drawing.Point(170, 94);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.Size = new System.Drawing.Size(278, 25);
            this.cboPrinter.SelectedIndexChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // grpBarcode
            // 
            this.grpBarcode.Controls.Add(this.lblWidth);
            this.grpBarcode.Controls.Add(this.numWidth);
            this.grpBarcode.Controls.Add(this.lblHeight);
            this.grpBarcode.Controls.Add(this.numHeight);
            this.grpBarcode.Controls.Add(this.lblMargin);
            this.grpBarcode.Controls.Add(this.numMargin);
            this.grpBarcode.Controls.Add(this.lblBorderThickness);
            this.grpBarcode.Controls.Add(this.numBorderThickness);
            this.grpBarcode.Controls.Add(this.lblLabelFont);
            this.grpBarcode.Controls.Add(this.txtLabelFont);
            this.grpBarcode.Controls.Add(this.lblLabelFontSize);
            this.grpBarcode.Controls.Add(this.numLabelFontSize);
            this.grpBarcode.Controls.Add(this.lblPrefix);
            this.grpBarcode.Controls.Add(this.txtPrefix);
            this.grpBarcode.Controls.Add(this.lblSuffix);
            this.grpBarcode.Controls.Add(this.txtSuffix);
            this.grpBarcode.Controls.Add(this.chkShowProductName);
            this.grpBarcode.Controls.Add(this.chkShowPrice);
            this.grpBarcode.Controls.Add(this.chkShowBorder);
            this.grpBarcode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpBarcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.grpBarcode.Location = new System.Drawing.Point(22, 130);
            this.grpBarcode.Name = "grpBarcode";
            this.grpBarcode.Size = new System.Drawing.Size(426, 230);
            this.grpBarcode.TabStop = false;
            this.grpBarcode.Text = "Configuracion de la etiqueta";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblWidth.ForeColor = System.Drawing.Color.Black;
            this.lblWidth.Location = new System.Drawing.Point(14, 28);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Text = "Ancho (mm):";
            // 
            // numWidth
            // 
            this.numWidth.DecimalPlaces = 1;
            this.numWidth.Location = new System.Drawing.Point(110, 24);
            this.numWidth.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numWidth.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(80, 23);
            this.numWidth.Value = new decimal(new int[] { 50, 0, 0, 0 });
            this.numWidth.ValueChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblHeight.ForeColor = System.Drawing.Color.Black;
            this.lblHeight.Location = new System.Drawing.Point(220, 28);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Text = "Alto (mm):";
            // 
            // numHeight
            // 
            this.numHeight.DecimalPlaces = 1;
            this.numHeight.Location = new System.Drawing.Point(320, 24);
            this.numHeight.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numHeight.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(80, 23);
            this.numHeight.Value = new decimal(new int[] { 25, 0, 0, 0 });
            this.numHeight.ValueChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // lblMargin
            // 
            this.lblMargin.AutoSize = true;
            this.lblMargin.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblMargin.ForeColor = System.Drawing.Color.Black;
            this.lblMargin.Location = new System.Drawing.Point(14, 60);
            this.lblMargin.Name = "lblMargin";
            this.lblMargin.Text = "Margen (mm):";
            // 
            // numMargin
            // 
            this.numMargin.DecimalPlaces = 1;
            this.numMargin.Location = new System.Drawing.Point(110, 56);
            this.numMargin.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this.numMargin.Name = "numMargin";
            this.numMargin.Size = new System.Drawing.Size(80, 23);
            this.numMargin.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // lblBorderThickness
            // 
            this.lblBorderThickness.AutoSize = true;
            this.lblBorderThickness.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBorderThickness.ForeColor = System.Drawing.Color.Black;
            this.lblBorderThickness.Location = new System.Drawing.Point(220, 60);
            this.lblBorderThickness.Name = "lblBorderThickness";
            this.lblBorderThickness.Text = "Grosor borde (mm):";
            // 
            // numBorderThickness
            // 
            this.numBorderThickness.DecimalPlaces = 1;
            this.numBorderThickness.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            this.numBorderThickness.Location = new System.Drawing.Point(320, 56);
            this.numBorderThickness.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numBorderThickness.Name = "numBorderThickness";
            this.numBorderThickness.Size = new System.Drawing.Size(80, 23);
            this.numBorderThickness.Value = new decimal(new int[] { 3, 0, 0, 65536 });
            // 
            // lblLabelFont
            // 
            this.lblLabelFont.AutoSize = true;
            this.lblLabelFont.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblLabelFont.ForeColor = System.Drawing.Color.Black;
            this.lblLabelFont.Location = new System.Drawing.Point(14, 92);
            this.lblLabelFont.Name = "lblLabelFont";
            this.lblLabelFont.Text = "Fuente texto:";
            // 
            // txtLabelFont
            // 
            this.txtLabelFont.Location = new System.Drawing.Point(110, 88);
            this.txtLabelFont.Name = "txtLabelFont";
            this.txtLabelFont.Size = new System.Drawing.Size(80, 23);
            this.txtLabelFont.Text = "Arial";
            // 
            // lblLabelFontSize
            // 
            this.lblLabelFontSize.AutoSize = true;
            this.lblLabelFontSize.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblLabelFontSize.ForeColor = System.Drawing.Color.Black;
            this.lblLabelFontSize.Location = new System.Drawing.Point(220, 92);
            this.lblLabelFontSize.Name = "lblLabelFontSize";
            this.lblLabelFontSize.Text = "Tam. fuente:";
            // 
            // numLabelFontSize
            // 
            this.numLabelFontSize.DecimalPlaces = 1;
            this.numLabelFontSize.Location = new System.Drawing.Point(320, 88);
            this.numLabelFontSize.Maximum = new decimal(new int[] { 72, 0, 0, 0 });
            this.numLabelFontSize.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            this.numLabelFontSize.Name = "numLabelFontSize";
            this.numLabelFontSize.Size = new System.Drawing.Size(80, 23);
            this.numLabelFontSize.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblPrefix.ForeColor = System.Drawing.Color.Black;
            this.lblPrefix.Location = new System.Drawing.Point(14, 124);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Text = "Prefijo:";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(110, 120);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(80, 23);
            // 
            // lblSuffix
            // 
            this.lblSuffix.AutoSize = true;
            this.lblSuffix.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSuffix.ForeColor = System.Drawing.Color.Black;
            this.lblSuffix.Location = new System.Drawing.Point(220, 124);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Text = "Sufijo:";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Location = new System.Drawing.Point(320, 120);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(80, 23);
            // 
            // chkShowProductName
            // 
            this.chkShowProductName.AutoSize = true;
            this.chkShowProductName.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkShowProductName.ForeColor = System.Drawing.Color.Black;
            this.chkShowProductName.Location = new System.Drawing.Point(16, 158);
            this.chkShowProductName.Name = "chkShowProductName";
            this.chkShowProductName.Text = "Mostrar nombre";
            this.chkShowProductName.Checked = true;
            // 
            // chkShowPrice
            // 
            this.chkShowPrice.AutoSize = true;
            this.chkShowPrice.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkShowPrice.ForeColor = System.Drawing.Color.Black;
            this.chkShowPrice.Location = new System.Drawing.Point(160, 158);
            this.chkShowPrice.Name = "chkShowPrice";
            this.chkShowPrice.Text = "Mostrar precio";
            this.chkShowPrice.Checked = true;
            // 
            // chkShowBorder
            // 
            this.chkShowBorder.AutoSize = true;
            this.chkShowBorder.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkShowBorder.ForeColor = System.Drawing.Color.Black;
            this.chkShowBorder.Location = new System.Drawing.Point(300, 158);
            this.chkShowBorder.Name = "chkShowBorder";
            this.chkShowBorder.Text = "Mostrar borde";
            this.chkShowBorder.Checked = true;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblSummary.Location = new System.Drawing.Point(22, 368);
            this.lblSummary.MaximumSize = new System.Drawing.Size(430, 40);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(200, 15);
            this.lblSummary.Text = "Resumen";
            // 
            // btnAccept
            // 
            this.btnAccept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnAccept.FlatAppearance.BorderSize = 0;
            this.btnAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccept.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAccept.ForeColor = System.Drawing.Color.White;
            this.btnAccept.Location = new System.Drawing.Point(284, 410);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(80, 38);
            this.btnAccept.Text = "Imprimir";
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.Location = new System.Drawing.Point(368, 410);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 38);
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // QuantityDialog
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(470, 466);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.grpBarcode);
            this.Controls.Add(this.cboPrinter);
            this.Controls.Add(this.lblPrinter);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.panelAccent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuantityDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Imprimir codigos de barra";
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBorderThickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelFontSize)).EndInit();
            this.grpBarcode.ResumeLayout(false);
            this.grpBarcode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelAccent;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Label lblPrinter;
        private System.Windows.Forms.ComboBox cboPrinter;
        private System.Windows.Forms.GroupBox grpBarcode;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.Label lblMargin;
        private System.Windows.Forms.NumericUpDown numMargin;
        private System.Windows.Forms.Label lblBorderThickness;
        private System.Windows.Forms.NumericUpDown numBorderThickness;
        private System.Windows.Forms.Label lblLabelFont;
        private System.Windows.Forms.TextBox txtLabelFont;
        private System.Windows.Forms.Label lblLabelFontSize;
        private System.Windows.Forms.NumericUpDown numLabelFontSize;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblSuffix;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.CheckBox chkShowProductName;
        private System.Windows.Forms.CheckBox chkShowPrice;
        private System.Windows.Forms.CheckBox chkShowBorder;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
    }
}
