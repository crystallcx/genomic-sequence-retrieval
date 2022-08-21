using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace DNA
{
    class Part1
    {
        const int TOSEQ = 2; //converts from sequences to lines.
        public static List<int> size = new List<int>(); // an array to keep line positions in
        public static List<int> pos = new List<int>();  // an array to keep line size in
        public static int counter { get; set; } // line counter
        public static bool fail; // boolean to check if a match has been found
        
        /* Writes the input string (outputtext) to a file within the current directory */
        public static void ToFile(string outputext, string ofile)
        {
            // Writing out output to a file.
            FileStream outFile = new FileStream(ofile, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            writer.Write(outputext);
            writer.Close();
            outFile.Close();
            LevelCheck.customMsg(LevelCheck.MsgType.success, "Output to {0} succeeded.", ofile);
        }

        /* LEVEL 3: Sequential access to find a set of sequence-ids given in a query file, and
        writing the output to a specified result file.*/
        public static void Level3(string queryfile, string ofile)
        {
            var result = new List<string>();
            string apppath = Environment.CurrentDirectory; //obtains current directory of .exe file
            string querypath = Path.Combine(apppath, queryfile);

            string[] seqs = File.ReadAllLines(querypath);
            if (seqs == null) { return; }

            foreach (string x in seqs)
            {
                string line; // line of text is being read
                counter = 0;
                System.IO.StreamReader file = new System.IO.StreamReader(Program.path);
                bool fail = true;
                using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] linestr = line.Split(' ');

                        if (linestr.Contains(">" + x))
                        {
                            fail = false;
                            for (int n = counter; n < counter + (1 * TOSEQ); n++)
                            {
                                byte[] bytes = new byte[size[n]];
                                fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                                fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access

                                if (Encoding.Default.GetString(bytes).Contains(">"))
                                {
                                    string[] k = Encoding.Default.GetString(bytes).Split('>');
                                    k = k.Skip(1).ToArray();

                                    for (int i = 0; i < k.Length; i++)
                                    {
                                        result.Add(">" + k[i]);
                                    }
                                }
                                else
                                {
                                    result.Add(Encoding.Default.GetString(bytes)); // display the line
                                }
                                result.Add(Environment.NewLine);
                            }
                        }
                        counter++;
                    }
                }
                file.Close();
                if (fail)
                {
                    LevelCheck.customMsg(LevelCheck.MsgType.error, "Sequence ID {0} could not be found.", x);
                }
            }
            // Writing out output to a file.
            string s = String.Join("", result);
            ToFile(s,ofile);
        }


        /* Excecutes level 2*/
        public static void Level2(string sequenceid)
        {
            fail = true;
            string line; // line of text being read
            counter = 0;
            StreamReader file = new StreamReader(Program.path);
            using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] linestr = line.Split(' ');

                    if (linestr.Contains(">" + sequenceid))
                    {
                        for (int n = counter; n < counter + (1 * TOSEQ); n++)
                        {
                            byte[] bytes = new byte[size[n]];
                            fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet
                            fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                            if (Encoding.Default.GetString(bytes).Contains(">"))
                            {
                                fail = false;
                                string[] k = Encoding.Default.GetString(bytes).Split('>');
                                k = k.Skip(1).ToArray();
                                for (int i = 0; i < k.Length; i++)
                                {
                                    Console.Write(">" + k[i]);
                                    if (i < k.Length - 1) Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.Write(Encoding.Default.GetString(bytes)); // display the line
                            }
                        }
                    }
                    counter++;
                }
            }
            file.Close();

            if (fail)
            {
                LevelCheck.customMsg(LevelCheck.MsgType.error, "Sequence ID {0} could not be found.", sequenceid);
                return;
            }
        }

        /* Excecutes level 1*/
        public static void Level1(int start, int seqs, string filename = null)
        {
            int startline = start - 1;
            if (startline < 0 || seqs < 0) { LevelCheck.customMsg(LevelCheck.MsgType.error, "Please enter positive and whole numbers only."); return; }
            else if ((start - 1) >= counter) { LevelCheck.customMsg(LevelCheck.MsgType.error, "Line number exceeds end of file."); return; }
            else if ((start) % 2 == 0) { LevelCheck.customMsg(LevelCheck.MsgType.error, "Please enter an odd number for the starting line position."); return; }
            else if ((start - 1) + (seqs * TOSEQ) > counter) { LevelCheck.customMsg(LevelCheck.MsgType.error, "Number of sequences from starting point exceeds end of file."); return; }

            if(filename == null) 
            {
                // now use the parallel arrays index to access a line directly 
                using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
                {
                    // the "using" construct ensures that the FileStream is properly closed/disposed   
                    for (int n = startline; n < startline + seqs * TOSEQ; n++)
                    {
                        byte[] bytes = new byte[size[n]];
                        fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet
                        fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                        if (Encoding.Default.GetString(bytes).Contains(">"))
                        { 
                            string[] k = Encoding.Default.GetString(bytes).Split('>');
                            k = k.Skip(1).ToArray();
                            for (int i = 0; i < k.Length; i++)
                            {
                                Console.Write(">" + k[i]);
                                if (i < k.Length - 1) Console.WriteLine();
                            }
                        }
                        else Console.WriteLine(Encoding.Default.GetString(bytes)); // display the line
                    }
                }
            }

            if (filename != null)
            {
                var result = new List<string>();
                using (FileStream fs = new FileStream(Program.path, FileMode.Open, FileAccess.Read))
                {
                    for (int n = startline; n < (startline + seqs * TOSEQ); n++)
                    {
                        byte[] bytes = new byte[size[n]];
                        fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                        fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access
                        if (Encoding.Default.GetString(bytes).Contains(">"))
                        {
                            string[] k = Encoding.Default.GetString(bytes).Split('>');
                            k = k.Skip(1).ToArray();
                            for (int i = 0; i < k.Length; i++)
                            {
                                result.Add(">" + k[i]);
                                if (i < k.Length - 1) result.Add(Environment.NewLine);
                            }
                        }
                        else result.Add(Encoding.Default.GetString(bytes)); // display the line
                        result.Add(Environment.NewLine);
                        if (n % 2 == 1) result.Add(Environment.NewLine);
                    }

                }
                // Writing out output to a file.
                FileStream outFile = new FileStream(filename, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(outFile);
                string r = String.Join("", result);
                writer.Write(r);
                writer.Close();
                outFile.Close();
                LevelCheck.customMsg(LevelCheck.MsgType.success, "Output to {0} succeeded.", filename);

            }
        }
    }
}
