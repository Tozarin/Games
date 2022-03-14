using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class Player : Person
    {
        public int Balance { get; private set; }
        public int Bet { get; private set; }

        public void MakeBet(int bet)
        {
            Bet = bet;
            Balance -= bet;
        }

        public void TookBet()
        {
            Balance += 2 * Bet;
            Bet = 0;
        }

        public Player(string name, int balance)
        {
            Name = name;
            Balance = balance;
        }
    }
}
