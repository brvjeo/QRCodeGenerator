using System;
using System.Drawing;
using System.IO;

namespace BitMatrix
{
    public class Matrix
    {
        private string path;
        private static int[] MatrixSize = new int[]
        {
            0,21,25,29,33,37,41,45,49,53,57,61,
            65,69,73,77,81,85,89,93,97,101,105,109,
            113,117,121,125,129,
            133,137,141,145,149,153,157,161,165,
            169,173,177
        };
        private static sbyte[,] AlignmentPattern = new sbyte[,]
        {
            {1,1,1,1,1},
            {1,0,0,0,1},
            {1,0,1,0,1},
            {1,0,0,0,1},
            {1,1,1,1,1},
        };
        private static short[][] ALIGNMENTPOSITIONS = new short[][]
        {
            new short[]{},
            new short[]{},
            new short[]{18},
            new short[]{22},
            new short[]{26},
            new short[]{30},
            new short[]{34},
            new short[]{6,22,38},
            new short[]{6,24,42},
            new short[]{6,26,46},
            new short[]{6,28,50},
            new short[]{6,30,54},
            new short[]{6,32,58},
            new short[]{6,34,62},
            new short[]{6, 26, 46, 66},
            new short[]{6, 26, 48, 70},
            new short[]{6, 26, 50, 74},
            new short[]{6, 30, 54, 78},
            new short[]{6, 30, 56, 82},
            new short[]{6, 30, 58, 86},
            new short[]{6, 34, 62, 90},
            new short[]{6, 28, 50, 72, 94},
            new short[]{6, 26, 50, 74, 98},
            new short[]{6, 30, 54, 78, 102},
            new short[]{6, 28, 54, 80, 106},
            new short[]{6, 32, 58, 84, 110},
            new short[]{6, 30, 58, 86, 114},
            new short[]{6, 34, 62, 90, 118},
            new short[]{6, 26, 50, 74, 98, 122},
            new short[]{6, 30, 54, 78, 102, 126},
            new short[]{6, 26, 52, 78, 104, 130},
            new short[]{6, 30, 56, 82, 108, 134},
            new short[]{6, 34, 60, 86, 112, 138},
            new short[]{6, 30, 58, 86, 114, 142},
            new short[]{6, 34, 62, 90, 118, 146},
            new short[]{6, 30, 54, 78, 102, 126, 150},
            new short[]{6, 24, 50, 76, 102, 128, 154},
            new short[]{6, 28, 54, 80, 106, 132, 158},
            new short[]{6, 32, 58, 84, 110, 136, 162},
            new short[]{6, 26, 54, 82, 110, 138, 166},
            new short[]{6, 30, 58, 86, 114, 142, 170},
        };
        private static sbyte[,] FinderPattern = new sbyte[,]
        {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,1,1,1,0,1},
            {1,0,1,1,1,0,1},
            {1,0,1,1,1,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        };
        private static string[] VersionCodes = new string[]
        {
            "",
            "","","","","","",   //1-6
            "000010011110100110",//7
            "010001011100111000",//8
            "110111011000000100",//9
            "101001111110000000",//10
            "001111111010111100",//11
            "001101100100011010",//12
            "101011100000100110",//13
            "110101000110100010",//14
            "010011000010011110",//15
            "011100010001011100",//16
            "111010010101100000",//17
            "100100110011100100",//18
            "000010110111011000",//19
            "000000101001111110",//20
            "100110101101000010",//21
            "111000001011000110",//22
            "011110001111111010",//23
            "001101001101100100",//24
            "101011001001011000",//25
            "110101101111011100",//26
            "010011101011100000",//27
            "010001110101000110",//28
            "110111110001111010",//29
            "101001010111111110",//30
            "001111010011000010",//31
            "101000011000101101",//32
            "001110011100010001",//33
            "010000111010010101",//34
            "110110111110101001",//35
            "110100100000001111",//36
            "010010100100110011",//37
            "001100000010110111",//38
            "101010000110001011",//39
            "111001000100010101" //40
        }; 
        private static string MaskPattern = "101010000010010";
        private sbyte[,] matrix;
        private byte[] dataBits;
        private int width;
        private int height;
        private int version;
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public Matrix(byte[] dataBits,int version)
        {
            path = new DirectoryInfo(@"..\..").FullName;
            width = MatrixSize[version];
            height = MatrixSize[version];
            this.dataBits = dataBits;
            this.version = version;            
            matrix = new sbyte[width,height];
            FillMatrix();
            EmbedTechnicalInfo();
            EmbedDataBits();
            if(version>=7) EmbedVersionCodes();
            EmbedFormatCodes();
            if (version >= 6 && version < 16)
            {
                EmbedPicture(1);
            }
            if (version>=16)
            {
                EmbedPicture(2);
            }
        }

