﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Huffman;

namespace CompressText.NUnitTest
{
    [TestFixture]
    public class CompressText
    {
        [Test]
        public void ReturnCorrectText()
        {
            // create string text to test
            string myText = "Here we just want to make sure that our text is the same.";
            string path = @"C:\Users\Christian\Documents\myText.txt";

            DeleteIfExists(path);

            // Create the file now
            File.WriteAllText(path, myText);

            // Instantiate the Class and begin test
            Compress c = new Compress(path);

            // read in bytes and test the characters 
            List<byte> readInBytes = c.ReadBytesFromFile(path);

            // Assert 
            Assert.That(myText, Is.EqualTo(c.ConvertBytesToText(readInBytes)));

            DeleteIfExists(path);
        }

        [Test]
        public void ThrowBadFileNameException()
        {
            string path = @"C:\DoesNotExist.txt";
            Compress c = new Compress(path);
            Assert.Throws(typeof(FileNotFoundException), delegate { c.ReadBytesFromFile(path); } );
        }

        private void DeleteIfExists(string path)
        {
            // Delete the file we are creating if it exists
            if (File.Exists(path))
            {
                Console.WriteLine("Deleting File...");
                File.Delete(path);
            }
        }

        [Test]
        public void TestConvertToList()
        {
            Dictionary<char, int> myDict = new Dictionary<char, int>();
            myDict.Add('z', 4);
            myDict.Add('v', 9);
            myDict.Add('J', 2);

            List<Tuple<string, int>> myList = new List<Tuple<string, int>>();
            myList.Add(new Tuple<string, int>("z", 4));
            myList.Add(new Tuple<string, int>("v", 9));
            myList.Add(new Tuple<string, int>("J", 2));

            Compress comp = new Compress("genericPath");
            List<Tuple<string, int, HuffmanNode>> convertedList = comp.ConvertDictToList(myDict);

            // Quickly sort by integer in DESC order 
            convertedList.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            myList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            for (int i = 0; i < myList.Count; i++)
            { 
                Assert.That(convertedList[i].Item1, Is.EqualTo(myList[i].Item1));
                Assert.That(convertedList[i].Item2, Is.EqualTo(myList[i].Item2));
            }

        }
        
        [Test]
        public void GetListFromTextFile()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            myNewList.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            // generate list manurally from myText above
            List<Tuple<string, int>> testList = new List<Tuple<string, int>>();
            testList.Add(new Tuple<string, int>("a", 1));
            testList.Add(new Tuple<string, int>("b", 2));
            testList.Add(new Tuple<string, int>("c", 3));
            testList.Add(new Tuple<string, int>("X", 4));
            testList.Add(new Tuple<string, int>("Z", 5));

            // Sort the new list in ASC order
            myNewList.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            for (int i = 0; i < myNewList.Count; i++)
            {
                Assert.That(testList[i].Item1, Is.EqualTo(myNewList[i].Item1));
                Assert.That(testList[i].Item2, Is.EqualTo(myNewList[i].Item2));
            }
        }

        [Test]
        public void ThrowInvalidCharacterFound()
        {
            string myText = "alkdjsfkyehcƒ €";
            Compress comp = new Compress("Generic");
            
            Assert.Throws(typeof(Exception), delegate { comp.GetFrequencyList(myText); } );
        }

        [Test]
        public void TestRootNode()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Assert.That(myText.Count, Is.EqualTo(root._freq));
        }

        [Test]
        public void TestRootNodeAgain()
        {
            string myText = "Mississippi River";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode root = comp.ConstructHuffmanTree(myNewList);
            Assert.That(myText.Count, Is.EqualTo(root._freq));
        }

        [Test]
        public void EncodedDictionaryTest()
        {
            string myText = "abbcccXXXXZZZZZ";
            Dictionary<char, string> testDict = new Dictionary<char, string>();

            testDict.Add('a', "111");
            testDict.Add('b', "110");
            testDict.Add('c', "10");
            testDict.Add('X', "01");
            testDict.Add('Z', "00");
            
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            HuffmanNode rootNode = comp.ConstructHuffmanTree(myNewList);
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(rootNode);

            Assert.AreEqual(ToAssertTableString(testDict), ToAssertTableString(encodedDict));          
        }

        public string ToAssertTableString(IDictionary<char, string> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        [Test]
        public void CheckEncodedBinaryList()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");

            HuffmanNode rootNode = comp.ConstructHuffmanTree(comp.GetFrequencyList(myText));
            Dictionary<char, string> encodedDict = comp.CreateNewBinaryDictionary(rootNode);
            List<byte> encodedBytes = comp.GenerateBinaryEncoding(encodedDict, myText);

            List<byte> expectedBytes = new List<byte>() { 0xDF, 0x2A, 0x55, 0x00, 0x00, 0x01 };

            CollectionAssert.AreEqual(expectedBytes, encodedBytes);
        }

        [Test]
        public void TestOffsetConverter()
        {
            Compress comp = new Compress("Generic");
            byte bitIndex = 0x01;

            for (int i = 0; i <= 7; ++i)
            {
                byte test = (byte) (bitIndex == 0x00 ? 0x80 : bitIndex >> 1);
                byte offset = comp.ConvertBitIndexToOffset(test);
                Assert.That(i, Is.EqualTo(offset));
                bitIndex <<= 1;
            }
        }

        [Test]
        public void HuffmanKeyTest()
        {
            string myText = "abbcccXXXXZZZZZ";
            Compress comp = new Compress("Generic");
            List<Tuple<string, int, HuffmanNode>> myNewList = comp.GetFrequencyList(myText);

            List<byte> expectedKey = new List<byte>()
            { 0x05, 0x61, 0x01, 0x00, 0x62, 0x02, 0x00, 0x63, 0x03, 0x00, 0x58, 0x04, 0x00, 0x5A, 0x05, 0x00 };

            List<byte> returnedKey = comp.CreateEncodingKey(myNewList);

            CollectionAssert.AreEqual(expectedKey, returnedKey);
        }
    }
}
