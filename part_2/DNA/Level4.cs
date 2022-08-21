using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace DNA
{
    class Level4
    {
        /*LEVEL 4: Indexed file access, implementing direct access to sequences using an index file
         created by the program IndexSequence16s. Uses the indexes to search for query sequence-ids.*/
        public static void Execute(string indexfile, string queryfile, string ofile)
        {
            var result = new List<string>();    // result thats is outputted to the file
            var seqpos = new List<int>();       // sequence positions (bytes)
            var countn = new List<int>();       // counter numbers
            List<int> size = new List<int>();           // an array to keep line size in

            List<int> allpos = new List<int>(); // an array to keep ine size in

            if (!ValidInput(indexfile, queryfile, ofile)) return;

            string[] indexcontent = File.ReadAllLines(indexfile);   // obtain data within index file
            string[] queryseqs = File.ReadAllLines(queryfile);      // obtain data within query file

            foreach (string y in queryseqs) // for each sequence-id in the query file
            {
                bool fail = true;    // default boolean value

                for (int i = 0; i < indexcontent.Length; i++) // for each line within the index file
                {
                    string[] cur_line = indexcontent[i].Split(' '); // split line
                    string cur_seqid = cur_line[0];                 // current sequence id
                    int curr_pos = Convert.ToInt32(cur_line[1]);    // current position
                    allpos.Add(curr_pos);

                    if (y == cur_seqid) // check if match
                    {
                        fail = false;                  // seq -id was found
                        countn.Add(i);                 // store current counter number
                        seqpos.Add(curr_pos);          // store current position
                    }
                }

                for (int i = 1; i < allpos.Count; i++)  // obtain size of each sequence
                {
                    size.Add(allpos[i] - allpos[i - 1]);
                }

                if (fail) // sequence id could not be found
                {
                    LevelCheck.customMsg(LevelCheck.MsgType.error, "The sequence-id {0} could not be found.", y);
                }
            }

            // now use the parallel arrays index to access a line directly 
            for (int n = 0; n < countn.Count; n++) 
            {
                using (FileStream f1 = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[size[countn[n]]];
                    f1.Seek(seqpos[n], SeekOrigin.Begin);
                    f1.Read(bytes, 0, size[countn[n]]); // get the data off disk - now there is disk access
                    result.Add(Encoding.Default.GetString(bytes)); // display the line
                }
            }

            // Writing out output to a file.
            string s = String.Join(Environment.NewLine, result);
            Part1.ToFile(s, ofile);
        }

        /* Checks if the input is valid */
        static bool ValidInput(string indexfile, string queryfile, string ofile)
        {
            // Checks the output file extension
            if (!Regex.IsMatch(ofile, @"^.*\.(txt|fasta)$")) 
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Enter a valid output file extension...", queryfile);
                return false;
            }

            // check if query file and index file exist
            if (!Program.Validfile(indexfile) || !Program.Validfile(queryfile)) return false; 


            // check that the input files are not empty
            if (new FileInfo(indexfile).Length == 0 || new FileInfo(queryfile).Length == 0)
            {
                if (new FileInfo(queryfile).Length == 0)
                {
                    LevelCheck.customMsg(LevelCheck.MsgType.error, "Query file {0} is empty...", queryfile);
                }
                if (new FileInfo(indexfile).Length == 0)
                {
                    LevelCheck.customMsg(LevelCheck.MsgType.error, "Index file {0} is empty...", indexfile);
                }
                return false;
            }
            return true;
        }
    }
}



// ignore below...
/*            // store sequence-id
            foreach (int j in countn)       // for each stored counter number (sequence)
            {
                int linenum = j * 2 + 1;    // convert to line number
                for (int n = linenum; n < linenum + 1 * 2; n++) //to store the full sequence
                {
                    string line = File.ReadLines(Program.path).Skip(n - 1).Take(1).First(); //Skip to the line number 
                    result.Add(line);                   //store the line
                    result.Add(Environment.NewLine);    //add a new line
                }
            }*/
