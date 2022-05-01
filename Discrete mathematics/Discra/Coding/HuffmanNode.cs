namespace Discra
{
    public class HuffmanNode
    {
        public char Value { get; }
        public double Prob { get; }
        public HuffmanNode Left { get; }
        public HuffmanNode Right { get; }

        public HuffmanNode(char c, double prob, HuffmanNode left = null, HuffmanNode right = null)
        {
            Value = c;
            Left = left;
            Right = right;
            Prob = prob;
        }

    }
}
