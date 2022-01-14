using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMatrix;
namespace QRCodeGenerator
{
    internal class QRCodePrint
    {
        private Matrix matrix;
        public QRCodePrint(Matrix matrix)
        {
            this.matrix = matrix;
        }
        public Bitmap Print()
        {
            Bitmap bitmap = new Bitmap(matrix.Height+1, matrix.Width+1);
            for(int i = 1; i <= matrix.Width; i++)
            {
                for(int j = 1;j<= matrix.Height; j++)
                {
                    if(matrix[i-1,j-1] == 1)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                    if(matrix[i-1,j-1] == 2)
                    {
                        bitmap.SetPixel(i, j, Color.Blue);
                    }
                    if (matrix[i - 1, j - 1] == 0)
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                }
            }
            return bitmap;
        }
    }
}
