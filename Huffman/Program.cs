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

                string myText = "The loop is unnecessary, you can just get the byte array from the string and then call the AddRange() method of List to add them to the list.";
                string fileName = @"C:\Users\Christian\Downloads\edge_case";
                Compress comp = new Compress("Generic");
                List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

                HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
                Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
                List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
                List<byte> encodedKey = comp.CreateEncodingKey(myNewList);
                List<byte> encodedFileList = comp.BuildFullEndcodedList(encodedKey, encodedText);

                comp.WriteBytesToFile(fileName, "huf", encodedFileList);

                // Uncompress
                Uncompress uncomp = new Uncompress(fileName + ".huf");
                List<byte> rawBytes = new List<byte>();
                rawBytes.AddRange(encodedFileList);

                List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
                List<byte> encodedBytesList = uncomp.GetEncodedBytesFromFile(rawBytes);
                List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

                HuffmanNode rootNode = uncomp.ConstructHuffmanTree(testList);
                Dictionary<char, string> huffmanKey = uncomp.CreateNewBinaryDictionary(rootNode);

                string decodedText = uncomp.DecodeBytes(huffmanKey, encodedBytesList);

                List<byte> myList = new List<byte>();
                myList.AddRange(Encoding.Unicode.GetBytes(decodedText));

                uncomp.WriteBytesToFile(fileName, "txt", myList);
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
