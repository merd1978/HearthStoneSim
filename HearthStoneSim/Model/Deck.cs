using System.Collections.Generic;

namespace HearthStoneSim.Model
{
    public class Deck
    {
        public List<Card> Cards = new List<Card>();
        public int StartingCards { get; set; } = 30;

        public void Add(Card card)
        {
            _cards.Add(card);
        }
    }
}
