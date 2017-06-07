using System.Collections.Generic;

namespace HearthStoneSim.Model
{
    public class Hand
    {
        public List<Card> Cards = new List<Card>();
        public int MaxSize { get; set; } = 10;

        public void Add(Card card)
        {
            Cards.Add(card);
        }
    }
}
