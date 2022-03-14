using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Person
    {
        public string Name { get; protected set; }
        public Hand Hand { get; private set; }

        public void TakeNewHand(Card firstCard, Card secondCart)
        {
            Hand = new Hand(firstCard, secondCart);
        }

        public void TakeCard(Card card)
        {
            Hand.AddCard(card);
        }
    }
}
