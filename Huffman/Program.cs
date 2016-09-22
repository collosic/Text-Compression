using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huffman
{

    class Program
    {
        static void Main(string[] args)
        {
            Arguments arguments = new Arguments(args);
            try
            {
                arguments.checkArguments();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Invalid argument: {0}, use /h or /? for help", e.Message);
                System.Environment.Exit(1);
            }

            switch (arguments.state)
            {
                case State.Compress:
                    Compress compress = new Compress(arguments.incomingFileName, arguments.outgoingFileName);
                    compress.Start();
                    if (arguments.isVerbose) compress.VerboseMode();
                    break;
                case State.Uncompress:
                    Uncompress uncompress = new Uncompress(arguments.incomingFileName, arguments.outgoingFileName);
                    uncompress.Start();
                    break;
                case State.Help:
                    displayHelpMessage();
                    break;
                default:
                    Console.WriteLine("FATAL ERROR: Invalid State!");
                    break;                
            }
        }

        private static void displayHelpMessage()
        {
            Console.WriteLine("Will compress and uncompress a text file using the Huffman Coding algorithm.\n");
            Console.WriteLine("HUFFMAN [/C | /U] [/V] [source] [destination]\n");
            Console.WriteLine("  filename\tSpecifies the name of the text file");
            Console.WriteLine("  /C\t\tUsed to compress a text file");
            Console.WriteLine("  /U\t\tUsed to uncompress a huff file");
            Console.WriteLine("  /V\t\tIndicates verbose mode");
            Console.WriteLine("  source\t\tName of incoming file");
            Console.WriteLine("  destination\t\tPath to the location of the output file, if none provided source path is used");
        }
    }
}
