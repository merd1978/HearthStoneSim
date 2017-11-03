using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class HandZone : Zone<Playable>
    {
	    public HandZone(Controller controller) : base(controller, Zone.HAND)
	    {
		    MaxSize = 10;
	    }

	    public void Add(Card card)
	    {
		    if (Count == MaxSize) return;
		    var element = Playable.FromCard(Controller, card, null, Zone.HAND);
		    Add(element);
	    }
    }
}
