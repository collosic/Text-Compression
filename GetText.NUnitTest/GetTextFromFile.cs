using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Huffman;

namespace GetText.NUnitTest
{
    [TestFixture]
    public class GetTextFromFile
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
            Compress c = new Compress();

            // Assert 
            Assert.That(myText, Is.EqualTo(c.GetTextFromFile(path)));

            DeleteIfExists(path);
        }

        [Test]
        public void ThrowBadFileNameException()
        {
            string path = @"C:\DoesNotExist.txt";
            Compress c = new Compress();
            Assert.Throws(typeof(FileNotFoundException), delegate { c.GetTextFromFile(path); } );
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
    }
}
