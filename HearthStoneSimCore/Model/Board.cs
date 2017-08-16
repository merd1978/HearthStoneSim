using HearthStoneSimCore.Enums;
using System.Collections.Generic;

namespace HearthStoneSimCore.Model
{
    public class Board
    {
        public List<Minion> Cards = new List<Minion>();
	    public Controller Controller { get; set; }
        public int MaxSize { get; set; } = 7;

	    public Board(Controller controller)
	    {
            Controller = controller;
	    }

        public void Add(Minion card, int zonePosition = -1)
        {
            if (Cards.Count == MaxSize) return;
            card.Zone = Zone.PLAY;
	        card.ZonePosition = zonePosition < 0 ? Cards.Count : zonePosition;
            Cards.Insert(zonePosition, card);
	        UpdateZonePositions(zonePosition);
		}

        public void Add(Card card)
        {
            if (Cards.Count == MaxSize) return;
            var minion = new Minion(Controller, card, null) {Zone = Zone.PLAY};
            Cards.Add(minion);
        }

	    private void UpdateZonePositions(int zonePosition = 0)
	    {
		    for (var i = zonePosition; i < Cards.Count; i++)
		    {
			    Cards[i].ZonePosition = i;
		    }
	    }
	}
}
