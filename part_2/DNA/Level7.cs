using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DNA
{
    class Level7
    {
        /*LEVEL 7: Search for a sequence containing wild cards.*/
        public static void Execute(string manyseq)
        {
            bool fail; // boolean to check if a match has been found

            string[] seqs = manyseq.Split('*');
            string newinput = manyseq.Replace("*", ".*");   // replace * with .* (to create regex pattern)
            var counternum = new List<int>();               // contains line numbers of sequences which contain the input sequence

            string pattern = (newinput);                    // regex pattern for matching

            string line;    // line of text being read
            int counter = 0;    // line counter in text file
            fail = true;    //boolean to check if a match has been found

            // Reading the file to check for matches
            StreamReader file = new StreamReader(Program.path);
            using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if ((seqs.All(s => line.Contains(s)))) //if line contains all the seqs
                    {
                        if (Regex.IsMatch(line, pattern)) //if regular expression finds a match in the specified input string
                        {
                            fail = false;            // matching sequence is found
                            counternum.Add(counter); // storing current counter in counternum
                        }
                    }
                    counter++; // increment counter
                }
            }
            file.Close();

            // accessing line directly
            foreach (int n in counternum) //for each number in counternm
            {
                // display the sequence id
                string liner = File.ReadLines(Program.path).Skip(n - 1).Take(1).First();
                Console.WriteLine(liner.Split(' ')[0].TrimStart('>'));
            }

            if (fail) // display error msg if sequence could not be found
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "DNA sub-sequences {0} could not be found.", manyseq);
            }
        }

    }
}
