using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Huffman
{
    public class Uncompress : Presser
    {
        private string path;
        public Uncompress(string path)
        {
            this.path = String.Copy(path);
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
            List<byte> byteCopy = new List<byte>(rawBytes);
            List<byte> encodedList = new List<byte>();
            int encodedOffset = (int) byteCopy[0];
            byteCopy.RemoveAt(0);

            byteCopy.RemoveRange(0, (encodedOffset * 3));

            return byteCopy;
        }

        public string DecodeBytes(Dictionary<char, string> huffmanKey, List<byte> encodedBytes)
        {
            // Extract the Last Byte Offset from the encoded bytes list
            int numOfBytesLeft = encodedBytes.Count();
            int lastByteOffset = encodedBytes[numOfBytesLeft - 1];
            encodedBytes.RemoveAt(numOfBytesLeft - 1);
            numOfBytesLeft--;
            
            int numBitsInByte = 8;
            StringBuilder encodedBits = new StringBuilder();
            StringBuilder decodedText = new StringBuilder();
            

            foreach (byte b in encodedBytes)
            {
                int bitLimit = (--numOfBytesLeft > 0) ? numBitsInByte : lastByteOffset;

                for (int i = 1; i <= bitLimit; ++i)
                {
                    char bit = ((b & (1 << i - 1)) != 0) ? '1' : '0';
                    encodedBits.Append(bit);
                    char character = huffmanKey.FirstOrDefault(
                                            x => x.Value == encodedBits.ToString()).Key;

                    if (character != '\0')
                    {
                        decodedText.Append(character);
                        encodedBits.Clear();
                    }
                }                
            }
            return decodedText.ToString();
        }
    }
}
