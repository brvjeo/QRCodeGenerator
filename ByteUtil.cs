using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BitMatrix
{
    public class ByteUtil
    {
        private static BitArray CODE_METHOD = new BitArray(new Boolean[] { false, true, false, false });
        private static BitArray ADD_BYTE_1 = new BitArray(new Boolean[] { true, true, true, false, true, true, false, false });
        public static BitArray ADD_BYTE_2 = new BitArray(new Boolean[] { false, false, false, true, false, false, false, true });


        //*STATIC FIELDS*
        private short DATA_AMOUNT_FIELD
        {
            get
            {
                if (cur_version <= 9 && cur_version >= 1)
                {
                    return 8;
                }
                else
                {
                    return 16;
                }
            }
        }
        private static byte[] RemainderBits = new byte[]
        {
            0,
            0,7,7,7,7,7,0,0,0,0,0,0,0,
            3,3,3,3,3,3,3,4,4,4,4,4,4,4,
            3,3,3,3,3,3,3,0,0,0,0,0,0
        };
        private static Dictionary<byte, byte[]> POLYNOMICAL_TABLE = new Dictionary<byte, byte[]>(){      //checked twice
            [7] = new byte[] {87,229,146,149,238,102,21},
            [10] = new byte[] {251,67,46,61,118,70,64,94,32,45},
            [13] = new byte[] {74,152,176,100,86,100,106,104,130,218,206,140,78},
            [15] = new byte[] {8,183,61,91,202,37,51,58,58,237,140,124,5,99,105},
            [16] = new byte[] {120,104,107,109,102,161,76,3,91,191,147,169,182,194,225,120},
            [17] = new byte[] {43,139,206,78,43,239,123,206,214,147,24,99,150,39,243,163,136},
            [18] = new byte[] {215,234,158,94,184,97,118,170,79,187,152,148,252,179,5,98,96,153},
            [20] = new byte[] {17,60,79,50,61,163,26,187,202,180,221,225,83,239,156,164,212,212,188,190},



            [22] = new byte[] {210,171,247,242,93,230,14,109,221,53,200,74,8,172,98,80,219,134,160,105,165,231},
            [24] = new byte[] {229,121,135,48,211,117,251,126,159,180,169,152,192,226,228,218,111,0,117,232,87,96,227,21},
            [26] = new byte[] {173,125,158,2,103,182,118,17,145,201,111,28,165,53,161,21,245,142,13,102,48,227,153,145,218,70},
            [28] = new byte[] {168,223,200,104,224,234,108,180,110,190,195,147,205,27,232,201,21,43,245,87,42,195,212,119,242,37,9,123},
            [30] = new byte[] {41,173,145,152,216,31,179,182,50,48,110,86,239,96,222,125,42,173,226,193,224,130,156,37,251,216,238,40,192,180}
        };
        private static byte[] GALUA_FIELD = new byte[]
        {
            1,2,4,8,16,32,64,128,29,58,116,232,205,135,19,38,
            76,152,45,90,180,117,234,201,143,3,6,12,24,48,96,192,
            157,39,78,156,37,74,148,53,106,212,181,119,238,193,159,35,
            70,140,5,10,20,40,80,160,93,186,105,210,185,111,222,161,
            95,190,97,194,153,47,94,188,101,202,137,15,30,60,120,240,
            253,231,211,187,107,214,177,127,254,225,223,163,91,182,113,226,
            217,175,67,134,17,34,68,136,13,26,52,104,208,189,103,206,
            129,31,62,124,248,237,199,147,59,118,236,197,151,51,102,204,
            133,23,46,92,184,109,218,169,79,158,33,66,132,21,42,84,
            168,77,154,41,82,164,85,170,73,146,57,114,228,213,183,115,
            230,209,191,99,198,145,63,126,252,229,215,179,123,246,241,255,
            227,219,171,75,150,49,98,196,149,55,110,220,165,87,174,65,
            130,25,50,100,200,141,7,14,28,56,112,224,221,167,83,166,
            81,162,89,178,121,242,249,239,195,155,43,86,172,69,138,9,
            18,36,72,144,61,122,244,245,247,243,251,235,203,139,11,22,
            44,88,176,125,250,233,207,131,27,54,108,216,173,71,142,1
        };
        private static byte[] REVERSE_GALUA_FIELD = new byte[] 
        { 
            0,0,1,25,2,50,26,198,3,223,51,238,27,104,199,75,
            4,100,224,14,52,141,239,129,28,193,105,248,200,8,76,113,
            5,138,101,47,225,36,15,33,53,147,142,218,240,18,130,69,
            29,181,194,125,106,39,249,185,201,154,9,120,77,228,114,166,
            6,191,139,98,102,221,48,253,226,152,37,179,16,145,34,136,
            54,208,148,206,143,150,219,189,241,210,19,92,131,56,70,64,
            30,66,182,163,195,72,126,110,107,58,40,84,250,133,186,61,
            202,94,155,159,10,21,121,43,78,212,229,172,115,243,167,87,
            7,112,192,247,140,128,99,13,103,74,222,237,49,197,254,24,
            227, 165, 153 ,119 ,38 , 184, 180, 124 ,17,  68,  146 ,217, 35 , 32 , 137 ,46,
            55,  63,  209, 91 , 149, 188, 207 ,205, 144 ,135, 151, 178, 220, 252, 190, 97,
            242, 86 , 211 ,171, 20 , 42 , 93  ,158 ,132, 60  ,57 , 83,  71,  109 ,65 , 162,
            31 , 45,  67 , 216, 183, 123, 164, 118, 196, 23,  73 , 236, 127, 12,  111, 246,
            108, 161, 59,  82 , 41  ,157, 85 , 170 ,251 ,96,  134, 177 ,187 ,204, 62 , 90,
            203 ,89 , 95 , 176 ,156 ,169 ,160, 81,  11  ,245 ,22,  235 ,122 ,117, 44 , 215,
            79 , 174 ,213 ,233 ,230, 231, 173 ,232,116, 214 ,244, 234, 168, 80 , 88,  175
        };

        private static short[] AMOUNT_OF_BLOCKS = new short[]       //checked twice
        {
            0,
            1,1,1,2,2,4,4,4,5,5,
            5,8,9,9,10,10,11,13,14,16,
            17,17,18,20,21,23,25,26,28,29,
            31,33,35,37,38,40,43,45,47,49
        };
        private static short[] MAX_INFO_CAPACITY = new short[]      //checked twice
        {
            0,
            128,224,352,512,688,864,992,1232,1456,1728,
            2032,2320,2672,2920,3320,3624,4056,4504,5016,5352,
            5712,6256,6880,7312,8000,8496,9024,9544,10136,10984,
            11640,12328,13048,13800,14496,15312,15936,16816,17728,18672
        };
        private static byte[] BYTES_FOR_SINGLEBLOCK = new byte[]        //checked twice
        {   0,
            10,16,26,18,24,16,18,22,22,26,
            30,22,22,24,24,28,28,26,26,26,
            26,28,28,28,28,28,28,28,28,28,
            28,28,28,28,28,28,28,28,28,28
        };
        //


        private char[] chars;
        private BitArray bitArray;
        private byte cur_version;
        private byte cur_byte_correction;
        private int cur_length;
        private int cur_blocks_amount;
        private short cur_info_capacity;
        private BitArray start_bitArray;
        private byte[] finalDataArray;



        public byte GetVersion{ get { return cur_version; }}



        public ByteUtil(string text)
        {
            byte[] bytes;
            chars = text.ToCharArray();
            bytes = new UTF8Encoding().GetBytes(chars);
            bitArray = TranslateFromBytes(bytes);
            start_bitArray = bitArray;
            cur_length = bitArray.Length;
            cur_version = DefineCodeVersion(bitArray.Length);
        }

        private void BitArrayPushLeft(BitArray arrayPush)
        {
            int i = 0;
            BitArray updateArr = new BitArray(bitArray.Length + arrayPush.Length);
            for(; i < arrayPush.Length; i++)
            {
                updateArr[i] = arrayPush[i];
            }
            for(int j = 0; j < bitArray.Length; j++, i++)
            {
                updateArr[i] = bitArray[j];
            }

            bitArray = updateArr;
        }
        private void BitArrayPushRight(BitArray arrayPush)
        {
            int i = 0;
            BitArray updateArr = new BitArray(bitArray.Length + arrayPush.Length);
            for (; i < bitArray.Length; i++)
            {
                updateArr[i] = bitArray[i];
            }
            for (int j = 0; j < arrayPush.Length; j++, i++)
            {
                updateArr[i] = arrayPush[j];
            }

            bitArray = updateArr;
        }
        public byte[][] DefineBlockAmount()
        {
            byte[] byteArray = TranslateToBytes(bitArray);
            int i = 0, j = 0, k = 0;

            int rem = (cur_length / 8) % AMOUNT_OF_BLOCKS[cur_version];
            int amount = (cur_length / 8) / AMOUNT_OF_BLOCKS[cur_version];

            byte[][] byteBlocks = new byte[AMOUNT_OF_BLOCKS[cur_version]][];

            for(;i < AMOUNT_OF_BLOCKS[cur_version] - rem; i++)
            {
                byteBlocks[i] = new byte[amount];
            }
            for (; j < rem; j++, i++)
            {
                byteBlocks[i] = new byte[amount + 1];
            }

            i = 0;

            for(;i<AMOUNT_OF_BLOCKS[cur_version] - rem; i++)
            {
                for (j = 0; j < amount; j++)
                {
                    byteBlocks[i][j] = byteArray[k++];
                }
            }
            for (; i < AMOUNT_OF_BLOCKS[cur_version]; i++)
            {
                for(j = 0; j < amount+1; j++)
                {
                    byteBlocks[i][j] = byteArray[k++];
                }
            }
            return byteBlocks;
        }
        private byte DefineCodeVersion(int length)
        {
            byte i = 1;
            while(MAX_INFO_CAPACITY[i] <= length)
            {
                i++;
            }
            cur_info_capacity = MAX_INFO_CAPACITY[i];
            return i;
        }
        private void InsertTechnicalBytes()
        {
            BitArray dataLenAmount = CalculateDataAmount();
            BitArrayPushLeft(dataLenAmount);
            BitArrayPushLeft(CODE_METHOD);
            cur_length = bitArray.Length;
            if (cur_length > cur_info_capacity)
            {
                UpdateVersion();
            }
        }
        private void FillZeros()
        {
            int differ = (8 - cur_length % 8);
            BitArray zeroArray = new BitArray(differ, false);
            BitArrayPushRight(zeroArray);
            cur_length = bitArray.Length;
        }
        private void FillAdditionalBytes()
        {
            while(bitArray.Length < cur_info_capacity)
            {
                if (bitArray.Length < cur_info_capacity)
                {
                    BitArrayPushRight(ADD_BYTE_1);
                }
                else
                {
                    break;
                }
                if(bitArray.Length < cur_info_capacity)
                {
                    BitArrayPushRight(ADD_BYTE_2);
                }
                else
                {
                    break;
                }
            }
            cur_length = bitArray.Length;
        }
        private void UpdateVersion()
        {
            cur_version++;
            while(MAX_INFO_CAPACITY[cur_version] < cur_length)
            {
                cur_version++;
            }
            cur_info_capacity = MAX_INFO_CAPACITY[cur_version];
            bitArray = start_bitArray;
            cur_length = bitArray.Length;
            InsertTechnicalBytes();
        }
        public byte[][] ByteCorrection(byte[][] byteBlocks)
        {
            byte[] draft;
            byte[][] correctedBlocks = new byte[cur_blocks_amount][];
            for(int i = 0;i < cur_blocks_amount; i++)
            {
                draft = GetCorrectedArray(byteBlocks[i]);
                correctedBlocks[i] = new byte[draft.Length];
                correctedBlocks[i] = draft;
            }
            return correctedBlocks;
        }
        public byte[] GetCorrectedArray(byte[] initArr)
        {
            byte corBlocksAmount = BYTES_FOR_SINGLEBLOCK[cur_version];
            cur_byte_correction = corBlocksAmount;
            byte[] polynom = POLYNOMICAL_TABLE[corBlocksAmount];
            int A, B, C;

            int corrLength = corBlocksAmount > initArr.Length ? corBlocksAmount : initArr.Length;
            byte[] correctedArray = new byte[corrLength];


            for (int i = 0; i < corrLength; i++)
            {
                if(i > initArr.Length - 1)
                {
                    correctedArray[i] = 0;
                }
                else
                {
                    correctedArray[i] = initArr[i];
                }

            }
            for(int i = 0; i < initArr.Length; i++)
            {
                A = correctedArray[0];
                ShiftLeft(correctedArray);
                if (A == 0) continue;
                B = REVERSE_GALUA_FIELD[A];
                for(int j = 0; j < corBlocksAmount; j++)
                {
                    C = polynom[j] + B;
                    if (C > 254)
                    {
                        C = C % 255;
                    }
                    correctedArray[j] = (byte)(GALUA_FIELD[C] ^ correctedArray[j]);
                }
            }
            byte[] finalArray = new byte[corBlocksAmount];
            for (int i = 0; i < corBlocksAmount; i++)
            {
                finalArray[i] = correctedArray[i];
            }
            return finalArray;
        }       //algorithm right;
        public void ShiftLeft(byte[] array)
        {
            for(int i = 0; i < array.Length - 1; i++)
            {
                array[i] = array[i + 1];
            }
            array[array.Length - 1] = 0;
        }
        public byte[] CombineByteBlocks(byte[][] bytes,byte[][] corrects)
        {
            int length = 0;
            for(int i = 0; i < bytes.Length; i++)
            {
                length += bytes[i].Length + corrects[i].Length;
            }
            byte[] finalBytes = new byte[length];
            int k = 0;
            for(int i = 0; i < bytes[bytes.Length - 1].Length; i++)
            {
                for(int j = 0; j < bytes.Length; j++)
                {
                    if (bytes[j].Length <= i)
                    {
                        continue;
                    }
                    else
                    {
                        finalBytes[k++] = bytes[j][i];
                    }
                }
            }
            for (int i = 0; i < corrects[corrects.Length - 1].Length; i++)
            {
                for (int j = 0; j < corrects.Length; j++)
                {
                    if (corrects[j].Length <= i)
                    {
                        continue;
                    }
                    else
                    {
                        finalBytes[k++] = corrects[j][i];
                    }
                }
            }
            return finalBytes;
        }
        public byte[] GenerateBitArray()
        {
            byte[][] bytes = null;
            byte[][] correctedBytes = null;
            InsertTechnicalBytes();
            if((cur_length%8) != 0) FillZeros();
            if (cur_info_capacity > cur_length) FillAdditionalBytes();
            if (AMOUNT_OF_BLOCKS[cur_version] > 1)
            {
                bytes = DefineBlockAmount();
            }
            else
            {
                bytes = new byte[1][];
                bytes[0] = TranslateToBytes(bitArray);
            }
            cur_blocks_amount = AMOUNT_OF_BLOCKS[cur_version];
            correctedBytes = ByteCorrection(bytes);
            Console.WriteLine("Data blocks:");
            for(int i = 0; i < bytes.Length; i++)
            {
                GetArray(bytes[i]);
            }
            Console.WriteLine("Corrected blocks:");
            for (int i = 0; i < correctedBytes.Length; i++)
            {
                GetArray(correctedBytes[i]);
            }
            Console.WriteLine("\n");

            byte[] finalBytes = CombineByteBlocks(bytes, correctedBytes);
            string finalString = "";
            for(int i = 0; i < finalBytes.Length; i++)
            {
                finalString += ConvertToString(finalBytes[i], 2, 8);
            }
            for(int i = 0; i < RemainderBits[cur_version]; i++)
            {
                finalString += "0";
            }
            finalDataArray = new byte[finalString.Length];
            for(int i = 0; i < finalString.Length; i++)
            {
                finalDataArray[i] = (byte)(finalString[i] == '1' ? 1 : 0);
            }
            Console.WriteLine("FinalBytes");
            GetArray(finalDataArray);
            GetInfo();
            return finalDataArray;
        }
        private BitArray CalculateDataAmount()                              //one of left techical parts. ENTIRE = CODEMETHOD + DATAAMOUNT + bitArray;
        {
            BitArray dataAmount = GetNumBinary(cur_length/8,DATA_AMOUNT_FIELD);
            return dataAmount;
        }

        //TECHNICAL FUNCTIONS
        public static void GetArray(byte[] array)
        {
            Console.Write("LENGTH:{0}   ", array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i] + " ");
            }
            Console.WriteLine("\n\n");
        }
        public static BitArray GetNumBinary(int x,int length)               //translate to binary system;
        {
            string s = Convert.ToString(x,2);
            while(s.Length < length)
            {
                s = "0" + s;
            }

            BitArray bitArray = new BitArray(s.Length,false);
            for(int i = 0; i < s.Length; i++)
            {
                if (s[i] == '1') bitArray[i] = true;
            }
            return bitArray;
        }
        public static string BitArrayToString(BitArray array)
        {
            string s = "";
            for(int i = 0;i < array.Length; i++)
            {
                if (array[i])
                {
                    s += "1";
                }
                else
                {
                    s += "0";
                }
            }
            return s;
        }
        public static byte[] TranslateToBytes(BitArray arr)
        {
            int k = 0;
            string s = "";
            byte[] bytes = new byte[arr.Length/8];
            for(int i = 0;i < arr.Length; i++)
            {
                if((i+1) % 8 == 0)
                {
                    s = s + Convert.ToByte(arr[i]).ToString();
                    bytes[k] = Convert.ToByte(s, 2);
                    k++;
                    s = "";
                }
                else
                {
                    s = s + Convert.ToByte(arr[i]).ToString();
                }
                
            }
            return bytes;
        }
        public static BitArray TranslateFromBytes(byte[] bytes)
        {
            string s = "";
            for(int i = 0; i < bytes.Length; i++)
            {
                s += ConvertToString(bytes[i],2,8);
            }
            BitArray bitArray = new BitArray(s.Length, false);
            for(int i = 0; i < s.Length; i++)
            {
                if (s[i] == '1') bitArray[i] = true;
            }
            return bitArray;
        }
        public static string ConvertToString(byte x,int format,int length)
        {
            string s = Convert.ToString(x, format);
            while(s.Length < length)
            {
                s = "0" + s;
            }
            return s;
        }
        public void GetInfo()
        {
            Console.WriteLine("START_ARRAY:{0},LENGTH:{1}", BitArrayToString(start_bitArray),start_bitArray.Length);
            Console.WriteLine("CUR_LENGTH:{0}", cur_length);
            Console.WriteLine("CUR_VERSION:{0}", cur_version);
            Console.WriteLine("CUR_INFO_CAPACITY:{0}", cur_info_capacity);
            Console.WriteLine("CUR_ARRAY:{0},LENGTH:{1}", BitArrayToString(bitArray),bitArray.Length);
            Console.WriteLine("CUR_BLOCKS_AMOUNT:{0}",cur_blocks_amount);
            Console.WriteLine("CUR_BYTES_CORRECTION:{0}",cur_byte_correction);

            Console.WriteLine("\n\n");
        }
    }
}
