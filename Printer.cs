using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRCodeGenerator
{
    internal class Printer: PrintDocument
    {
        public Bitmap bitmap = null;
        public Printer(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }
        public void pr_PrintPage(object sender,PrintPageEventArgs e)
        {
            float paperHeight = (int)this.DefaultPageSettings.PrintableArea.Height;
            float paperWidth = (int)this.DefaultPageSettings.PrintableArea.Width;

            float size = paperWidth / 100.0f * 75.0f;

            e.Graphics.DrawImage(bitmap, (paperWidth / 2 - size / 2) + 5, paperHeight / 2 - size / 2, size, size);
            e.HasMorePages = false;
        }
    }
}
