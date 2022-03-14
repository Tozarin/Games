using System;
using static System.Console;

namespace TaskTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck();
            Shoe shoe = new Shoe(3);
            Card card1 = new Card("9", "Q");
            Card card2 = new Card("A", "B");
            Hand hand = new Hand(card1, card2);
            WriteLine(hand.GetScore());

            hand.AddCard(new Card("A", "D"));
            WriteLine(hand.GetScore());

            WriteLine("Ended");
            ReadKey();
        }
    }
}
