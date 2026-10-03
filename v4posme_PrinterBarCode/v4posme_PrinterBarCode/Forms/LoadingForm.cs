using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Ventana sin bordes que muestra un mensaje de "Cargando..." mientras
    /// se descarga la informacion inicial.
    /// </summary>
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        /// <summary>Actualiza el texto de estado de forma segura entre hilos.</summary>
        public void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatus), text);
                return;
            }
            lblStatus.Text = text;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Borde sutil alrededor de la ventana.
            using (var pen = new Pen(Color.FromArgb(210, 210, 210), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
