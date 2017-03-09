using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthStoneSim.Model
{
    public class Game
    {
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        public Game()
        {
            var deck1 = new Deck();
            deck1.Add(new Card(Cards.All["EX1_306"]));
            deck1.Add(new Card(Cards.All["CS2_172"]));
            deck1.Add(new Card(Cards.All["CS2_124"]));
            deck1.Add(new Card(Cards.All["CS2_182"]));
            deck1.Add(new Card(Cards.All["CS2_222"]));
            deck1.Add(new Card(Cards.All["OG_279"]));
            Player1 = new Player(deck1);
        }
    }
}
