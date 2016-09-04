using System;
using System.IO;
using System.Collections.Generic;

namespace Huffman
{
    interface ICommonPress
    {
        string GetTextFromFile(string fileName);
        FileStream CloseFile(string fileName);
    }
}
