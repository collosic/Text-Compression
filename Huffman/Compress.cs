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
        public override List<Tuple<string, int>> GetFrequencyList(string data)
        {
            Dictionary<char, int> frequencyTable = new Dictionary<char, int>();

            // Check if char is ASCII
            foreach (char c in data)
            {
                if (c < 128)
                {
                    if (frequencyTable.ContainsKey(c))
                    {
                        frequencyTable[c] += 1;
                    }
                    else
                    {
                        frequencyTable.Add(c, 1);
                    }
                }
                else
                {
                    // We've come across a character that is not ASCII
                    throw new Exception("Found invalid character");
                }
            }
            List<Tuple<string, int>> frequencyList = ConvertDictToList(frequencyTable);
            return frequencyList;
        }

        public List<Tuple<string, int>> ConvertDictToList(Dictionary<char, int> table)
        {
            List<Tuple<string, int>> convertedList = new List<Tuple<string, int>>();
            foreach (KeyValuePair<char, int> entry in table)
            {
                string key = entry.Key.ToString();
                int pair = entry.Value;
                convertedList.Add(new Tuple<string, int>(key, pair));
            }
            return convertedList;
        }
    }
}