        private void EmbedPicture(int type)
        {
            Bitmap bitmap;
            if (type == 1)
            {
                bitmap = new Bitmap(path + @"\src\" + "57.bmp");
            }
            else
            {
                bitmap = new Bitmap(path + @"\src\" + "eagle.bmp");
            }
            int y = height / 2 - bitmap.Height / 2;
            int x = width / 2 - bitmap.Width / 2;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if ((bitmap.GetPixel(i, j) == Color.FromArgb(255, 255, 0, 0)))
                    {
                        continue;
                    }
                    if (bitmap.GetPixel(i, j) == Color.FromArgb(255, 0, 0, 0))
                    {
                        matrix[x + i, y + j] = 1;
                    }
                    else if (bitmap.GetPixel(i, j) == Color.FromArgb(255, 255, 255, 255))
                    {
                        matrix[x + i, y + j] = 0;
                    }
                }
            }
        }

        private void EmbedFormatCodes()
        {
            int index = 0;
            //LeftTop
            for(int i = 0; i < 9;i++)
            {
                if (i == 6) continue;
                matrix[i, 8] = (sbyte)(MaskPattern[index++] == '1' ? 1 : 0);
            }
            for(int i = 7; i >= 0; i--)
            {
                if (i == 6) continue;
                matrix[8,i] = (sbyte)(MaskPattern[index++] == '1' ? 1 : 0);
            }
            //
            //LeftBottom + Right Top
            index = 0;
            for(int i = 0; i < 7; i++)
            {
                matrix[8,height - 1 - i] = (sbyte)(MaskPattern[index++] == '1' ? 1 : 0);
            }
            for(int i = 8; i > 0; i--)
            {
                matrix[width - i,8] = (sbyte)(MaskPattern[index++] == '1' ? 1 : 0);
            }
            //
        }

