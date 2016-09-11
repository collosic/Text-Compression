using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Huffman
{
    public abstract class Presser : ICommonPress
    {
        public abstract List<Tuple<string, int, HuffmanNode>> GetFrequencyList(string data);
        public string GetTextFromFile(string fileName)
        {
            FileStream fs = null;
            string readText = "";
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs = null;
                    readText = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
            return readText;
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
    }
}
