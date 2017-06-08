using System.Collections.Generic;
using HearthStoneSim.Model.Enums;
using HearthStoneSim.Model.GameCore;

namespace HearthStoneSim.Model
{
    public class Hand
    {
        public List<Core> Cards = new List<Core>();
        public int MaxSize { get; set; } = 10;

        public void Add(Card card)
        {
           var c = new Core(card) {Zone = Zone.HAND};
           Cards.Add(c);
        }
    }
}
