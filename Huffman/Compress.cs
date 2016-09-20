using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Huffman
{
    public class Compress : Presser
    {
        // Variables needed in Compress
        private string incoming;
        private string outgoing;
        private string path;
        private int sizeOfTextFile;
        private int sizeOfCompressedFile;


        public Compress(string incoming, string outgoing = null)
        {
            this.incoming = String.Copy(incoming);
            this.outgoing = outgoing == null ? GetOutgoingFileName(this.incoming) : outgoing;
            this.path = Path.GetDirectoryName(this.outgoing);
        }

        // Abstract Method implementation
        public List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();
            int ASCII_LIMIT = 128;

            // Check if char is ASCII
            foreach (char c in data)
            {
                if (c < ASCII_LIMIT)
                {
                    frequencyTable[c] = frequencyTable.ContainsKey(c) ? frequencyTable[c] + 1 : 1;
                }
                else
                {
                    // We've come across a character that is not ASCII
                    throw new Exception("Found invalid character");
                }
            }
            List<Tuple<string, int, HuffmanNode>> frequencyList = ConvertDictToList(frequencyTable);
            return frequencyList;
        }

        private string GetOutgoingFileName(string filepath)
        {
            return Path.GetFileNameWithoutExtension(filepath);
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
                    break;
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
            return huffmanKey;
        }

        public List<byte> BuildFullEndcodedList(List<byte> keytoEncoding, List<byte> encodedText)
        {
            /* The encoded huf file will have the following structure:
             * [(byte) Number of characters in the Key] [KEY { (byte) character, (ushort) frequency }]
             * ...
             * [(int) Number of encoded bytes] [(byte) Last byte offset] [(byte) Encoded Text]
             * ...
             */

            // Number of encoded bytes
            byte[] numberOfBytesEncoded = BitConverter.GetBytes(encodedText.Count());


            List<byte> encodedFileList = new List<byte>();

            // Append Key and EncodedText + LastByteOffst
            encodedFileList.AddRange(keytoEncoding);
            encodedFileList.AddRange(encodedText);

            return encodedFileList;
        }
        
        public void Start()
        {
            try
            {
                Console.Write(" Compressing:", Path.GetFileName(this.incoming));
                int progress = 0;
                // Read in bytes and convert to a string buffer
                DrawText(progress += 10, 100);
                List<byte> byteList = ReadBytesFromFile(this.incoming);
                this.sizeOfTextFile = byteList.Count;
                DrawText(progress += 10, 100);
                string textBuffer = System.Text.Encoding.UTF8.GetString(byteList.ToArray());

                // Generate frequency table and Huffman tree  
                DrawText(progress += 10, 100);
                List<Tuple<string, int, HuffmanNode>> freqList = GetFrequencyList(textBuffer);
                DrawText(progress += 10, 100);
                HuffmanNode rootNode = ConstructHuffmanTree(freqList);

                // Generate Encoded dictionary (key) for converting ASCII into a new binary format        
                DrawText(progress += 10, 100);
                Dictionary<char, string> encodedDict = CreateNewBinaryDictionary(rootNode);


                // Using the Encoded dictionary to create the encoded text and encoded key 
                // **NOTE: The encoded key is used to decode the compressed file back into text  
                DrawText(progress += 10, 100);
                List<byte> encodedText = GenerateBinaryEncoding(encodedDict, textBuffer);
                DrawText(progress += 10, 100);
                List<byte> encodedKey = CreateEncodingKey(freqList);
                DrawText(progress += 10, 100);
                List<byte> encodedFile = BuildFullEndcodedList(encodedKey, encodedText);
                DrawText(progress += 10, 100);
                this.sizeOfCompressedFile = encodedFile.Count;

                // Write encoded bytes to file
                DrawText(progress += 10, 100);
                WriteBytesToFile(path + outgoing, "huf", encodedFile);
                DrawText(progress, 100);
                Console.WriteLine("\n");
                Console.WriteLine(" Completed Compression! \n");
            }
            catch (Exception e)
            {
                Console.WriteLine(" \n\nThe following error occurred during compression: {0}", e.Message);
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
            Console.Read();
        }

        private void DrawText(int progress, int total)
        {
            int start = " Compressing: ".Count() - 1;
            Console.CursorLeft = start + 1;
            Console.Write("[");
            Console.CursorLeft = start + 33;
            Console.Write("]");
            Console.CursorLeft = start + 2;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = start + 2;
            for (int i = 0; i < (onechunk * progress); i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;                
                Console.CursorLeft = position++;
                Console.Write("=");
            }
            Console.ResetColor();
            //draw unfilled part
            for (int i = position; i <= start + 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = start + 36;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
