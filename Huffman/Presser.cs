using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Huffman
{
    public abstract class Presser : ICommonPress
    {
        public FileStream CloseFile(string fileName)
        {
            throw new Exception("Not now");

        }

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
    }
}
