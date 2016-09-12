using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Huffman
{
    public abstract class Presser : ICommonPress
    {
        //public abstract List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data);
        public List<byte> ReadBytesFromFile(string fileName)
        {
            List<byte> readInBytes = new List<byte>();

            try
            {
                byte [] readBytes = File.ReadAllBytes(fileName);
                readInBytes.AddRange(readBytes);
            }
            catch (Exception e)
            {
                throw e;
            }
            return readInBytes;
        }

        public void WriteBytesToFile(string fileName, string ext, List<byte> bytesToWrite)
        {
            string fullName = fileName + "." + ext;
            try
            {
                File.WriteAllBytes(fullName, bytesToWrite.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Tuple<string, int, HuffmanNode>> ConvertDictToList(Dictionary<char, int> table)
        {
            List<Tuple<string, int, HuffmanNode>> convertedList = new List<Tuple<string, int, HuffmanNode>>();
            foreach (KeyValuePair<char, int> entry in table)
            {
                string key = entry.Key.ToString();
                int pair = entry.Value;
                HuffmanNode node = new HuffmanNode(key, pair);
                convertedList.Add(new Tuple<string, int, HuffmanNode>(key, pair, node));
            }
            return convertedList;
        }
    }
}
