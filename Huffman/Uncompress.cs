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
    }
}
