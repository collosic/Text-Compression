using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman
{
    public class HuffmanNode
    {
        public string _chars { get; private set; }
        public  int _freq { get; private set; }
        public HuffmanNode leftNode { get; private set; }
        public HuffmanNode rightNode { get; private set; }

        public HuffmanNode(string characters, int numFreq, HuffmanNode lnode = null, HuffmanNode rnode = null)
        {
            this._chars = characters;
            this._freq = numFreq;
            this.leftNode = lnode;
            this.rightNode = rnode;
        }

        public bool IsLeaf(ref HuffmanNode node)
        {
            return node.leftNode == null && node.rightNode == null;
        }


    }
}
