using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class Compress : Presser
    {
        // Variables needed in Compress
        private string path;
        private string readInText;

        public Compress(string path)
        {
            this.path = String.Copy(path);
        }

        // Abstract Method implementation
        public override List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

            // Check if char is ASCII
            foreach (char c in data)
            {
                if (c < 128)
                {
                    if (frequencyTable.ContainsKey(c))
                    {
                        frequencyTable[c] += 1;
                    }
                    else
                    {
                        frequencyTable.Add(c, 1);
                    }
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

        public HuffmanNode ConstructHuffmanTree(List<Tuple<string, int, HuffmanNode>> freqList)
        {
            // We will construct a tree in order to generate our new binary encoding
            while (freqList.Count > 1)
            {
                // Sort the list by the frequency number in ascending order, removing from the tails each time
                freqList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

                // Get the number of tuples and extrac the last two nodes with the smallest frequencies
                int listCount = freqList.Count;
                HuffmanNode right = freqList.ElementAt(listCount - 1).Item3;
                HuffmanNode left = freqList.ElementAt(listCount - 2).Item3;

                // Create a new parent node and attache the left and right nodes
                string mergedChars = left._chars + right._chars;
                Console.WriteLine(mergedChars);
                int mergedFreq = left._freq + right._freq;
                HuffmanNode parentNode = new HuffmanNode(mergedChars, mergedFreq, left, right);

                // Remove the last two characters from list and add the merged characters to the list
                freqList.RemoveAt(listCount - 1);
                freqList.RemoveAt(listCount - 2);

                freqList.Add(new Tuple<string, int, HuffmanNode>(mergedChars, mergedFreq, parentNode));
            }
            return freqList[0].Item3;
        }

        public void start()
        {
            
        }
    }
}
