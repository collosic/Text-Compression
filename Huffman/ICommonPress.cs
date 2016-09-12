using System;
using System.IO;
using System.Collections.Generic;

namespace Huffman
{
    interface ICommonPress
    {
        List<byte> ReadBytesFromFile(string fileName);
        void WriteBytesToFile(string fileName, string ext, List<byte> encodedBytes);
    }
}