        public sbyte this[int x,int y]
        {
            get
            {
                return matrix[x,y];
            }
            set
            {
                matrix[x,y] = value;
            }
        }
        public void FillMatrix()
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    matrix[i, j] = -1;
                }
            }
        }
        public void EmbedTechnicalInfo()
        {
            EmbedFinderPatternsAndSeparators();
            if (version >= 2) EmbedAlignmentPatterns();
            EmbedSynchronizationLines();
            EmbedDarkModule();
            ReserveInfoArea();
        }

        private void ReserveInfoArea()
        {
            ReserveFormatArea();
            if(version >= 7) ReserveVersionArea();
        }
        private void ReserveFormatArea()
        {
            for(int i = 0; i < 9; i++)
            {
                if (i < 7) matrix[8, height - 7 + i] = (sbyte)(matrix[8, height - 7 + i] == -1 ? 2 : matrix[8, height - 7 + i]);

                if (i < 8)
                {
                    matrix[width - i - 1, 8] = 2;
                    matrix[8, i] = (sbyte)(matrix[8, i] == -1 ? 2 : matrix[8, i]);
                }

                matrix[i, 8] = (sbyte)(matrix[i, 8] == -1 ? 2 : matrix[i, 8]);
            }
        }
        private void ReserveVersionArea()
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    matrix[j, width - 9 - i] = 2;
                    matrix[height - 9 - i, j] = 2;
                }
            }
        }
        private void EmbedDarkModule()
        {
            matrix[8, (4 * version) + 9] = 1;
        }

        private void EmbedSynchronizationLines()
        {
            sbyte[] lines = new sbyte[height - 14];
            for(int i = 0; i < lines.Length; i++)
            {
                if (i % 2 == 0) lines[i] = 1;
            }
            for(int i = 0; i < lines.Length; i++)
            {
                matrix[6, 6 + i] = lines[i];
                matrix[6 + i, 6] = lines[i];
            }
        }

        private void EmbedAlignmentPatterns()
        {
            Point[] points;
            short[] array = ALIGNMENTPOSITIONS[version];
            if (version > 6)
            {
                points = new Point[array.Length * array.Length-3];
            }
            else
            {
                points = new Point[array.Length * array.Length];
            }

            int k = 0;
            for(int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if(matrix[array[i],array[j]] == -1) points[k++] = new Point(array[i],array[j]); 
                }
            }
            int kx = 0, ky = 0,b = 0,a = 0;
            for(int i = 0; i < points.Length; i++)
            {
                a = 0;
                b = 0;
                kx = points[i].X-2;
                ky = points[i].Y-2;
                for(int x = kx; x < kx + 5; x++)
                {
                    b = 0;
                    for(int y = ky;y < ky + 5; y++)
                    {
                        matrix[x, y] = AlignmentPattern[a, b];
                        b++;
                    }
                    a++;
                }
            }
        }

        public void EmbedFinderPatternsAndSeparators()
        {
            for(int i = 0; i < 7; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    //Left Top
                    matrix[i, j] = FinderPattern[i, j];
                    //Right Top
                    matrix[width - 7 + i, j] = FinderPattern[i, j];
                    //Left Bottom
                    matrix[i, height - 7 + j] = FinderPattern[i, j];
                }
            }
            for (int i = 0; i < 8; i++)
            {
                //Left Top
                matrix[i, 7] = 0;
                matrix[7, i] = 0;
                //Right Top
                matrix[width - 8, i] = 0;
                matrix[i,height - 8] = 0;
                //Left Bottom
                matrix[7, height - i-1] = 0;
                matrix[width - i-1, 7] = 0;
            }
        }
        public void EmbedVersionCodes()
        {
            string codeVersion = VersionCodes[version];
            int index = 0;
            for (int i = 2; i >= 0; i--)
            {
                for (int j = 0; j < 6; j++)
                {
                    matrix[j, width - 9 - i] = ((sbyte)(codeVersion[index] == '1' ? 1 : 0));
                    matrix[height - 9 - i, j] = ((sbyte)(codeVersion[index] == '1' ? 1 : 0));
                    index++;
                }
            }
        }
        public void EmbedDataBits()
        {
            int bitIndex = 0;

            int direction = -1;

            int x = width - 1;
            int y = height - 1;

            while (x > 0)
            {
                if(x == 6)
                {
                    x -= 1;
                }
                while (y >= 0 && y < height)
                {
                    for(int i = 0; i < 2; ++i)
                    {
                        int xx = x - i;

                        if (!IsEmpty(xx,y))
                        {
                            continue;
                        }
                        matrix[xx, y] = SetMask(xx,y, (sbyte)dataBits[bitIndex]);
                        bitIndex++;
                    }
                    y += direction;
                }
                direction = -direction;
                y += direction;
                x -= 2;
            }
            Console.WriteLine(bitIndex);
            Console.WriteLine(dataBits.Length);
        }
        public bool IsEmpty(int x,int y)
        {
            if(matrix[x,y] == -1) return true;
            return false;
        }
        public sbyte SetMask(int x,int y,sbyte module)
        {
            sbyte mod = 0;
            if((x+y)%2 == 0)
            {
                mod = (sbyte)((module == 0) ? 1 : 0);
            }
            else
            {
                mod = (sbyte)module;
            }
            return mod;
        }
        public void PrintConsole()
        {
            for(int i = 0; i < width; i++)
            {
                Console.WriteLine();
                for(int j = 0; j < height; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
        }
    }
}