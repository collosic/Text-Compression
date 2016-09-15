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
        
        public HuffmanNode ConstructHuffmanTree(List<Tuple<string, int, HuffmanNode>> freqList)
        {
            List<Tuple<string, int, HuffmanNode>> editableList = new List<Tuple<string, int, HuffmanNode>>(freqList);
            // We will construct a tree in order to generate our new binary encoding
            while (editableList.Count > 1)
            {
                // Sort the list by the frequency number in ascending order, removing from the tails each time
                editableList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

                // Get the number of tuples and extract the last two nodes with the smallest frequencies
                int listCount = editableList.Count;
                HuffmanNode right = editableList.ElementAt(listCount - 1).Item3;
                HuffmanNode left = editableList.ElementAt(listCount - 2).Item3;

                // Add the bit represention from parent to child edge
                right.SetBitOn();
                left.SetBitOff();

                // Create a new parent node and attach the left and right nodes
                string mergedChars = left._chars + right._chars;
                int mergedFreq = left._freq + right._freq;
                HuffmanNode parentNode = new HuffmanNode(mergedChars, mergedFreq, left, right);

                // Remove the last two characters from list and add the merged characters to the list
                editableList.RemoveAt(listCount - 1);
                editableList.RemoveAt(listCount - 2);

                editableList.Add(new Tuple<string, int, HuffmanNode>(mergedChars, mergedFreq, parentNode));
            }
            return editableList[0].Item3;
        }
        
        public Dictionary<char, string> CreateNewBinaryDictionary(HuffmanNode rootNode)
        {
            Dictionary<char, string> encodedDict = new Dictionary<char, string>();
            Stack<char> binaryEncodingStack = new Stack<char>();

            // TODO: Comment on the DFS
            DFS(rootNode, binaryEncodingStack, encodedDict);
            return encodedDict;
        }

        public void DFS(HuffmanNode node, Stack<char> binaryEncodingStack, Dictionary<char, string> encodedDict)
        {
            // Traverse each neighboring child node from left to right
            foreach (HuffmanNode neighbor in node.neighbors)
            {
                // Push the next bit on the stack
                binaryEncodingStack.Push(neighbor.bitValue);

                // Check and see if the neighbor is a leaf
                if (neighbor.IsLeaf())
                {
                    // Extract the bit encoding from the stack and into a string builder
                    StringBuilder binaryBits = new StringBuilder();
                    foreach (char c in binaryEncodingStack.Reverse()) binaryBits.Append(c);

                    // Store in the Binary Encoded Dictionary
                    encodedDict.Add(neighbor._chars.ToCharArray()[0], binaryBits.ToString());
                    binaryEncodingStack.Pop();
                }
                else
                {
                    DFS(neighbor, binaryEncodingStack, encodedDict);
                    binaryEncodingStack.Pop();
                }
            }
        }
    }
}
