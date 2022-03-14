using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTests
{
    public class BlackJack
    {
        private Croupier Croupier { get; set; }
        private List<Player> Players { get;  set; }
        private Shoe Shoe { get; set; }

        private void StartNewTurn()
        {
            Shoe = new Shoe(8);
            Shoe.Shuffle();
            Croupier.TakeNewHand(Shoe.TookCard(), Shoe.TookCard());
            foreach(Player player in Players)
            {
                player.TakeNewHand(Shoe.TookCard(), Shoe.TookCard());
            }
        }

        private void StartPlayersActions()
        {
            foreach(Player player in Players)
            {
                // Ставка
                // Действия
            }
        }

        private void StartCroupiersActions()
        {
            while(Croupier.Hand.GetScore() < 17)
            {
                Croupier.TakeCard(Shoe.TookCard());
            }
        }

        private void EndTurn()
        {
            foreach(Player player in Players)
            {
                player.TookBet();
                if (player.Balance <= 0)
                {
                    Players.Remove(player);
                }
            }
        }

        public void Start()
        {
            do
            {
                StartNewTurn();
                StartPlayersActions();
                StartCroupiersActions();
                EndTurn();
            } while (Players.Count > 0);
        }

        public BlackJack(Croupier croupier, List<Player> players)
        {
            Croupier = croupier;
            Players = players;
        }
    }
}
