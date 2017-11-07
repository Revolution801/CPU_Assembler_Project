using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


// Changes rafi to RW and we need to change the encoding to accept another register as 
// its encoding for 32 bit instructions(zero'd out if there is no need for another register)
namespace Assembler
{
    class Program
    {
        static Dictionary<string, string> labelMap = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            string line;
            int bitsRead = 0;

            if (args.Length != 2)
            {
                Console.WriteLine("Not enouh args." + args.Length + args[0] + args[1]);
                Console.Read();
                return;
            }

            Console.WriteLine("Begin encoding.");
            try
            {
                using (System.IO.StreamReader firstPass = new System.IO.StreamReader(args[0]))
                {
                    while ((line = firstPass.ReadLine()) != null)
                    {
                        if (!line[0].Equals('\t'))
                        {
                            line = line.Substring(0, line.Length - 1);
                            StringBuilder bits = new StringBuilder();
                             bits.Append(Convert.ToString(bitsRead, 2));
                            labelMap.Add(line, bits.Insert(0, "0", 16 - bits.Length).ToString());
                        }
                        else
                        {
                            List<string> tokens = new List<string>(line.Split(new char[] { ' ', '\t', ',' }).Where(s => (!string.IsNullOrWhiteSpace(s))));
                            string inst = tokens[0];
                            switch (inst)
                            {
                                // ONE WORD INSTRUCTIONS
                                case "add":
                                case "and":
                                case "or":
                                case "cmp":
                                case "sub":
                                case "mov":
                                case "sl":
                                case "srl":
                                case "sra":
                                case "jr":
                                    bitsRead += 16;
                                    break;
                                // TWO WORD INSTRUCTIONS
                                case "addi":
                                case "subi":
                                case "andi":
                                case "ori":
                                case "beq":
                                case "bne":
                                case "bgt":
                                case "blt":
                                case "rw":
                                case "ww":
                                case "movi":
                                case "j":
                                case "jal":
                                    bitsRead += 32;
                                    break;

                                default:
                                    Console.WriteLine("Instruction " + inst + " not recognized.");
                                    Console.Read();
                                    return;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File not found." + e.Message);
                throw new FileNotFoundException("File Not Found");
            }

            try
            {
                using (System.IO.StreamReader secondPass = new System.IO.StreamReader(args[0]))
                {
                    //output file will be supplied by command line arg 2
                    using (System.IO.StreamWriter sr = new System.IO.StreamWriter(args[1]))
                    {
                        sr.WriteLine("memory_initialization_radix=2;");
                        sr.WriteLine("memory_initialization_vector=");
                        sr.WriteLine("");
                        while ((line = secondPass.ReadLine()) != null)
                        {
                            if (!line[0].Equals('\t'))
                            {
                                continue;
                            }

                            else
                            {
                                List<string> tokens = new List<string>(line.Split(new char[] { ' ', '\t', ',' }).Where(s => (!string.IsNullOrWhiteSpace(s))));

                                switch (tokens[0])
                                {
                                    // 16 bit instructions
                                    case "add":
                                        writeInstruction(sr, '0', (int)instructions.add);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    case "and":
                                        writeInstruction(sr, '0', (int)instructions.and);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    case "or":
                                        writeInstruction(sr, '0', (int)instructions.or);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    case "cmp":
                                        writeInstruction(sr, '0', (int)instructions.cmp);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    case "sub":
                                        writeInstruction(sr, '0', (int)instructions.sub);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    case "mov":
                                        writeInstruction(sr, '0', (int)instructions.mov);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]));
                                        sr.Write(",");
                                        break;
                                    // Second Register will have 5 bit immediate instead of register index
                                    case "sl":
                                        writeInstruction(sr, '0', (int)instructions.sl);
                                        string temp = regDecoding(tokens[2]);
                                        sr.Write(temp.Substring(temp.Length - 6, temp.Length - 1));
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(",");
                                        break;
                                    case "srl":
                                        writeInstruction(sr, '0', (int)instructions.srl);
                                        temp = regDecoding(tokens[2]);
                                        sr.Write(temp.Substring(temp.Length - 6, temp.Length - 1));
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(",");
                                        break;
                                    case "sra":
                                        writeInstruction(sr, '0', (int)instructions.sra);
                                        temp = regDecoding(tokens[2]);
                                        sr.Write(temp.Substring(temp.Length - 6, temp.Length - 1));
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(",");
                                        break;
                                    case "jr":
                                        writeInstruction(sr, '0', (int)instructions.jr);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write("00000");
                                        sr.Write(",");
                                        break;

                                    // 32 bit instructions
                                    case "addi":
                                        writeInstruction(sr, '1', (int)instructions.addi);
                                        sr.Write("00000,");
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        break;
                                    case "subi":
                                        writeInstruction(sr, '1', (int)instructions.subi);
                                        sr.Write("00000,");
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        break;
                                    case "andi":
                                        writeInstruction(sr, '1', (int)instructions.andi);
                                        sr.Write("00000,");
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        break;
                                    case "ori":
                                        writeInstruction(sr, '1', (int)instructions.ori);
                                        sr.Write("00000,");
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        break;
                                    case "beq":
                                        writeInstruction(sr, '1', (int)instructions.beq);
                                        sr.Write("0000000000,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    case "bne":
                                        writeInstruction(sr, '1', (int)instructions.bne);
                                        sr.Write("0000000000,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    case "bgt":
                                        writeInstruction(sr, '1', (int)instructions.bgt);
                                        sr.Write("0000000000,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    case "blt":
                                        writeInstruction(sr, '1', (int)instructions.blt);
                                        sr.Write("0000000000,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    // Get other register instead of 0'ing out 5 bits
                                    case "rw":
                                        writeInstruction(sr, '1', (int)instructions.rw);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[3]) + ",");
                                        break;
                                    case "ww":
                                        writeInstruction(sr, '1', (int)instructions.ww);
                                        sr.Write(regDecoding(tokens[1]));
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[3]) + ",");
                                        break;
                                    case "movi":
                                        writeInstruction(sr, '1', (int)instructions.movi);
                                        sr.Write("00000");
                                        sr.Write(regDecoding(tokens[1])+",");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[2]) + ",");
                                        break;
                                    case "j":
                                        writeInstruction(sr, '1', (int)instructions.j);
                                        sr.Write("0000000000,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    case "jal":
                                        writeInstruction(sr, '1', (int)instructions.jal);
                                        sr.Write("0000000101,");
                                        sr.WriteLine();
                                        sr.Write(regDecoding(tokens[1]) + ",");
                                        break;
                                    default:
                                        Console.WriteLine("Instruction " + tokens + " not recognized.");
                                        Console.Read();
                                        return;
                                }
                            }
                            sr.WriteLine();
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading or writing on second pass");
                throw new Exception("Error reading or writing on second pass" + e.Message);
            }
            Console.WriteLine("Encoding complete.");
            Console.Read();
        }

        private static void writeInstruction(StreamWriter sr, char v, int inst)
        {
            sr.Write(v);
            string bitArray = Convert.ToString(inst, 2);
            int iters = bitArray.Length;
            for (int i = iters; i < 5; i++)
            {
                bitArray = "0" + bitArray;
            }
            sr.Write(bitArray);
        }

        private static string regDecoding(string curr)
        {
            if (curr[0] == '!')
            {
                switch (curr.TrimStart('!'))
                {
                    case "zero":
                        return regNameToBits(registers.zero);
                    case "sp":
                        return regNameToBits(registers.sp);
                    case "fp":
                        return regNameToBits(registers.fp);
                    case "ret0":
                        return regNameToBits(registers.ret0);
                    case "ret1":
                        return regNameToBits(registers.ret1);
                    case "ra":
                        return regNameToBits(registers.ra);
                    case "val0":
                        return regNameToBits(registers.val0);
                    case "val1":
                        return regNameToBits(registers.val1);
                    case "val2":                       
                        return regNameToBits(registers.val2);
                    case "val3":                       
                        return regNameToBits(registers.val3);
                    case "val4":                       
                        return regNameToBits(registers.val4);
                    case "val5":                       
                        return regNameToBits(registers.val5);
                    case "val6":                       
                        return regNameToBits(registers.val6);
                    case "val7":                       
                        return regNameToBits(registers.val7);
                    case "val8":                       
                        return regNameToBits(registers.val8);
                    case "val9":                       
                        return regNameToBits(registers.val9);
                    case "val10":                      
                        return regNameToBits(registers.val10);
                    case "val11":                      
                        return regNameToBits(registers.val11);
                    case "val12":                      
                        return regNameToBits(registers.val12);
                    case "temp0":
                        return regNameToBits(registers.temp0);
                    case "temp1":                     
                        return regNameToBits(registers.temp1);
                    case "temp2":                      
                        return regNameToBits(registers.temp2);
                    case "temp3":                      
                        return regNameToBits(registers.temp3);
                    case "temp4":                      
                        return regNameToBits(registers.temp4);
                    case "temp5":                      
                        return regNameToBits(registers.temp5);
                    case "temp6":                      
                        return regNameToBits(registers.temp6);
                    case "temp7":                      
                        return regNameToBits(registers.temp7);
                    case "temp8":                      
                        return regNameToBits(registers.temp8);
                    case "temp9":                      
                        return regNameToBits(registers.temp9);
                    case "temp10":                     
                        return regNameToBits(registers.temp10);
                    case "temp11":                     
                        return regNameToBits(registers.temp11);
                    case "temp12":                     
                        return regNameToBits(registers.temp12);
                    default:
                        Console.WriteLine("Register not recognized");
                        throw new ArgumentException("Register unrecognized");
                }
            }
            else if (Regex.IsMatch(curr, "^0x"))
            {
                StringBuilder binaryString = new StringBuilder();
                // Convert each hex digit into a 4 bit decimal and concatenate them into a string


                binaryString.Append(String.Join(String.Empty, curr.Substring(2).Select(c =>
                {
                if (c == '0')
                {
                    return "0000";
                }
                else
                {
                        StringBuilder intermediate = new StringBuilder();
                        intermediate.Append(Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2));
                        return intermediate.Insert(0, "0", 4 - intermediate.Length).ToString();
                    }
                })));
                
                // Pad beginning with 0's to make 16 bit binary string
                binaryString.Insert(0, "0", 16 - binaryString.Length);
                return binaryString.ToString();
            }
            else
            {
                if (labelMap.ContainsKey(curr))
                {
                    return labelMap[curr];
                }
                else
                {
                    throw new ArgumentException("Label was not recognized by the assembler.");
                }
            }
        }

        private static string regNameToBits(registers reg)
        {
            string bits = Convert.ToString((int)reg, 2);
            int iters = bits.Length;
            for (int i = iters; i < 5; i++)
            {
                bits = "0" + bits;
            }
            return bits;
        }

        enum instructions
        {
            and = 0,
            or = 1,
            andi = 2,
            ori = 3,
            add = 4,
            addi = 5,
            sub = 6,
            subi = 7,
            sl = 8,
            srl = 9,
            sra = 10,
            cmp = 11,
            mov = 12,
            beq = 13,
            bne = 14,
            bgt = 15,
            blt = 16,
            rw = 17,
            ww = 18,
            movi = 20,
            j = 21,
            jal = 22,
            jr = 23
        };

        enum registers
        {
            zero = 0,
            sp = 1,
            fp = 2,
            ret0 = 3,
            ret1 = 4,
            ra = 5,
            val0 = 6,
            val1 = 7,
            val2 = 8,
            val3 = 9,
            val4 = 10,
            val5 = 11,
            val6 = 12,
            val7 = 13,
            val8 = 14,
            val9 = 15,
            val10 = 16,
            val11 = 17,
            val12 = 18,
            temp0 = 19,
            temp1 = 20,
            temp2 = 21,
            temp3 = 22,
            temp4 = 23,
            temp5 = 24,
            temp6 = 25,
            temp7 = 26,
            temp8 = 27,
            temp9 = 28,
            temp10 = 29,
            temp11 = 30,
            temp12 = 31
        };
    }
}


