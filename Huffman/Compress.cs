﻿using System;
using System.Collections.Generic;
using System.IO;
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
            List<Tuple<string, int, HuffmanNode>> editableList = new List<Tuple<string, int, HuffmanNode>>(freqList);
            // We will construct a tree in order to generate our new binary encoding
            while (editableList.Count > 1)
            {
                // Sort the list by the frequency number in ascending order, removing from the tails each time
                editableList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

                // Get the number of tuples and extrac the last two nodes with the smallest frequencies
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
            DFS(rootNode, binaryEncodingStack, encodedDict);
            return encodedDict;
        }

        public void DFS(HuffmanNode node, Stack<char> binaryEncodingStack, Dictionary<char, string> encodedDict)
        {
            // Traverse each neighboring child node from left to right
            foreach(HuffmanNode neighbor in node.neighbors)
            {
                // Push the next bit on the stack
                binaryEncodingStack.Push(neighbor.bitValue);
                
                // Check and see if the neighbor is a leaf
                if(neighbor.IsLeaf())
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


        public void start()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
            List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
            List<byte> encodedKey = comp.CreateEncodingKey(myNewList);

            byte lastByteOffset = encodedText[encodedText.Count() - 1];
            encodedText.RemoveAt(encodedText.Count() - 1);
            byte [] sizeOfEncodedText = BitConverter.GetBytes(encodedText.Count());

            // Used for minor testing will remove later
            List<byte> encodedFileList = new List<byte>();
            encodedFileList.AddRange(encodedKey);
            encodedKey.AddRange(sizeOfEncodedText);
            encodedKey.Add(lastByteOffset);
            encodedKey.AddRange(encodedText);

            comp.WriteBytesToFile("mycompressed", "huf", encodedFileList);
        }
    }
}
