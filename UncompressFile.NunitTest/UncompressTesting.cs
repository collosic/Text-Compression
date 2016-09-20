using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Huffman;

namespace UncompressFile.NunitTest
{
    [TestFixture]
    public class UncompressTesting
    {
        [Test]
        public void ReadInBytesFromFileTest()
        {
            // set path to a huf file on the local disk
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";

            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
            List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
            List<byte> encodedKey = comp.CreateEncodingKey(myNewList);

            List<byte> expectedBytes = comp.BuildFullEndcodedList(encodedKey, encodedText);

            Uncompress uncomp = new Uncompress(path);
            List<byte> testList = uncomp.ReadBytesFromFile(path);

            CollectionAssert.AreEqual(expectedBytes, testList);
        }

        [Test]
        public void GetKeyfromList()
        {
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";

            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            // Create Expected Key
            List<byte> expectedKey = comp.CreateEncodingKey(myNewList);
            expectedKey.RemoveAt(0);

            // Instatiate an Uncompress object and extract the key
            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            CollectionAssert.AreEqual(expectedKey, testKey);

        }

        [Test]
        public void ByteListToFrequencyListTest()
        {
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> expectedList = comp.GetFrequencyList(myText);

            // Instatiate an Uncompress object and extract the key
            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

            expectedList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            testList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.That(expectedList[i].Item1, Is.EqualTo(testList[i].Item1));
                Assert.That(expectedList[i].Item2, Is.EqualTo(testList[i].Item2));
            }
        }

        [Test]
        public void GetEncodedBytesFromList()
        {
            // set path to a huf file on the local disk
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";

            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
            List<byte> expectedList = comp.GenerateBinaryEncoding(encodedDict, myText);

            // Instatiate an Uncompress object and extract the key
            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> encodedList = uncomp.GetEncodedBytesFromFile(rawBytes);
            CollectionAssert.AreEqual(expectedList, encodedList);
        }

        [Test]
        public void TestRootNodeFromEncodedFile()
        {
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";
            string myText = "abbcccXXXXZZZZZ";

            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

            HuffmanNode root = uncomp.ConstructHuffmanTree(testList);
            Assert.That(myText.Count, Is.EqualTo(root._freq));
        }

        [Test]
        public void EncodedDictionaryFromEncodedFileTest()
        {
            Dictionary<char, string> testDict = new Dictionary<char, string>();
            testDict.Add('a', "111");
            testDict.Add('b', "110");
            testDict.Add('c', "10");
            testDict.Add('X', "01");
            testDict.Add('Z', "00");

            string path = @"C:\Users\Christian\Documents\mycompressed.huf";
            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

            HuffmanNode root = uncomp.ConstructHuffmanTree(testList);
            Dictionary<char, string> encodedDict = uncomp.CreateNewBinaryDictionary(root);

            Assert.AreEqual(ToAssertTableString(testDict), ToAssertTableString(encodedDict));
        }

        public string ToAssertTableString(IDictionary<char, string> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        [Test]
        public void DecodeTextCompression()
        {
            string path = @"C:\Users\Christian\Documents\mycompressed.huf";
            string myText = "abbcccXXXXZZZZZ";

            Uncompress uncomp = new Uncompress(path);
            List<byte> rawBytes = uncomp.ReadBytesFromFile(path);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            List<byte> encodedBytesList = uncomp.GetEncodedBytesFromFile(rawBytes);
            List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

            HuffmanNode root = uncomp.ConstructHuffmanTree(testList);
            Dictionary<char, string> huffmanKey = uncomp.CreateNewBinaryDictionary(root);

            string decodedText = uncomp.DecodeBytes(huffmanKey, encodedBytesList);
            Assert.That(myText, Is.EqualTo(decodedText));
        }

        [Test]
        public void DecodeTextOnEdgeCase()
        {
            // Compress
            string myText = "aaabbbbbcccccccc";
            string fileName = @"C:\Users\Christian\Downloads\edge_case";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(root);
            List<byte> encodedText = comp.GenerateBinaryEncoding(encodedDict, myText);
            List<byte> encodedKey = comp.CreateEncodingKey(myNewList);
            List<byte> encodedFileList = comp.BuildFullEndcodedList(encodedKey, encodedText);

            // Uncompress
            Uncompress uncomp = new Uncompress(fileName + ".huf");
            List<byte> rawBytes = new List<byte>();
            rawBytes.AddRange(encodedFileList);

            List<byte> testKey = uncomp.GetKeyFromBytes(rawBytes);
            List<byte> encodedBytesList = uncomp.GetEncodedBytesFromFile(rawBytes);
            List<Tuple<string, int, HuffmanNode>> testList = uncomp.GetFrequencyList(testKey);

            HuffmanNode rootNode = uncomp.ConstructHuffmanTree(testList);
            Dictionary<char, string> huffmanKey = uncomp.CreateNewBinaryDictionary(rootNode);

            string decodedText = uncomp.DecodeBytes(huffmanKey, encodedBytesList);

            List<byte> myList = new List<byte>();
            myList.AddRange(Encoding.Unicode.GetBytes(decodedText));
            Assert.That(myText, Is.EqualTo(decodedText));
        }
        [Test]
        public void ArgumentsUnitTestCases()
        {
            // Test for too few arguments
            string[] args1 = { "/c" };
            Arguments arguments = new Arguments(args1);
            Assert.Throws(typeof(ArgumentException), delegate { arguments.checkArguments(); });

            // Test for too many arguments
            string[] args2 = { "/c", "/v", "/h", "toomuch.txt",  "yaaasssss" };
            arguments = new Arguments(args2);
            Assert.Throws(typeof(ArgumentException), delegate { arguments.checkArguments(); });

            // Test for options only
            string[] args3 = { "/c", "/v" };
            arguments = new Arguments(args3);
            Assert.Throws(typeof(Exception), delegate { arguments.checkArguments(); });

            // Test for file names only
            string[] args4 = { "myfile.txt", "hello" };
            arguments = new Arguments(args4);
            Assert.Throws(typeof(Exception), delegate { arguments.checkArguments(); });

            string[] args5 = { "myfile.txt" };
            arguments = new Arguments(args5);
            Assert.Throws(typeof(Exception), delegate { arguments.checkArguments(); });

            // Test for edge cases


        }
    }
}
