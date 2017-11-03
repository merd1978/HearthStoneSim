using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class BoardZone : Zone<Minion>
    {
	    public BoardZone(Controller controller) : base(controller, Zone.PLAY)
	    {
		    MaxSize = 7;
	    }

	    public void Add(Card card)
	    {
		    if (Count == MaxSize) return;
		    var minion = new Minion(Controller, card, null) { Zone = Zone.PLAY };
		    Add(minion);
	    }
	}
}
