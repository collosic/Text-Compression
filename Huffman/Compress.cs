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

                // Add the bit represention from parent to child edge
                right.SetBitOn();
                left.SetBitOff();

                // Create a new parent node and attach the left and right nodes
                string mergedChars = left._chars + right._chars;
                int mergedFreq = left._freq + right._freq;
                HuffmanNode parentNode = new HuffmanNode(mergedChars, mergedFreq, left, right);

                // Remove the last two characters from list and add the merged characters to the list
                freqList.RemoveAt(listCount - 1);
                freqList.RemoveAt(listCount - 2);

                freqList.Add(new Tuple<string, int, HuffmanNode>(mergedChars, mergedFreq, parentNode));
            }
            return freqList[0].Item3;
        }

        public Dictionary<char, string> CreateNewBinaryDictionary(HuffmanNode rootNode)
        {
            Dictionary<char, string> encodedDict = new Dictionary<char, string>();
            Stack<char> binaryEncoding = new Stack<char>();
            DFS(rootNode, binaryEncoding, encodedDict);
            return encodedDict;
        }

        public void DFS(HuffmanNode node, Stack<char> binaryEncoding, Dictionary<char, string> encodedDict)
        {
            // Traverse each neighboring child node from left to right
            foreach(HuffmanNode neighbor in node.neighbors)
            {
                // Push the next bit on the stack
                binaryEncoding.Push(neighbor.bitValue);
                
                // Check and see if the neighbors are leaves
                if(neighbor.IsLeaf())
                {
                    // Extract the bit encoding from the stack and into a string builder
                    StringBuilder binaryBits = new StringBuilder();
                    foreach (char c in binaryEncoding.Reverse()) binaryBits.Append(c);

                    // Store in the Binary Encoded Dictionary
                    encodedDict.Add(neighbor._chars.ToCharArray()[0], binaryBits.ToString());
                    binaryEncoding.Pop();
                }
                else
                {
                    DFS(neighbor, binaryEncoding, encodedDict);
                    binaryEncoding.Pop();
                }
            }
        }

        public void start()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            comp.CreateNewBinaryDictionary(root);
        }
    }
}
