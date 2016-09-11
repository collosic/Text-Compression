using System;
using System.IO;
using System.Collections.Generic;

namespace Huffman
{
    interface ICommonPress
    {
        string GetTextFromFile(string fileName);
        void WriteBytesToFile(string fileName, string ext, List<byte> encodedBytes);
    }
}
