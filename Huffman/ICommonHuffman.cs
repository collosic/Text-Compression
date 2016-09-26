using System;
using System.IO;
using System.Collections.Generic;

namespace Huffman
{
    interface ICommonHuffman
    {
        List<byte> ReadBytesFromFile(string fileName);
        void WriteBytesToFile(string fileName, string ext, List<byte> encodedBytes);
        List<Tuple<string, int, HuffmanNode>> ConvertDictToList(Dictionary<char, int> table);
        HuffmanNode ConstructHuffmanTree(List<Tuple<string, int, HuffmanNode>> freqList);
        Dictionary<char, string> CreateNewBinaryDictionary(HuffmanNode rootNode);
        void DFS(HuffmanNode node, Stack<char> binaryEncodingStack, Dictionary<char, string> encodedDict);
    }
}
