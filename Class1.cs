using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Letters
    {
        public double P { get; set; }
        public int Number { get; set; }
        public string Code { get; set; }
        public List<int> OtherElements { get; set; }

        public Letters(double p, int n)
        {
            P = p;
            Number = n;
            Code = "";
            OtherElements = new List<int> { n };
        }
    }
}
