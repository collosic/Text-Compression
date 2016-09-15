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

                // Instatiate an Uncompress object and extract the key
                string path = @"C:\Users\Christian\Documents\mycompressed.huf";
                string myText = "abbcccXXXXZZZZZ";

                Uncompress uncomp = new Uncompress(path);
                List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

                List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
                List<byte> encodedBytesList = uncomp.GetEncodedBytesFromFile(rawBytes);
                List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

                HuffmanNode root = uncomp.ConstructHuffmanTree(testList);
                Dictionary<char, string> huffmanKey = uncomp.CreateNewBinaryDictionary(root);

                string decodedText = uncomp.DecodeBytes(huffmanKey, encodedBytesList);
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
