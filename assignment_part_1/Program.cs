using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

class Program
{
    public static List<int> size = new List<int>();
    public static List<int> pos = new List<int>();
    private string level, program, file, outfilename;

    const int TOSEQ = 2; //converts from sequences to lines.
    private const int LVL1_1 = 5; //Length of level 1 input (not outputting to file)
    private const int LVL1_2 = 7; //Length of level 1 input (output to file)

    public static int counter { get; set; }
    public static string path { get; set; }
    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n\t\tWelcome to Geometric Sequence Retrieval!!!\n");
        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine("Example calls:");
        Console.WriteLine("Lvl 1:  Search using a starting position: Search16s -level1 16S.fasta 73 1");
        Console.WriteLine("\tOutput to a file: Search16s -level1 16S.fasta 273 N > myfile.fasta");

        Console.WriteLine("Lvl 2:  Find a sequence by seq-id: Search16s -level2 16S.fasta NR_115365.1");
        Console.WriteLine("Lvl 3:  Find seq-ids given in query file, output to file: Search16s -level3 16S.fasta query.txt results.txt ");

        Program p = new Program();
        while (true)
        {
            p.Excecute();
        }
    }

    /* Checks which level should be excecuted*/
    public void levelcheck(string[] input)
    {
        int startnum, seqs;
        if (this.level == "-level1")
        {
            if (input.Length == LVL1_1)
            {
                if (int.TryParse(input[3], out startnum) && int.TryParse(input[4], out seqs))
                {
                    level1(startnum, seqs);
                }
                else
                {
                    customMsg(MsgType.error, "Invalid inputs for -level1.");
                    return;
                }
            }

            else if (input.Length == LVL1_2 && input[5] == ">")
            {
                if (int.TryParse(input[3], out startnum) && int.TryParse(input[4], out seqs))
                {
                    this.outfilename = input[6];
                    level1(startnum, seqs, this.outfilename);
                }
                else
                {
                    customMsg(MsgType.error, "Invalid inputs for -level1.");
                    return;
                }
            }
            else { customMsg(MsgType.error, "Wrong number of inputs for level 1."); return; }
        }
        else if (this.level == "-level2")
        {
            if (input.Length == 4) {
                Level2(input[3]);
            }
            else { customMsg(MsgType.error, "Wrong number of inputs for level 2."); return; }

        }
        else if (this.level == "-level3")
        {
            if (input.Length == 5)
            {
                Level3(input[3], input[4]);
            }
            else { customMsg(MsgType.error, "Wrong number of inputs for level 3."); return; }
        }
    }

    /* Excecutes level 3*/
    public static void Level3(string queryfile, string ofile)
    {
        var result = new List<string>();
        string apppath = Environment.CurrentDirectory; //obtains current directory of .exe file
        string querypath = Path.Combine(apppath, queryfile);

        string[] seqs = GetContent(querypath);
        if (seqs == null) {  return; }
               
        foreach (string x in seqs)
        {
            string line; // line of text is being read
            counter = 0;
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            bool fail = true;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
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
                                    if (i < k.Length - 1) result.Add(Environment.NewLine);
                                }
                            }
                            else
                            {
                                result.Add(Encoding.Default.GetString(bytes)); // display the line
                            }
                            result.Add(Environment.NewLine);
                            if (n % 2 == 1) result.Add(Environment.NewLine);
                        }
                    }
                    counter++;
                }
            }
            file.Close();
            if (fail)
            {
                customMsg(MsgType.error, "Sequence ID {0} could not be found.", x);
            }
        }
        // Writing out output to a file.
        FileStream outFile = new FileStream(ofile, FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(outFile);
        string s = String.Join("", result);
        s = s.Remove(s.Length - 1);
        writer.Write(s);
        writer.Close();
        outFile.Close();
        customMsg(MsgType.success, "Output to {0} succeeded.", ofile);
    }

    /* Obtains information from the query file*/
    public static string[] GetContent(string querypath)
    {
        if (File.Exists(querypath))
        {
            string[] seqs = File.ReadAllLines(querypath);
            string disp = String.Join("\n", seqs);
            Console.WriteLine(disp);
            return seqs;
        }
        else
        {
            customMsg(MsgType.error, "Query file could not be found.");
        }
        return null;
    }

    /* Excecutes level 2*/
    public static void Level2(string sequenceid)
    {
        bool found = false;
        string line; // line of text being read
        counter = 0;
        System.IO.StreamReader file = new System.IO.StreamReader(path);
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
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
                            found = true;
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
                            System.Console.Write(Encoding.Default.GetString(bytes)); // display the line
                        }
                        if (n % 2 == 1)
                        {
                            Console.Write("\nLine number: {0}\n", counter + 1);
                        }
                    }
                }
                counter++;
            }
        file.Close();

        if (!found)
        {
            customMsg(MsgType.error, "Sequence ID {0} could not be found.", sequenceid);
            return;
        }
    }

    /* Excecutes level 1*/
    static void level1(int start, int seqs, string filename = null)
    {
        int startline = start - 1;

        if (startline < 0 || seqs < 0) { customMsg(MsgType.error, "Please enter positive and whole numbers only."); return; }
        else if ((start - 1) >= counter) { customMsg(MsgType.error, "Line number exceeds end of file."); return; }
        else if ((start) % 2 == 0) { customMsg(MsgType.error, "Please enter an odd number for the starting line position."); return; }
        else if ((start - 1) + (seqs * TOSEQ) > counter) { customMsg(MsgType.error, "Number of sequences from starting point exceeds end of file."); return; }

        // now use the parallel arrays index to access a line directly 
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
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
                else
                {
                    Console.WriteLine(Encoding.Default.GetString(bytes)); // display the line
                }
            }

            if (filename != null)
            {
                var result = new List<string>();
                for (int n = startline; n < (startline + seqs * TOSEQ); n++)
                {
                    byte[] bytes = new byte[size[n]];
                    fs.Seek(pos[n], SeekOrigin.Begin);// seek to line n (note: there is no actual disk access yet)
                    fs.Read(bytes, 0, size[n]); // get the data off disk - now there is disk access

                    if (Encoding.Default.GetString(bytes).Contains(">"))
                    {
                        string[] k = Encoding.Default.GetString(bytes).Split('>');
                        k = k.Skip(1).ToArray();

                        for (int i = 0; i<k.Length; i++)
                        {
                            result.Add(">" + k[i]);
                            if (i<k.Length-1)result.Add(Environment.NewLine);
                        }
                    }
                    else
                    {
                        result.Add(Encoding.Default.GetString(bytes)); // display the line
                    }
                    result.Add(Environment.NewLine);
                    if (n%2==1) result.Add(Environment.NewLine);
                }
                // Writing out output to a file.
                FileStream outFile = new FileStream(filename, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(outFile);
                string r = String.Join("", result);
                r = r.Remove(r.Length - 1);
                writer.Write(r);
                writer.Close();
                outFile.Close();
                customMsg(MsgType.success, "Output to {0} succeeded.", filename);
            }
        }
    }
    // =========================================================================================== //
    /*Main program excecution*/
    public bool Excecute()
    {
        string inputstring;
        string[] input;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nEnter your query: ");
        Console.ForegroundColor = ConsoleColor.White;

        inputstring = Console.ReadLine();

        if (inputstring == "quit") Exit();

        try
        {
            input = inputstring.Split(' ');
            this.program = input[0]; //program name
            this.level = input[1]; //fasta file name.
            this.file = input[2]; //level name.

            if (input[0] == "Search16s")
            {
                if (!Validfile() || (!Validlevel())) return false;
            }
            else if (input[0] == "IndexSequence16s")
            {
                if (!Validfile() || this.level != "16S.index")
                {
                    return false;
                }
            }
            else
            {
                customMsg(MsgType.error, "Enter a valid program name.");
                return false;
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            customMsg(MsgType.error, "Not enough arguments.");
            return false;
        }

        levelcheck(input);
        return true;
    }

    //==================== READ FILE ==========================/
    static void ReadFile(string filepath)
    {
        string line; // line of text i will read
        int position = 0; // file position of first line
        counter = 0;

        // Read the file and display it line by line.
        System.IO.StreamReader file = new System.IO.StreamReader(filepath);
        while ((line = file.ReadLine()) != null)
        {
            pos.Insert(counter, position); // store line position
            size.Insert(counter, line.Length + 1); // store line size
            counter++;
            position = position + line.Length + 1; // add 1 for '\n' character in file
                                                   //System.Console.WriteLine(line); // display the line
        }
        file.Close();
    }
    // ============== VALIDATE LEVEL INPUT, VALIDATE FILE INPUT ================ //
    public bool Validlevel()
    {
        string[] x = { "-level1", "-level2", "-level3", "-level4", "-level5", "-level6" };
        if (Array.Exists(x, c => c == this.level)) return true;
        else { customMsg(MsgType.error, "Please enter a valid level."); return false; }
    }
    public bool Validfile()
    {
        string apppath = Environment.CurrentDirectory;
        path = Path.Combine(apppath, this.file);
        if (File.Exists(path))
        {
            ReadFile(path);
            return true;
        }
        else
        {
            customMsg(MsgType.error, "Please enter a valid file name.");
            return false;
        }
    }

    //======================== no need to change below =========================//
    public enum MsgType { success, error }
    static void customMsg(MsgType type, string msg, string variable = null)
    {
        if (type == MsgType.success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (variable != null) Console.WriteLine(msg, variable);
            else Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (type == MsgType.error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (variable != null) Console.WriteLine(msg, variable);
            else Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    static void Exit()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n\nGoodbye!");
        Console.ForegroundColor = ConsoleColor.White;
        System.Threading.Thread.Sleep(1000);
        System.Environment.Exit(1);
    }// end Exit
}
