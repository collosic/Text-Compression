using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huffman
{
    public class Uncompress : Presser
    {
        private string incoming;
        private string outgoing;
        private string path;
        private int sizeOfTextFile;
        private int sizeOfCompressedFile;

        public Uncompress(string incoming, string outgoing = null)
        {
            this.incoming = String.Copy(incoming);
            this.outgoing = outgoing == null ? GetOutgoingFileName(this.incoming) : outgoing;
            this.path = Path.GetDirectoryName(this.outgoing);
            this.percentage = 0;
            this.percInc = 12;
        }

        public List<Tuple<string, int, HuffmanNode>> GetFrequencyList(List<byte> keyList)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();
            int sizeOfKey = keyList.Count() / 3; 

            for (int i = 0; i < keyList.Count(); i += 3)
            {
                char chars = Convert.ToChar(keyList[i]);
                byte[] freqArray = { keyList[i + 1], keyList[i + 2] };
                int freq = BitConverter.ToInt16(freqArray, 0);
                frequencyTable.Add(chars, freq);
            }

            List<Tuple<string, int, HuffmanNode>> frequencyList = ConvertDictToList(frequencyTable);
            return frequencyList;
        }

        public List<byte> GetKeyFromBytes(List<byte> rawBytes)
        {
            List<byte> keyList = new List<byte>();
            byte numOfCharacters = rawBytes[0];
            int charOffset = 3;

            for (int i = 1; i <= numOfCharacters * charOffset; ++i)
            {
                keyList.Add(rawBytes[i]);
            }
            return keyList;
        }

        public List<byte> GetEncodedBytesFromFile(List<byte> rawBytes)
        {
            List<byte> encodedTextCopy = new List<byte>(rawBytes);
            int numOfKeyValuePairs = (int) encodedTextCopy[0];
            int numOfBytesPerKeyValuePair = 3;

            // Remove the keyOffset byte followed by the huffman key
            encodedTextCopy.RemoveAt(0);
            encodedTextCopy.RemoveRange(0, (numOfKeyValuePairs * numOfBytesPerKeyValuePair));
            return encodedTextCopy;
        }

        public string DecodeBytes(Dictionary<char, string> huffmanKey, List<byte> encodedBytes)
        {
            int numBitsInByte = 8;
            int numOfBytesLeft = encodedBytes.Count();
            int lastByteOffset = encodedBytes[numOfBytesLeft - 1];
            encodedBytes.RemoveAt(numOfBytesLeft - 1);
            numOfBytesLeft = encodedBytes.Count();

            StringBuilder encodedBits = new StringBuilder();
            StringBuilder decodedText = new StringBuilder();
            
            foreach (byte b in encodedBytes)
            {
                int bitLimit = (--numOfBytesLeft > 0 || lastByteOffset == 0) ? numBitsInByte : lastByteOffset;

                for (int i = 1; i <= bitLimit; ++i)
                {
                    // Test if the bit is turned on or off and append to string
                    char bit = ((b & (1 << i - 1)) != 0) ? '1' : '0';
                    encodedBits.Append(bit);

                    // Do a reverse dictionary lookup for matching bit strings
                    char character = huffmanKey.FirstOrDefault(
                                            x => x.Value == encodedBits.ToString()).Key;
                    // If the character is not \0 then add it to decoded text string
                    if (character != '\0')
                    {
                        decodedText.Append(character);
                        encodedBits.Clear();
                    }
                }                
            }
            return decodedText.ToString();
        }

        public void Start()
        {
            try
            {
                string textOutput = "\n Decoding: ";
                Console.Write(textOutput);
                this.progLocation = textOutput.Length;

                // Read in bytes and convert to a string buffer
                List<byte> encodedBytes = ReadBytesFromFile(this.incoming);

                // Extract decoding key and the encoded/compressed text and generate a frequency list
                List<byte> decodingKey = GetKeyFromBytes(encodedBytes);
                List<byte> encodedText = GetEncodedBytesFromFile(encodedBytes);
                List<Tuple<string, int, HuffmanNode>> freqList = GetFrequencyList(decodingKey);

                // Create huffman Tree and decoding table
                HuffmanNode rootNode = ConstructHuffmanTree(freqList);
                Dictionary<char, string> decodingDict = CreateNewBinaryDictionary(rootNode);

                // Using the decoding table decode the text.  Dump text into a List of bytes
                string decodedText = DecodeBytes(decodingDict, encodedText);
                List<byte> decodedBytes = new List<byte>();
                decodedBytes.AddRange(Encoding.Unicode.GetBytes(decodedText));

                // Write decoded bytes to file
                WriteBytesToFile(path + outgoing, "txt", decodedBytes);
                DrawText(100, 100, progLocation);
                Console.WriteLine("\n");
                Console.WriteLine(" Decoding Complete! \n");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n The following error occurred during decoding: {0}", e.Message);
                System.Environment.Exit(1);
            }

        }

        public void VerboseMode()
        {

        }
    }
}
