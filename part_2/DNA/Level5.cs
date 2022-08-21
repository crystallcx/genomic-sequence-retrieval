using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DNA
{
    class Level5
    {
        /*LEVEL 5: Search for an exact match of a DNA query string.
         Displays all matching sequence-ids to the console.*/
        public static void Execute(string DNAseq)
        {
            if (!Regex.IsMatch(DNAseq, @"[A,G,C,T,N]")) // check that the input only contains letter A,G,C,T, and N.
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Enter a valid DNA string that consists of only letters A,G,C,T,N.", DNAseq);
                return;
            }

            var counternum = new List<int>();   // an array to keep counter numbers in (matches)
            string line;                        // line of text is being read
            int counter = 0;                    // initial counter value
            bool fail = true;                   // deafult boolean value

            StreamReader file = new System.IO.StreamReader(Program.path);

            // Reading the file to check for matches
            using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(DNAseq) && !line.Contains(">")) // if line contains (DNAseq) & ">"
                    {
                        fail = false;               // a match is found
                        counternum.Add(counter);    // store counter value
                    }
                    counter++;                      // increment counter
                }
            }
            file.Close();

            // accessing line directly
            foreach (int n in counternum)           // for each number in counternm
            {
                // display the sequence id
                string liner = File.ReadLines(Program.path).Skip(n - 1).Take(1).First();
                Console.WriteLine(liner.Split(' ')[0].TrimStart('>'));
            }

            if (fail) // if not found
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "DNA sub-sequence {0} could not be found.", DNAseq);
            }
        }
    }
}
