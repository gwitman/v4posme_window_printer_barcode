namespace v4posme_PrinterBarCode.Forms
{
    partial class ConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.panelBody = new System.Windows.Forms.TableLayoutPanel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.lblProductsUrl = new System.Windows.Forms.Label();
            this.txtProductsUrl = new System.Windows.Forms.TextBox();
            this.lblHttpMethod = new System.Windows.Forms.Label();
            this.txtHttpMethod = new System.Windows.Forms.TextBox();
            this.lblNickname = new System.Windows.Forms.Label();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUserAgent = new System.Windows.Forms.Label();
            this.txtUserAgent = new System.Windows.Forms.TextBox();
            this.lblPrinterName = new System.Windows.Forms.Label();
            this.txtPrinterName = new System.Windows.Forms.TextBox();
            this.lblPrinterPort = new System.Windows.Forms.Label();
            this.txtPrinterPort = new System.Windows.Forms.TextBox();
            this.lblUsbVid = new System.Windows.Forms.Label();
            this.txtUsbVid = new System.Windows.Forms.TextBox();
            this.lblUsbPid = new System.Windows.Forms.Label();
            this.txtUsbPid = new System.Windows.Forms.TextBox();
            this.lblPrinterNamePriority = new System.Windows.Forms.Label();
            this.txtPrinterNamePriority = new System.Windows.Forms.TextBox();
            this.lblTypePrinter = new System.Windows.Forms.Label();
            this.txtTypePrinter = new System.Windows.Forms.TextBox();
            this.lblLogFilePath = new System.Windows.Forms.Label();
            this.txtLogFilePath = new System.Windows.Forms.TextBox();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.panelHeader.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(620, 56);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(170, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Configuracion";
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.panelButtons.Controls.Add(this.btnGuardar);
            this.panelButtons.Controls.Add(this.btnCancelar);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 560);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(620, 56);
            this.panelButtons.TabIndex = 2;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(350, 10);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 36);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancelar.Location = new System.Drawing.Point(480, 10);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 36);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // panelBody
            // 
            this.panelBody.AutoScroll = true;
            this.panelBody.ColumnCount = 2;
            this.panelBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.panelBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(0, 56);
            this.panelBody.Name = "panelBody";
            this.panelBody.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.panelBody.RowCount = 14;
            this.panelBody.Size = new System.Drawing.Size(620, 504);
            this.panelBody.TabIndex = 1;
            this.panelBody.Controls.Add(this.lblVersion, 0, 0);
            this.panelBody.Controls.Add(this.txtVersion, 1, 0);
            this.panelBody.Controls.Add(this.lblProductsUrl, 0, 1);
            this.panelBody.Controls.Add(this.txtProductsUrl, 1, 1);
            this.panelBody.Controls.Add(this.lblHttpMethod, 0, 2);
            this.panelBody.Controls.Add(this.txtHttpMethod, 1, 2);
            this.panelBody.Controls.Add(this.lblNickname, 0, 3);
            this.panelBody.Controls.Add(this.txtNickname, 1, 3);
            this.panelBody.Controls.Add(this.lblPassword, 0, 4);
            this.panelBody.Controls.Add(this.txtPassword, 1, 4);
            this.panelBody.Controls.Add(this.lblUserAgent, 0, 5);
            this.panelBody.Controls.Add(this.txtUserAgent, 1, 5);
            this.panelBody.Controls.Add(this.lblPrinterName, 0, 6);
            this.panelBody.Controls.Add(this.txtPrinterName, 1, 6);
            this.panelBody.Controls.Add(this.lblPrinterPort, 0, 7);
            this.panelBody.Controls.Add(this.txtPrinterPort, 1, 7);
            this.panelBody.Controls.Add(this.lblUsbVid, 0, 8);
            this.panelBody.Controls.Add(this.txtUsbVid, 1, 8);
            this.panelBody.Controls.Add(this.lblUsbPid, 0, 9);
            this.panelBody.Controls.Add(this.txtUsbPid, 1, 9);
            this.panelBody.Controls.Add(this.lblPrinterNamePriority, 0, 10);
            this.panelBody.Controls.Add(this.txtPrinterNamePriority, 1, 10);
            this.panelBody.Controls.Add(this.lblTypePrinter, 0, 11);
            this.panelBody.Controls.Add(this.txtTypePrinter, 1, 11);
            this.panelBody.Controls.Add(this.lblLogFilePath, 0, 12);
            this.panelBody.Controls.Add(this.txtLogFilePath, 1, 12);
            this.panelBody.Controls.Add(this.lblTimeout, 0, 13);
            this.panelBody.Controls.Add(this.numTimeout, 1, 13);
            // 
            // Etiquetas y campos
            // 
            this.ConfigLabel(this.lblVersion, "Version:");
            this.ConfigText(this.txtVersion);
            this.ConfigLabel(this.lblProductsUrl, "URL de productos:");
            this.ConfigText(this.txtProductsUrl);
            this.ConfigLabel(this.lblHttpMethod, "Metodo HTTP:");
            this.ConfigText(this.txtHttpMethod);
            this.ConfigLabel(this.lblNickname, "Usuario:");
            this.ConfigText(this.txtNickname);
            this.ConfigLabel(this.lblPassword, "Clave:");
            this.ConfigText(this.txtPassword);
            this.ConfigLabel(this.lblUserAgent, "User-Agent:");
            this.ConfigText(this.txtUserAgent);
            this.ConfigLabel(this.lblPrinterName, "Impresora:");
            this.ConfigText(this.txtPrinterName);
            this.ConfigLabel(this.lblPrinterPort, "Puerto (COM/LPT):");
            this.ConfigText(this.txtPrinterPort);
            this.ConfigLabel(this.lblUsbVid, "USB VID:");
            this.ConfigText(this.txtUsbVid);
            this.ConfigLabel(this.lblUsbPid, "USB PID:");
            this.ConfigText(this.txtUsbPid);
            this.ConfigLabel(this.lblPrinterNamePriority, "Impresora prioritaria:");
            this.ConfigText(this.txtPrinterNamePriority);
            this.ConfigLabel(this.lblTypePrinter, "Tipo impresora:");
            this.ConfigText(this.txtTypePrinter);
            this.ConfigLabel(this.lblLogFilePath, "Ruta del log:");
            this.ConfigText(this.txtLogFilePath);
            this.ConfigLabel(this.lblTimeout, "Timeout (seg):");
            // 
            // numTimeout
            // 
            this.numTimeout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numTimeout.Location = new System.Drawing.Point(183, 3);
            this.numTimeout.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            this.numTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(100, 25);
            this.numTimeout.TabIndex = 13;
            this.numTimeout.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.btnGuardar;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 616);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "v4posme - Configuracion";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelBody.ResumeLayout(false);
            this.panelBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.ResumeLayout(false);
        }

        /// <summary>Aplica estilo comun a una etiqueta de la columna izquierda.</summary>
        private void ConfigLabel(System.Windows.Forms.Label lbl, string text)
        {
            lbl.AutoSize = true;
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lbl.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lbl.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            lbl.Text = text;
        }

        /// <summary>Aplica estilo comun a un campo de texto de la columna derecha.</summary>
        private void ConfigText(System.Windows.Forms.TextBox txt)
        {
            txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            txt.Font = new System.Drawing.Font("Segoe UI", 10F);
            txt.Margin = new System.Windows.Forms.Padding(3, 3, 16, 6);
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.TableLayoutPanel panelBody;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Label lblProductsUrl;
        private System.Windows.Forms.TextBox txtProductsUrl;
        private System.Windows.Forms.Label lblHttpMethod;
        private System.Windows.Forms.TextBox txtHttpMethod;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUserAgent;
        private System.Windows.Forms.TextBox txtUserAgent;
        private System.Windows.Forms.Label lblPrinterName;
        private System.Windows.Forms.TextBox txtPrinterName;
        private System.Windows.Forms.Label lblPrinterPort;
        private System.Windows.Forms.TextBox txtPrinterPort;
        private System.Windows.Forms.Label lblUsbVid;
        private System.Windows.Forms.TextBox txtUsbVid;
        private System.Windows.Forms.Label lblUsbPid;
        private System.Windows.Forms.TextBox txtUsbPid;
        private System.Windows.Forms.Label lblPrinterNamePriority;
        private System.Windows.Forms.TextBox txtPrinterNamePriority;
        private System.Windows.Forms.Label lblTypePrinter;
        private System.Windows.Forms.TextBox txtTypePrinter;
        private System.Windows.Forms.Label lblLogFilePath;
        private System.Windows.Forms.TextBox txtLogFilePath;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown numTimeout;
    }
}
