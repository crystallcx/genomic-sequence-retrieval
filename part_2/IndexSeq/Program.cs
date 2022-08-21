using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Text.RegularExpressions;

namespace IndexSeq
{
    public class Program
    {
        /*Two file names are specified, the fasta file name and the index file name(these are
        run-time variables, not fixed hard-code file names). The program creates a sequenceid index to the fasta file.
        The index supports direct access to sequences, by sequenceid.Specifically, the program creates an index file.
        Each line consists of a sequenceid and file - offset.*/

        public static string program, infile, ofile, inpath, opath;
        static void Main(string[] args)
        {
        }

        public void Excecute(string[] input)
        {
            try
            {
                infile = input[1];                      //input fasta file name.
                ofile = input[2];                       //output file name.
                string apppath = Environment.CurrentDirectory;
                inpath = Path.Combine(apppath, infile); 

                if (!File.Exists(inpath)) // check if file exists
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Enter a valid file to index.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

            }
            catch (System.IndexOutOfRangeException)  // catch errors, not enough arguments.
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not enough arguments. 3 inputs expected.");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Index(infile, ofile);                   // Indexing the file
        }

        /* Indexes the input fasta file */
        public static void Index(string iinfile, string outfile)
        {
            const int TOSEQ = 2;                //converts from sequences to lines.
            var result = new List<string>();    //list to store output in

            int counter = 0;                    // line counter in text file
            string line;                        // line of text
            string seqid;                       //seq id
            int position = 0;                   // file position of first line
            List<int> pos = new List<int>();    // an array to keep ine positions in
            List<int> size = new List<int>();   // an array to keep ine size in
            System.IO.StreamReader file = new System.IO.StreamReader(inpath);

            // Read the file and display it line by line.  
            while ((line = file.ReadLine()) != null)
            {
                pos.Insert(counter, position);         // store line position
                size.Insert(counter, line.Length + 1); // store line size
                counter++;
                position = position + line.Length + 1; // add 1 for '\n' character in file
            }
            file.Close();

            // now use the parallel arrays index to access a line directly 
            using (FileStream fs = new FileStream(iinfile, FileMode.Open, FileAccess.Read))
            {
                // the "using" construct ensures that the FileStream is properly closed/disposed   
                for (int n = 0; n < counter; n = n + 1*TOSEQ)
                {
                    byte[] bytes = new byte[size[n]];
                    fs.Seek(pos[n], SeekOrigin.Begin);  // seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[n]);         // get the data off disk - now there is disk access
                    seqid = Encoding.Default.GetString(bytes).Substring(1,12); // sequence id
                    result.Add(seqid  + pos[n]);        //store sequence-id and position
                }
            }
            file.Close();

            string s = string.Join(Environment.NewLine, result); // output string

            // Writing out the output to a file.
            FileStream outFile = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            writer.Write(s);
            writer.Close();
            outFile.Close();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nOutput to {0} succeeded.", ofile);
            Console.ForegroundColor = ConsoleColor.White;

        }
    }

}


