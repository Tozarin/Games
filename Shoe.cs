using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Shoe
    {
        public List<Card> Cards { get; private set; }
        // Подумать, как чекать конец шохи

        public Card TookCard()
        {
            Card card = Cards.First();
            Cards.RemoveAt(0);
            return card;
        }

        public void Shuffle()
        {
            Random rnd = new Random();
            Cards = Cards.OrderBy(x => rnd.Next()).ToList();
        }

        public Shoe(int countOfDecks)
        {
            Cards = new List<Card>();
            Deck deck = new Deck();
            for (int _ = 0; _ < countOfDecks; _++)
            {
                Cards = Cards.Concat(deck.Cards).ToList();
            }
        }
    }
}
