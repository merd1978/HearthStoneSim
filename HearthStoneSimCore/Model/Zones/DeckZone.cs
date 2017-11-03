using System;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class DeckZone : Zone<Playable>
    {
        public int StartingCards { get; set; } = 30;

	    public DeckZone(Controller controller) : base(controller, Zone.DECK)
	    {
		    MaxSize = Int32.MaxValue;
	    }

	    public void Add(Card card)
	    {
		    if (Count == MaxSize) return;
		    var element = Playable.FromCard(Controller, card, null, Zone.DECK);
		    Add(element);
	    }
	}
}
