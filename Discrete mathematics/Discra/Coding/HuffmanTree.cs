namespace Discra
{
    public class HuffmanTree
    {
        private List<HuffmanNode> nodes;
        public Dictionary<char, string> Codes { get; }


        public HuffmanTree()
        {
            nodes = new List<HuffmanNode>();
            Codes = new Dictionary<char, string>();
        }

        public void InitTree(Dictionary<char, double> alphabet)
        {
            foreach(var pair in alphabet.Reverse())
            {
                nodes.Add(new HuffmanNode(pair.Key, pair.Value));
            }

            while(nodes.Count > 1)
            {
                var left = nodes.First();
                nodes.RemoveAt(0);
                var right = nodes.First();
                nodes.RemoveAt(0);

                var newNode = new HuffmanNode('\0', left.Prob + right.Prob, left, right);
                nodes.Add(newNode);

                nodes = nodes.OrderBy(x => x.Prob).ToList();
            }
        }

        public void GenerateCodes()
        {
            nodes = nodes.OrderByDescending(x => x.Prob).ToList();
            Search("", nodes.First());
        }

        public void Search(string code, HuffmanNode node)
        {
            if (node.Value != '\0')
            {
                Codes.Add(node.Value, code);
            }

            if (node.Left != null)
            {
                Search(code + 1, node.Left);
            }

            if (node.Right != null)
            {
                Search(code + 0, node.Right);
            }
        }
    }
}
