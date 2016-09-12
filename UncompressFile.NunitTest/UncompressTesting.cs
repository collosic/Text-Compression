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
    }

}
