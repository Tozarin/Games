using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Card
    {
        public string Suit { get; private set; }
        public string Cost { get; private set; }

        public int GetValue()
        {
            if (Cost == "A")
            {
                return 1;
            }
            else if (Cost.All(x => x <= '9'))
            {
                return Int32.Parse(Cost);
            }
            else
            {
                return 10;
            }
        }

        public Card(string cost, string suit)
        {
            Cost = cost;
            Suit = suit;
        }
    }
}
