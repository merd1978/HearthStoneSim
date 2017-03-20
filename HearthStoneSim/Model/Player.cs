using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthStoneSim.Model
{
    public class Player
    {
        public Deck Deck { get; private set; };
        public string FriendlyName { get; }

        public Player(Deck deck)
        {
            Deck = deck;
        }
    }
}
