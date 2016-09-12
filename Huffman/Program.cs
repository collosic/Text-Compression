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

                string path = @"C:\Users\Christian\Documents\mycompressed.huf";

                string myText = "abbcccXXXXZZZZZ";
                Compress comp = new Compress("Generic");
                comp.start();

                List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

                HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
                Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
                // Create Expected Key
                List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
                List<byte> expectedKey = comp.CreateEncodingKey(myNewList);


                
                // Instatiate an Uncompress object and extract the key
                Uncompress uncomp = new Uncompress(path);
                List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

                List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);

                List<byte> encodedList = uncomp.GetEncodedBytesFromFile(rawBytes);
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
