using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Hand
    {
        public List<Card> Cards { get; private set; }

        public int GetScore()
        {
            List<int> rezults = new List<int> { 0 };
            foreach (Card card in Cards)
            {
                int value = card.GetValue();
                rezults = rezults.Select(x => x += value).ToList();

                if (value == 1) // Ace
                {
                    List<int> anotherRezults = rezults.Select(x => x += 10).ToList(); // +11 total
                    rezults = rezults.Concat(anotherRezults).ToList();
                }
            }

            return rezults.All(x => x > 21) ? rezults.First() : rezults.Where(x => x < 22).Max();
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public Hand(Card firstCard, Card secondCart)
        {
            Cards = new List<Card>();
            Cards.Add(firstCard);
            Cards.Add(secondCart);
        }
    }
}
