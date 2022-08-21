using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DNA
{
    class Level6
    {
        /*LEVEL 6: Search for a sequence meta-data containing a given word.
         Matching sequence i-ds are displayed to the console.*/
        public static void Execute(string word)
        {
            string line; // line of text is being read
            bool fail = true; // initial bool value

            // Reading the file to check for matches
            StreamReader file = new StreamReader(Program.path);
            using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
            {
                while ((line = file.ReadLine()) != null)
                {
                    // Check if line (excluding sequence-id) contains (word) and ">"
                    if (line.Substring(13, line.Length - 13).Contains(word) && line.Contains(">") ) 
                    {
                        fail = false;   // a match is found
                        string[] linestr = line.Split(' ');
                        Console.WriteLine(linestr[0].TrimStart('>')); // display sequence-id
                    }
                }
            }

            file.Close();
            if (fail) // display error msg if word could not be found
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Sequences that contain the word {0} could not be found.", word);
            }
        }

    }
}
