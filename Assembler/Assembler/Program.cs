using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                return;
            }

            System.IO.StreamReader firstPass =new System.IO.StreamReader(args[1]);

            string line;
            int bitsRead = 0;
            Dictionary<string, int[]> labelMap = new Dictionary<string, int[]>();

            while ((line = firstPass.ReadLine()) != null)
            {
                if (!line[0].Equals('\t')) {
                    labelMap.Add(line.Remove(':'), Decimal.GetBits(bitsRead));
                }
                else
                {
                    string inst = line.Split(new char[] { ' ', '\t', ',' })[0];
                    switch (inst)
                    {
                        case "add":
                        case "and":
                        case "or":
                        case "cmp":
                        case "sub":
                        case "mov":
                        case "rafr":
                        case "sl":
                        case "srl":
                        case "sra":
                        case "jr":
                            bitsRead += 16;
                    break;
                        case "addi":
                        case "subi":
                        case "andi":
                        case "ori":
                        case "beq":
                        case "bne":
                        case "bgt":
                        case "blt":
                        case "rafi":
                        case "ww":
                        case "wi":
                        case "movi":
                        case "j":
                        case "jal":
                            bitsRead += 32;
                            break;
                        default:
                            Console.WriteLine("Instruction " + inst + " not recognized.");
                            return;
                    }
                }
            }
            firstPass.Close();

            System.IO.StreamReader secondPass = new System.IO.StreamReader(args[1]);

            //output file will be supplied by command line arg 2
            System.IO.StreamWriter sr = new System.IO.StreamWriter(args[2]);

            while ((line = secondPass.ReadLine()) != null)
            {
                if (!line[0].Equals('\t'))
                {
                    continue;
                }

                else
                {
                    string []inst = line.Split(new char[] { ' ', '\t', ',' });
                    switch (inst[0])
                    {
                        case "add":
                            writeInstruction(sr, '0', (int)instructions.add);
                            break;
                        case "and":
                            writeInstruction(sr, '0', (int)instructions.and);
                            break;
                        case "or":
                            writeInstruction(sr, '0', (int)instructions.or);
                            break;
                        case "cmp":
                            writeInstruction(sr, '0', (int)instructions.cmp);
                            break;
                        case "sub":
                            writeInstruction(sr, '0', (int)instructions.sub);
                            break;
                        case "mov":
                            writeInstruction(sr, '0', (int)instructions.mov);
                            break;
                        case "rafr":
                            writeInstruction(sr, '0', (int)instructions.rafr);
                            break;
                        case "sl":
                            writeInstruction(sr, '0', (int)instructions.sl);
                            break;
                        case "srl":
                            writeInstruction(sr, '0', (int)instructions.srl);
                            break;
                        case "sra":
                            writeInstruction(sr, '0', (int)instructions.sra);
                            break;
                        case "jr":
                            writeInstruction(sr, '0', (int)instructions.jr);
                            break;

                        case "addi":
                            writeInstruction(sr, '1', (int)instructions.addi);
                            break;
                        case "subi":
                            writeInstruction(sr, '1', (int)instructions.subi);
                            break;
                        case "andi":
                            writeInstruction(sr, '1', (int)instructions.andi);
                            break;
                        case "ori":
                            writeInstruction(sr, '1', (int)instructions.ori);
                            break;
                        case "beq":
                            writeInstruction(sr, '1', (int)instructions.beq);
                            break;
                        case "bne":
                            writeInstruction(sr, '1', (int)instructions.bne);
                            break;
                        case "bgt":
                            writeInstruction(sr, '1', (int)instructions.bgt);
                            break;
                        case "blt":
                            writeInstruction(sr, '1', (int)instructions.blt);
                            break;
                        case "rafi":
                            writeInstruction(sr, '1', (int)instructions.rafi);
                            break;
                        case "ww":
                            writeInstruction(sr, '1', (int)instructions.ww);
                            break;
                        case "wi":
                            writeInstruction(sr, '1', (int)instructions.wi);
                            break;
                        case "movi":
                            writeInstruction(sr, '1', (int)instructions.movi);
                            break;
                        case "j":
                            writeInstruction(sr, '1', (int)instructions.j);
                            break;
                        case "jal":
                            writeInstruction(sr, '1', (int)instructions.jal);
                            break;
                        default:
                            Console.WriteLine("Instruction " + inst + " not recognized.");
                            return;
                    }
                }


                sr.Write('\n');
            }


            }

        private static void writeInstruction(StreamWriter sr, char v, int inst)
        {
            sr.Write(v);
            int []bitArray;
            bitArray = Decimal.GetBits(inst);
            for (int i = 3; i < 8; i++)
            {
                sr.Write(bitArray[i]);
            }
        }

        enum instructions{
            and=0,
            or=1,
            andi=2,
            ori=3,
            add=4,
            addi=5,
            sub=6,
            subi=7,
            sl=8,
            srl=9,
            sra=10,
            cmp=11,
            mov=12,
            beq=13,
            bne=14,
            bgt=15,
            blt=16,
            rafi=17,
            ww=18,
            wi=19,
            movi=20,
            j=21,
            jal=22,
            jr=23,
            rafr = 24
        };

        enum registers
        {
            zero=0,
            sp=1,
            fp=2,
            ret0=3,
            ret1=4,
            ra=5,
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
