using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Hand
    {
        public List<Playable> Cards = new List<Playable>();
	    public Controller Controller { get; set; }
        public int MaxSize { get; set; } = 10;

	    public Hand(Controller controller)
	    {
		    Controller = controller;
	    }

        public void Add(Card card)
        {
            if (Cards.Count == MaxSize) return;
            var c = Playable.FromCard(Controller, card, null, Zone.HAND);
            Cards.Add(c);
        }
    }
}
