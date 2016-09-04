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



        public string getString()
        {
            return "Compressing";
        }

        public int getInt()
        {
            return 5;
        }
    }
}
