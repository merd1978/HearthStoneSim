using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthStoneSim.Model
{
    public class Player
    {
        private Deck _deck;
        public string FriendlyName { get; }

        public Player(Deck deck)
        {
            _deck = deck;
        }
    }
}
