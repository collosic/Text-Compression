using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Huffman
{
    public class HuffmanCompression : HuffmanTools
    {
        // Variables needed in Compress
        private string incoming;
        private string outgoing;
        private string path;
        private int sizeOfTextFile;
        private int sizeOfCompressedFile;


        public HuffmanCompression(string incoming, string outgoing = null)
        {
            this.incoming = String.Copy(incoming);
            this.outgoing = outgoing == null ? GetOutgoingFileName(this.incoming) : outgoing;
            this.path = Path.GetDirectoryName(this.outgoing);
            this.percentage = 0;
            this.percInc = 12;
        }

        // Abstract Method implementation
        public List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();
            int ASCII_LIMIT = 128;
            int MAX_FREQ = ushort.MaxValue;

            // Check if char is ASCII
            foreach (char c in data)
            {
                if (c < ASCII_LIMIT)
                {
                    // Check and see if the frequency is past the limit of character frequency supported
                    if (frequencyTable.ContainsKey(c) && frequencyTable[c] + 1 > MAX_FREQ)
                    {
                        throw new Exception(String.Format(
                                "Max frequency of {0} characters for the '{1}' character has been exceeded!", MAX_FREQ, c));
                    } 
                    frequencyTable[c] = frequencyTable.ContainsKey(c) ? frequencyTable[c] + 1 : 1;
                }
                else
                {
                    // We've come across a character that is not ASCII
                    throw new Exception(String.Format(
                        "The following is not an ASCII character -> {0}, '{1}'", c, (int) c));
                }
            }
            List<Tuple<string, int, HuffmanNode>> frequencyList = ConvertDictToList(frequencyTable);
            DrawText(percentage += percInc, 100, progLocation);
            return frequencyList;
        }


        public string ConvertBytesToText(List<byte> bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }
                
        public List<byte> GenerateBinaryEncoding(Dictionary<char, string> encodedDict, string textFile)
        {
            List<byte> compressedText = new List<byte>();
            string binaryEncoding;
            byte byteBuffer = 0x00;
            byte bitIndex = 0x01; 
            const int END_OF_BYTE = 0x00; // 128 bit marker
            foreach (char c in textFile)
            {
                binaryEncoding = encodedDict[c];
                foreach(char bit in binaryEncoding)
                {
                    if (bit == '1')
                    {
                        byteBuffer |= bitIndex;
                    }
                    bitIndex <<= 1;

                    if (bitIndex == END_OF_BYTE)
                    {
                        // Add the encoded byte to the list and reset the byteBuffer & bitMask
                        compressedText.Add(byteBuffer);
                        byteBuffer = 0x00;
                        bitIndex = 0x01;
                    }
                }
            }
            // Move the bitIndex back one and convert
            bitIndex >>= 1;
            byte lastByteOffset = ConvertBitIndexToOffset(bitIndex);

            // Add the last bits from the last byteBuffer if an offset exists
            if (lastByteOffset != 0x00) compressedText.Add(byteBuffer);
            compressedText.Add(lastByteOffset);
            DrawText(percentage += percInc, 100, progLocation);
            return compressedText;
        }

        public byte ConvertBitIndexToOffset(byte bitIndex)
        {
            byte offset = 0;
            switch (bitIndex)
            {
                case 0x00:
                    break;
                case 0x01:
                    offset = 1;
                    break;
                case 0x02:
                    offset = 2;
                    break;
                case 0x04:
                    offset = 3;
                    break;
                case 0x08:
                    offset = 4;
                    break;
                case 0x10:
                    offset = 5;
                    break;
                case 0x20:
                    offset = 6;
                    break;
                case 0x40:
                    offset = 7;
                    break;
                default:
                    // Something went seriously wrong here
                    throw new System.ArgumentOutOfRangeException("BitIndex not in range");
            }
            return offset;
        }
        
        public List<byte> CreateEncodingKey(List<Tuple<string, int, HuffmanNode>> key)
        {
            List<byte> huffmanKey = new List<byte>();
            byte sizeOfKey = (byte) key.Count();
            byte character;

            // Put the size of the key at the front
            huffmanKey.Add(sizeOfKey);

            foreach (Tuple<string, int, HuffmanNode> t in key)
            {
                // Extract the characters and their frequencies into bytes
                character = (byte) t.Item1.ToCharArray()[0];
                byte[] freq = BitConverter.GetBytes((ushort) t.Item2);

                huffmanKey.Add(character);
                huffmanKey.AddRange(freq);
            }
            DrawText(percentage += percInc, 100, progLocation);
            return huffmanKey;
        }

        public List<byte> BuildFullEndcodedList(List<byte> keyToDecoding, List<byte> encodedText)
        {
            /* The encoded huf file will have the following structure:
             * [(1 byte) Number of characters in the Key] [KEY { (1 byte) character, (1 ushort) frequency }]
             * ...
             * [(bytes) Encoded Text] [(1 byte) Last byte offset] 
             * ...
             */

            // Create the encoded huf list
            List<byte> encodedFileList = new List<byte>();

            // Append Key and EncodedText + LastByteOffset
            encodedFileList.AddRange(keyToDecoding);
            encodedFileList.AddRange(encodedText);
            DrawText(percentage += percInc, 100, progLocation);
            return encodedFileList;
        }
        
        public void Start()
        {
            try
            {
                string textOutput = "\n Compressing: ";
                Console.Write(textOutput);
                this.progLocation = textOutput.Length;

                // Read in bytes and convert to a string buffer
                List<byte> byteList = ReadBytesFromFile(this.incoming);
                this.sizeOfTextFile = byteList.Count;
                string textBuffer = System.Text.Encoding.ASCII.GetString(byteList.ToArray());

                // Generate frequency table and Huffman tree  
                List<Tuple<string, int, HuffmanNode>> freqList = GetFrequencyList(textBuffer);
                HuffmanNode rootNode = ConstructHuffmanTree(freqList);

                // Generate Encoded dictionary (key) for converting ASCII into a new binary format        
                Dictionary<char, string> encodedDict = CreateNewBinaryDictionary(rootNode);
                
                // Using the Encoded dictionary to create the encoded text and encoded key 
                // **NOTE: The encoded key is used to decode the compressed file back into text  
                List<byte> encodedText = GenerateBinaryEncoding(encodedDict, textBuffer);
                List<byte> encodedKey = CreateEncodingKey(freqList);
                List<byte> encodedFile = BuildFullEndcodedList(encodedKey, encodedText);
                this.sizeOfCompressedFile = encodedFile.Count;

                // Write encoded bytes to file
                WriteBytesToFile(path + outgoing, "huf", encodedFile);
                DrawText(100, 100, progLocation);
                Console.WriteLine("\n");
                Console.WriteLine(" Compression Complete! \n");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n The following error occurred during compression: {0}", e.Message);
                System.Environment.Exit(1);
            }
        }

        public void VerboseMode()
        {
            if (this.sizeOfCompressedFile > this.sizeOfTextFile)
            {
                Console.WriteLine(" Unfortunately, the compression algorighm was unable to reduce the byte size =(");
                return;
            }
            else
            {
                int difference = this.sizeOfTextFile - this.sizeOfCompressedFile;
                float percent = ((float) difference / (float) sizeOfTextFile) * 100;
                string fullPath = Path.GetFullPath(Path.GetFullPath(this.outgoing));
                Console.WriteLine(" Size of original text file: {0} bytes", this.sizeOfTextFile);
                Console.WriteLine(" Size of compressed file: {0} bytes", this.sizeOfCompressedFile);
                Console.WriteLine(" Number of Bytes reduced: {0} bytes", difference);
                Console.WriteLine(" Text file was reduced by: {0:F2}%", percent);
                Console.WriteLine(" File Location: {0}", fullPath.Substring(0, fullPath.LastIndexOf('\\')));
            }
            Console.WriteLine(" File saved as {0}.{1}", this.outgoing, "huf");
        }
    }
}
