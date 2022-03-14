using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }

        public Deck()
        {
            Cards = new List<Card>();
            string[] suits = { "H", "D", "P", "B" };
            string[] costs = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            foreach (string suit in suits)
            {
                foreach(string cost in costs)
                {
                    Cards.Add(new Card(cost, suit));
                }
            }
        }
    }
}
