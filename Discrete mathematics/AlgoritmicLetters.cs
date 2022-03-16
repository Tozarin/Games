using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ArifmeticLetters
    {
        public char Letter { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }

        public double Prob { get; set; }

        public double GetLength()
        {
            return Right - Left;
        }

        public ArifmeticLetters(char l, double left, double right, double p)
        {
            Letter = l;
            Left = left;
            Right = right;
            Prob = p;
        }
    }
}
