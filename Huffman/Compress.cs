using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Huffman
{
    public class Compress : Presser
    {
        // Variables needed in Compress
        private string path;
        //private string readInText;

        public Compress(string path)
        {
            this.path = String.Copy(path);
        }

        // Abstract Method implementation
        public List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

            // Check if char is ASCII
            foreach (char c in data)
            {
                if (c < 128)
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
        
        public void start()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
            List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
            List<byte> encodedKey = comp.CreateEncodingKey(myNewList);

            // Used for minor testing will remove later
            List<byte> encodedFileList = comp.BuildFullEndcodedList(encodedKey, encodedText);

            comp.WriteBytesToFile("mycompressed", "huf", encodedFileList);
        }
    }
}
