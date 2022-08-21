using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNA
{
    class LevelCheck
    {
        private const int LVL1_1 = 5; //Length of level 1 input (not outputting to file)
        private const int LVL1_2 = 7; //Length of level 1 input (output to file)

        /* Checks which level should be excecuted, and performs some error checking.
         * If passed, the corresponding level is executed.*/
        public static void Levelcheck(string[] input)
        {
            string level = input[1];
            int startnum, seqs;
            if (level == "-level1")
            {
                if (input.Length == LVL1_1)
                {
                    if (int.TryParse(input[3], out startnum) && int.TryParse(input[4], out seqs))
                    {
                        Part1.Level1(startnum, seqs);
                    }
                    else
                    {
                        customMsg(MsgType.error, "Invalid number of inputs for -level1.");
                        return;
                    }
                }

                else if (input.Length == LVL1_2 && input[5] == ">")
                {
                    if (int.TryParse(input[3], out startnum) && int.TryParse(input[4], out seqs))
                    {
                        string outfilename = input[6];
                        Part1.Level1(startnum, seqs, outfilename);
                    }
                    else
                    {
                        customMsg(MsgType.error, "Invalid inputs for -level1.");
                        return;
                    }
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 1. At least 5 inputs expected."); return; }
            }
            else if (level == "-level2")
            {
                if (input.Length == 4)
                {
                    Part1.Level2(input[3]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 2. 4 inputs expected."); return; }

            }
            else if (level == "-level3")
            {
                if (input.Length == 5)
                {
                    Part1.Level3(input[3], input[4]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 3. 5 inputs expected."); return; }
            }
            else if (level == "-level4")
            {
                if (input.Length == 6)
                {
                    Level4.Execute(input[3],input[4],input[5]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 4. 6 inputs expected."); return; }
            }
            else if (level == "-level5")
            {
                if (input.Length == 4)
                {
                    Level5.Execute(input[3]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 5. 4 inputs expected."); return; }
            }
            else if (level == "-level6")
            {
                if (input.Length == 4)
                {
                    Level6.Execute(input[3]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 6. 4 inputs expected."); return; }
            }
            else if (level == "-level7")
            {
                if (input.Length == 4)
                {
                    Level7.Execute(input[3]);
                }
                else { customMsg(MsgType.error, "Invalid number of inputs for level 7. 4 inputs expected."); return; }
            }
        }

        /*Customised message. Prints out either green or red text to the console depending on the message type specified.*/
        public enum MsgType { success, error }
        public static void customMsg(MsgType type, string msg, string variable = null)
        {
            if (type == MsgType.success) // green text
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (variable != null) Console.WriteLine(msg, variable);
                else Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (type == MsgType.error) // red text
            {
                Console.ForegroundColor = ConsoleColor.Red;
                if (variable != null) Console.WriteLine(msg, variable);
                else Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

    }
}
