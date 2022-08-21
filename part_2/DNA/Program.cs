using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DNA
{
    class Program
    {
        public static string path { get; set; }
        public static string level, program, file;

        /*Main program excecution*/
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Excecute(args);
        }

        /* Executes based on the program name specified */
        public void Excecute(string[] input)
        {
            try
            {
                program = input[0]; //program name
            }
            catch (System.IndexOutOfRangeException)
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Please enter a program name.");
                return;
            }

            if (input[0] == "Search16s")
            {
                try
                {
                    level = input[1]; //fasta file name.
                    file = input[2]; //level name.
                    string apppath = Environment.CurrentDirectory;
                    path = Path.Combine(apppath, file);
                    if (!Validfile(path) || (!Validlevel())) return;
                }
                catch (System.IndexOutOfRangeException)
                {
                    LevelCheck.customMsg(LevelCheck.MsgType.error, "Not enough arguments. At least 4 inputs expected.");
                    return;
                }
                LevelCheck.Levelcheck(input);
                return;
            }
            else if (input[0] == "IndexSequence16s")
            {
                IndexSeq.Program i = new IndexSeq.Program();
                i.Excecute(input);
            }
            else
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Enter a valid program name.");
                return;
            }
            return;
        }

        /* Reads the specified input file*/
        public static void ReadFile(string filepath)
        {
            string line; // line of text i will read
            int position = 0; // file position of first line
            Part1.counter = 0;

            // Read the file 
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            while ((line = file.ReadLine()) != null)
            {
                Part1.pos.Insert(Part1.counter, position); // store line position
                Part1.size.Insert(Part1.counter, line.Length + 1); // store line size
                Part1.counter++;
                position = position + line.Length + 1; // add 1 for '\n' character in file
            }
            file.Close();
        }

        /* Validates the level input, checks if it exists.
         * Returns an error message if it does not exist. */ 
        public bool Validlevel()
        {
            string[] x = { "-level1", "-level2", "-level3", "-level4", "-level5", "-level6", "-level7" };
            if (Array.Exists(x, c => c == level)) return true;
            else { LevelCheck.customMsg(LevelCheck.MsgType.error, "Please enter a valid level."); return false; }
        }

        /* Validates the specified file input, checks if it exists.
         * Returns an error message if it does not exist. */
        public static bool Validfile(string filepath)
        {
            string fname = Path.GetFileName(filepath);
            if (File.Exists(filepath))
            {
                ReadFile(filepath);
                return true;
            }
            else
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "File {0} could not be found.", fname);
                return false;
            }
        }
    }
}
 