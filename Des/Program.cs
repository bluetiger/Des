using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Des
{

    class Program
    {

        static readonly int[,] s1 = new int[,]
            {
            { 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7 },
            { 0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8 },
            { 4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0 },
            { 15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13 }
            };
        static readonly int[,] s2 = new int[,]
            {
            { 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10 },
            { 3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5 },
            { 0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15 },
            { 13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9 }
            };
        static readonly int[,] s3 = new int[,]
            {
            { 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8 },
            { 13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1 },
            { 13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7 },
            { 1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12 }
            };
        static readonly int[,] s4 = new int[,]
            {
            { 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15 },
            { 13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9 },
            { 10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4 },
            { 3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14 },

            };
        static readonly int[,] s5 = new int[,]
            {
            { 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9 },
            { 14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6 },
            { 4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14 },
            { 11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3 }

            };
        static readonly int[,] s6 = new int[,]
            {
            { 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11 },
            { 10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8 },
            { 9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6 },
            { 4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13 }

            };
        static readonly int[,] s7 = new int[,]
            {
            { 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1 },
            { 13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6 },
            { 1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2 },
            { 6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12 }
            };
        static readonly int[,] s8 = new int[,]
            {
            { 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7 },
            { 1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2 },
            { 7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8 },
            { 2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11 }
            };
        static void Main(string[] args)
        {
            //準備
            var target = "abcdefgh";//64bit文字列
            var target_binary = StringToBoolArray(target);
            Console.WriteLine("plane:" + target);
            BoolArrayWriteLine(target_binary, "target(binary):");

            //初期転置
            target_binary = FirstTranspose(target_binary);

            var target_binary_left = target_binary.Take(32).ToArray();
            var target_binary_right = target_binary.Skip(32).ToArray();
            var target_binary_right_copy = new bool[32];
            //key生成
            var key_list = GenerateRandomKey();

            var count = 0;
            while (count != 16)
            {
                BoolArrayWriteLine(target_binary_left, "L(" + count + "):");
                BoolArrayWriteLine(target_binary_right, "R(" + count + "):");
                target_binary_right.CopyTo(target_binary_right_copy, 0);

                var expansion_right = Expansion(target_binary_right);
                var xor_result = XORBoolArray(expansion_right, key_list[count]);
                var subsitution = Substitution(xor_result);
                var next_right = Transpose32(subsitution.ToArray());
                next_right = XORBoolArray(target_binary_left, next_right);
                target_binary_right_copy.CopyTo(target_binary_left,0);
                next_right.CopyTo(target_binary_right,0);
                count++;
            }
            BoolArrayWriteLine(target_binary_left, "L(" + count + "):");
            BoolArrayWriteLine(target_binary_right, "R(" + count + "):");

            var right_tmp_list = target_binary_right.ToList();
            right_tmp_list.AddRange(target_binary_left.ToList());
            //最終転置
            var result = LastTranspose(right_tmp_list.ToArray());
            BoolArrayWriteLine(result, "encrypt:");
        }

        //string⇒bool[]
        static bool[] StringToBoolArray(string target)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(target);
            var sp = Common.SplitBytes(bytes);
            var target_binary = new bool[64];
            var count = 0;
            foreach (var v in sp.Item1)
            {
                foreach (var v2 in Convert.ToString(v, 2).PadLeft(8, '0').ToArray())
                {
                    target_binary[count] = v2 == '1';
                    count++;
                }
            }
            foreach (var v in sp.Item2)
            {
                foreach (var v2 in Convert.ToString(v, 2).PadLeft(8, '0').ToArray())
                {
                    target_binary[count] = v2 == '1';
                    count++;
                }
            }
            return target_binary;
        }
        //換字変換
        static bool[] Substitution(bool[] target)
        {
            var s1_result = FunctionS(target.Take(6).ToArray(), s1).ToList();
            var s2_result = FunctionS(target.Skip(6).Take(6).ToArray(), s2).ToList();
            var s3_result = FunctionS(target.Skip(12).Take(6).ToArray(), s3).ToList();
            var s4_result = FunctionS(target.Skip(18).Take(6).ToArray(), s4).ToList();
            var s5_result = FunctionS(target.Skip(24).Take(6).ToArray(), s5).ToList();
            var s6_result = FunctionS(target.Skip(30).Take(6).ToArray(), s6).ToList();
            var s7_result = FunctionS(target.Skip(36).Take(6).ToArray(), s7).ToList();
            var s8_result = FunctionS(target.Skip(42).Take(6).ToArray(), s8).ToList();
            s1_result.AddRange(s2_result);
            s1_result.AddRange(s3_result);
            s1_result.AddRange(s4_result);
            s1_result.AddRange(s5_result);
            s1_result.AddRange(s6_result);
            s1_result.AddRange(s7_result);
            s1_result.AddRange(s8_result);
            return s1_result.ToArray();
        }
        //初期転置
        static bool[] FirstTranspose(bool[] binary)
        {
            var transposed = new bool[64];
            transposed[0] = binary[57];
            transposed[1] = binary[49];
            transposed[2] = binary[41];
            transposed[3] = binary[33];
            transposed[4] = binary[25];
            transposed[5] = binary[17];
            transposed[6] = binary[9];
            transposed[7] = binary[1];

            transposed[8] = binary[59];
            transposed[9] = binary[51];
            transposed[10] = binary[43];
            transposed[11] = binary[35];
            transposed[12] = binary[27];
            transposed[13] = binary[19];
            transposed[14] = binary[11];
            transposed[15] = binary[3];

            transposed[16] = binary[61];
            transposed[17] = binary[53];
            transposed[18] = binary[45];
            transposed[19] = binary[37];
            transposed[20] = binary[29];
            transposed[21] = binary[21];
            transposed[22] = binary[13];
            transposed[23] = binary[5];

            transposed[24] = binary[63];
            transposed[25] = binary[55];
            transposed[26] = binary[47];
            transposed[27] = binary[39];
            transposed[28] = binary[31];
            transposed[29] = binary[23];
            transposed[30] = binary[15];
            transposed[31] = binary[7];

            transposed[32] = binary[56];
            transposed[33] = binary[48];
            transposed[34] = binary[40];
            transposed[35] = binary[32];
            transposed[36] = binary[24];
            transposed[37] = binary[16];
            transposed[38] = binary[8];
            transposed[39] = binary[0];

            transposed[40] = binary[58];
            transposed[41] = binary[50];
            transposed[42] = binary[42];
            transposed[43] = binary[34];
            transposed[44] = binary[26];
            transposed[45] = binary[18];
            transposed[46] = binary[10];
            transposed[47] = binary[2];

            transposed[48] = binary[60];
            transposed[49] = binary[52];
            transposed[50] = binary[44];
            transposed[51] = binary[36];
            transposed[52] = binary[28];
            transposed[53] = binary[20];
            transposed[54] = binary[12];
            transposed[55] = binary[4];

            transposed[56] = binary[62];
            transposed[57] = binary[54];
            transposed[58] = binary[46];
            transposed[59] = binary[38];
            transposed[60] = binary[30];
            transposed[61] = binary[22];
            transposed[62] = binary[14];
            transposed[63] = binary[6];

            return transposed;
        }
        //最終転置
        static bool[] LastTranspose(bool[] target)
        {
            var transposed = new bool[64];
            transposed[0] = target[39];
            transposed[1] = target[7];
            transposed[2] = target[47];
            transposed[3] = target[15];
            transposed[4] = target[55];
            transposed[5] = target[23];
            transposed[6] = target[63];
            transposed[7] = target[31];

            transposed[8] = target[38];
            transposed[9] = target[6];
            transposed[10] = target[46];
            transposed[11] = target[14];
            transposed[12] = target[54];
            transposed[13] = target[22];
            transposed[14] = target[62];
            transposed[15] = target[30];

            transposed[16] = target[37];
            transposed[17] = target[5];
            transposed[18] = target[45];
            transposed[19] = target[13];
            transposed[20] = target[53];
            transposed[21] = target[21];
            transposed[22] = target[61];
            transposed[23] = target[29];

            transposed[24] = target[36];
            transposed[25] = target[4];
            transposed[26] = target[44];
            transposed[27] = target[12];
            transposed[28] = target[52];
            transposed[29] = target[20];
            transposed[30] = target[60];
            transposed[31] = target[28];

            transposed[32] = target[35];
            transposed[33] = target[3];
            transposed[34] = target[43];
            transposed[35] = target[11];
            transposed[36] = target[51];
            transposed[37] = target[19];
            transposed[38] = target[59];
            transposed[39] = target[27];

            transposed[40] = target[34];
            transposed[41] = target[2];
            transposed[42] = target[42];
            transposed[43] = target[10];
            transposed[44] = target[50];
            transposed[45] = target[18];
            transposed[46] = target[58];
            transposed[47] = target[26];

            transposed[48] = target[33];
            transposed[49] = target[1];
            transposed[50] = target[41];
            transposed[51] = target[9];
            transposed[52] = target[49];
            transposed[53] = target[17];
            transposed[54] = target[57];
            transposed[55] = target[25];

            transposed[56] = target[32];
            transposed[57] = target[0];
            transposed[58] = target[40];
            transposed[59] = target[8];
            transposed[60] = target[48];
            transposed[61] = target[16];
            transposed[62] = target[56];
            transposed[63] = target[24];

            return transposed;
        }
        //拡大転置
        static bool[] Expansion(bool[] target)
        {
            var expansion = new bool[48];

            expansion[0] = target[31];
            expansion[1] = target[0];
            expansion[2] = target[1];
            expansion[3] = target[2];
            expansion[4] = target[3];
            expansion[5] = target[4];
            expansion[6] = target[3];
            expansion[7] = target[4];
            expansion[8] = target[5];
            expansion[9] = target[6];
            expansion[10] = target[7];
            expansion[11] = target[8];
            expansion[12] = target[7];
            expansion[13] = target[8];
            expansion[14] = target[9];
            expansion[15] = target[10];
            expansion[16] = target[11];
            expansion[17] = target[12];
            expansion[18] = target[11];
            expansion[19] = target[12];
            expansion[20] = target[13];
            expansion[21] = target[14];
            expansion[22] = target[15];
            expansion[23] = target[16];
            expansion[24] = target[15];
            expansion[25] = target[16];
            expansion[26] = target[17];
            expansion[27] = target[18];
            expansion[28] = target[19];
            expansion[29] = target[20];
            expansion[30] = target[19];
            expansion[31] = target[20];
            expansion[32] = target[21];
            expansion[33] = target[22];
            expansion[34] = target[23];
            expansion[35] = target[24];
            expansion[36] = target[23];
            expansion[37] = target[24];
            expansion[38] = target[25];
            expansion[39] = target[26];
            expansion[40] = target[27];
            expansion[41] = target[28];
            expansion[42] = target[27];
            expansion[43] = target[28];
            expansion[44] = target[29];
            expansion[45] = target[30];
            expansion[46] = target[31];
            expansion[47] = target[0];

            return expansion;
        }
        
        //S関数
        static bool[] FunctionS(bool[] target,int[,] s)
        {
            var column = -1;
            var row = -1;
            if (!target[0] && !target[5]) column = 0;
            else if (!target[0] && target[5]) column = 1;
            else if (target[0] && !target[5]) column = 2;
            else if (target[0] && target[5]) column = 3;

            var binary_s = "";
            binary_s += target[1] ? "1" : "0";
            binary_s += target[2] ? "1" : "0";
            binary_s += target[3] ? "1" : "0";
            binary_s += target[4] ? "1" : "0";
            row = Convert.ToInt32(binary_s, 2);

            var result_c = Convert.ToString(s[column, row], 2).PadLeft(4, '0').ToCharArray();

            var result = new bool[4];
            result[0] = result_c[0] == '1';
            result[1] = result_c[1] == '1';
            result[2] = result_c[2] == '1';
            result[3] = result_c[3] == '1';

            return result;
        }
        //S関数後転置
        static bool[] Transpose32(bool[] target)
        {
            var result = new bool[32];
            result[0] = target[15];
            result[1] = target[6];
            result[2] = target[19];
            result[3] = target[20];
            result[4] = target[28];
            result[5] = target[11];
            result[6] = target[27];
            result[7] = target[16];
            result[8] = target[0];
            result[9] = target[14];
            result[10] = target[22];
            result[11] = target[25];
            result[12] = target[4];
            result[13] = target[17];
            result[14] = target[30];
            result[15] = target[9];
            result[16] = target[1];
            result[17] = target[7];
            result[18] = target[23];
            result[19] = target[13];
            result[20] = target[31];
            result[21] = target[26];
            result[22] = target[2];
            result[23] = target[8];
            result[24] = target[18];
            result[25] = target[12];
            result[26] = target[29];
            result[27] = target[5];
            result[28] = target[21];
            result[29] = target[10];
            result[30] = target[3];
            result[31] = target[24];
            return result;

        }

        //keyのランダム作成
        static List<bool[]> GenerateRandomKey()
        {
            var key = new bool[64];
            var r = new Random();
            for (int i = 0; i < 64; i++)
            {
                key[i] = r.Next() % 2 == 0;
            }
            BoolArrayWriteLine(key, "key:");
            var contra = KeyContraction56(key);
            var keys = KeyShift(contra);
            return keys;
        }
        //keyから内部キー16個の作成
        static List<bool[]> KeyShift(bool[] key)
        {
            var shift_rule = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
            var key_list = new List<bool[]>();
            var tmp_key = new bool[56];
            key.CopyTo(tmp_key, 0);
            var key_c = new List<bool>(tmp_key.Take(28)).ToArray();
            var key_d = new List<bool>(tmp_key.Skip(28)).ToArray();
            var count = 0;
            foreach (var shift_num in shift_rule)
            {
                key_c = ShiftBoolArray(key_c, shift_num);
                key_d = ShiftBoolArray(key_d, shift_num);
                var shifted_key = new List<bool>(key_c.ToList());
                shifted_key.AddRange(key_d.ToList());
                var contractioned_key = KeyContraction48(shifted_key.ToArray());
                key_list.Add(contractioned_key);
                BoolArrayWriteLine(contractioned_key, "key(sub" + count + "):");
                count++;
            }
            return key_list;
        }
        //縮約転置1
        static bool[] KeyContraction56(bool[] key)
        {
            var contra = new bool[56];
            contra[0] = key[56];
            contra[1] = key[48];
            contra[2] = key[40];
            contra[3] = key[32];
            contra[4] = key[24];
            contra[5] = key[16];
            contra[6] = key[8];
            contra[7] = key[0];
            contra[8] = key[57];
            contra[9] = key[49];
            contra[10] = key[41];
            contra[11] = key[33];
            contra[12] = key[25];
            contra[13] = key[17];
            contra[14] = key[9];
            contra[15] = key[1];
            contra[16] = key[58];
            contra[17] = key[50];
            contra[18] = key[42];
            contra[19] = key[34];
            contra[20] = key[26];
            contra[21] = key[18];
            contra[22] = key[10];
            contra[23] = key[2];
            contra[24] = key[59];
            contra[25] = key[51];
            contra[26] = key[43];
            contra[27] = key[35];
            contra[28] = key[62];
            contra[29] = key[54];
            contra[30] = key[46];
            contra[31] = key[38];
            contra[32] = key[30];
            contra[33] = key[22];
            contra[34] = key[14];
            contra[35] = key[6];
            contra[36] = key[61];
            contra[37] = key[53];
            contra[38] = key[45];
            contra[39] = key[37];
            contra[40] = key[29];
            contra[41] = key[21];
            contra[42] = key[13];
            contra[43] = key[5];
            contra[44] = key[60];
            contra[45] = key[52];
            contra[46] = key[44];
            contra[47] = key[36];
            contra[48] = key[28];
            contra[49] = key[20];
            contra[50] = key[12];
            contra[51] = key[4];
            contra[52] = key[27];
            contra[53] = key[19];
            contra[54] = key[11];
            contra[55] = key[3];

            return contra;
        }
        //縮約転置2
        static bool[] KeyContraction48(bool[] key)
        {
            var contra = new bool[48];

            contra[0] = key[13];
            contra[1] = key[16];
            contra[2] = key[10];
            contra[3] = key[23];
            contra[4] = key[0];
            contra[5] = key[4];
            contra[6] = key[2];
            contra[7] = key[27];
            contra[8] = key[14];
            contra[9] = key[5];
            contra[10] = key[20];
            contra[11] = key[9];
            contra[12] = key[22];
            contra[13] = key[18];
            contra[14] = key[11];
            contra[15] = key[3];
            contra[16] = key[25];
            contra[17] = key[7];
            contra[18] = key[15];
            contra[19] = key[6];
            contra[20] = key[26];
            contra[21] = key[19];
            contra[22] = key[12];
            contra[23] = key[1];
            contra[24] = key[40];
            contra[25] = key[51];
            contra[26] = key[30];
            contra[27] = key[36];
            contra[28] = key[46];
            contra[29] = key[54];
            contra[30] = key[29];
            contra[31] = key[39];
            contra[32] = key[50];
            contra[33] = key[44];
            contra[34] = key[32];
            contra[35] = key[47];
            contra[36] = key[43];
            contra[37] = key[48];
            contra[38] = key[38];
            contra[39] = key[55];
            contra[40] = key[33];
            contra[41] = key[52];
            contra[42] = key[45];
            contra[43] = key[41];
            contra[44] = key[49];
            contra[45] = key[35];
            contra[46] = key[28];
            contra[47] = key[31];

            return contra;
        }


        //bool配列のコンソール出力
        static void BoolArrayWriteLine(bool[] target,string header)
        {
            var binstr = "";
            foreach (var v in target)
            {
                binstr += v ? "1" : "0";
            }
            Console.WriteLine(header + Convert.ToString(Convert.ToInt64(binstr, 2), 16));
        }
        //bool配列の循環シフト
        static bool[] ShiftBoolArray(bool[] target, int num)
        {
            var result = new bool[target.Length];
            for (int i = 0; i < target.Length - num; i++)
            {
                result[i] = target[i + num];
            }
            for (int i = 0; i < num; i++)
            {
                result[target.Length - (num - i)] = target[i];
            }

            return result;
        }
        //bool配列同士のXOR演算
        static bool[] XORBoolArray(bool[] array1, bool[] array2)
        {
            var result = new bool[array1.Length];
            if (array1.Length != array2.Length) throw new Exception();
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = array1[i] ^ array2[i];
            }
            return result;
        }
    }
}
