using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using BitMatrix;
using System.IO;
using System.Drawing.Imaging;

namespace QRCodeGenerator
{
    public partial class Form1 : Form
    {
        private Bitmap QRCode;
        private Matrix matrix;


        private string path;
        private string dataText;
        private Printer printer;
        private Form dateForm;
        private MonthCalendar calendar;
        private Button dateButton;
        private string choosenDate;
        
        private ByteUtil byteUtil;
        public Form1()
        {
            InitializeComponent();

            QRCode = null;
            matrix = null;
            choosenDate = "";
            path = new DirectoryInfo(@"..\..").FullName;
            //textbox
            textBox1.Location = new Point(this.Width/2 - textBox1.Width/2, this.Height/2 + textBox1.Height*2);
            textBox1.MaxLength = 100;
            //
            //panel
            panel1.Location = new Point(this.Width / 2 - panel1.Width / 2, textBox1.Top - textBox1.Height * 2 - panel1.Height);
            panel1.Height = this.Height / 2 - textBox1.Height;
            panel1.Dock = DockStyle.Top;
            panel1.BackColor = Color.White;
            //
            //button
            button1.Location = new Point(this.Width / 2 - button1.Width / 2, textBox1.Top + textBox1.Height * 2);
            //

            QRCode = new Bitmap(path + @"\src\" + @"startCode.bmp");
            DrawQRCodeAsBitmap();


            this.Resize += new EventHandler(Form1_Resize);
            panel1.Paint += Panel1_Paint;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            if(QRCode != null) DrawQRCodeAsBitmap();
        }
        private void Form1_Resize(object sender,EventArgs a)
        {
            if (QRCode != null) DrawQRCodeAsBitmap(); 
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(QRCode != null)
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.ShowDialog();

                Bitmap resized = new Bitmap(QRCode.Width * 10, QRCode.Height * 10);

                Graphics g = Graphics.FromImage(resized);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(QRCode, 0, 0, QRCode.Width * 10, QRCode.Width * 10);
                resized.Save(saveFile.FileName + ".png",ImageFormat.Png);
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }
        private void printerPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (QRCode != null)
            {
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = printer;
                previewDialog.ShowDialog();
            }
        }

        private void printerSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (QRCode != null)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printer;
                printDialog.ShowDialog();
            }
        }

        private void printDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(QRCode != null) printer.Print();
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            choosenDate = null;
            dateForm = new Form();
            dateForm.SuspendLayout();
            dateForm.Icon = this.Icon;
            dateForm.MaximizeBox = false;
            dateForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            dateForm.FormClosed += DateForm_FormClosed;

            calendar = new MonthCalendar();
            calendar.Location = new Point(dateForm.Width/2 - calendar.Width/2,dateForm.Height/2 - calendar.Height/2 - 40);
            calendar.DateSelected += new DateRangeEventHandler(calendar_DateSelector);

            dateButton = new Button();
            dateButton.Text = "Choose";
            dateButton.TextAlign = ContentAlignment.MiddleCenter;
            dateButton.Location = new Point(dateForm.Width / 2 - dateButton.Width / 2 - 5, calendar.Top + calendar.Height + dateButton.Height);
            dateButton.Click += dateButton_Click;

            dateForm.Controls.Add(dateButton);
            dateForm.Controls.Add(calendar);
            dateForm.ResumeLayout(true);
            dateForm.ShowDialog();
        }

        private void DateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            dateForm.Dispose();
        }

        private void calendar_DateSelector(object sender,DateRangeEventArgs e)
        {
            choosenDate = e.End.ToShortDateString();
        }
        private void dateButton_Click(object sender, EventArgs e)
        {
            if(choosenDate != null)
            {
                GenerateQRCode(choosenDate);
                dateForm.Dispose();
            }
            else
            {
                MessageBox.Show("Date is not choosen!");
            }
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text files (*.txt)|*.txt";
            openFile.Multiselect = false;
            openFile.FileOk += OpenFile_FileOk;
            openFile.ShowDialog();
        }

        private void OpenFile_FileOk(object sender, CancelEventArgs e)
        {
            OpenFileDialog obj = (OpenFileDialog)sender;
            string path = obj.FileName;
            string text = File.ReadAllText(path, Encoding.UTF8);
            if (text.Length > 2330)
            {
                MessageBox.Show("Too Much Information");
                return;
            }
            GenerateQRCode(text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            string text = "";

            if (s.Length == 0)
            {
                MessageBox.Show("Empty");
                return;
            }
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i] == ' ' & s[i + 1] == ' ')
                {
                    continue;
                }
                else
                {
                    text += s[i];
                }
            }
            if (s[s.Length - 1] != ' ') text += s[s.Length - 1];
            if (s[0] == ' ') text = text.Substring(1);
            dataText = text;
            if(dataText.Length < 2330)
            {
                GenerateQRCode(dataText);
            }
            else
            {
                MessageBox.Show("Too much information");
            }
        }

        private void GenerateQRCode(string text)
        {
            byteUtil = new ByteUtil(text);
            byte[] dataBits = byteUtil.GenerateBitArray();

            matrix = new Matrix(dataBits, byteUtil.GetVersion);

            QRCodePrint qrPrinter = new QRCodePrint(matrix);
            QRCode = qrPrinter.Print();

            DrawQRCodeAsBitmap();
            //DrawQRCodeAsPaintings(matrix);

            printer = new Printer(QRCode);
            printer.PrintPage += new PrintPageEventHandler(printer.pr_PrintPage);

        }
        private void DrawQRCodeAsPaintings(Matrix matrix)
        {
            Graphics g = Graphics.FromHwnd(panel1.Handle);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            float areaSize = panel1.Height / 100 * 90;
            float moduleSize = areaSize / matrix.Height;

            float paddingTop = panel1.Height / 2 - areaSize / 2;
            float paddingLeft = panel1.Width / 2 - areaSize / 2;

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        g.FillEllipse(Brushes.Black, paddingLeft, paddingTop, moduleSize, moduleSize);
                        paddingTop += moduleSize;
                    }
                    else
                    {
                        g.FillEllipse(Brushes.White, paddingLeft, paddingTop, moduleSize, moduleSize);
                        paddingTop += moduleSize;
                    }
                }
                paddingTop -= moduleSize * matrix.Height;
                paddingLeft += moduleSize;
            }
        }
        private void DrawQRCodeAsBitmap()
        {
            if(QRCode != null)
            {
                int panelHeight = panel1.Height;
                int panelWidth = panel1.Width;

                int size = panelHeight / 100 * 90;

                Graphics g = Graphics.FromHwnd(panel1.Handle);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(QRCode, (panelWidth / 2 - size / 2) + 5, panelHeight / 2 - size / 2, size, size);
            }
        }
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данное приложение поддерживает кодировку UTF-8.\n\n(Если ваш сканер правильно не декодирует первоначальную информацию, значит сканер и генератор поддерживают разные кодировки,попробуйте другой сканер.)\nКод сгенерированный данным генератором поддерживает визуальную потерю 15% информации.Это означает,что при потере 15% процентов кода, первоначальная информация будет прочитана верно.\n\nБлагодаря этой особенности внуть кода можно поместить изображение.");

        }
    }
}
