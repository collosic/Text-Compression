using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // arguments.display();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Invalid argument: {0}, use /h or /? for help", e.Message);
                System.Environment.Exit(1);
            }

            if (arguments.getArgs()
                .FirstOrDefault(stringToCheck => stringToCheck.Contains("h")) != null)
            {
                displayHelpMessage();
            }
            else if (arguments.getArgs()
                .FirstOrDefault(stringToCheck => stringToCheck.Contains("c")) != null)
            {
                Console.WriteLine("Let's begin compressing...");
                Compress comp = new Compress(arguments.fileName);
                comp.start();
            }
            else if (arguments.getArgs()
                .FirstOrDefault(stringToCheck => stringToCheck.Contains("u")) != null)
            {
                
            }
            else
            {

            }

        }

        private static void displayHelpMessage()
        {
            Console.WriteLine("Will compress and uncompress a text file using the Huffman Coding algorithm.\n");
            Console.WriteLine("HUFFMAN [/C | /U] [/V] filename\n");
            Console.WriteLine("  filename\tSpecifies the name of the text file");
            Console.WriteLine("  /C\t\tUsed to compress a text file");
            Console.WriteLine("  /U\t\tUsed to uncompress a huff file");
            Console.WriteLine("  /V\t\tIndicates verbose mode");
        }
    }
}
