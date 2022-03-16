using System.Collections.Generic;

namespace Tests
{
    public class Letters
    {
        public double Prob { get; set; }
        public char Letter { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public List<int> ConnectedElements { get; set; }

        public Letters(double p, int n, char l)
        {
            Prob = p;
            Number = n;
            Letter = l;
            Code = "";
            ConnectedElements = new List<int> { n };
        }
    }
}
