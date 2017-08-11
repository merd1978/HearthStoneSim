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

        public void Add(Minion card)
        {
            if (Cards.Count == MaxSize) return;
            card.Zone = Zone.PLAY;
            Cards.Add(card);
        }

        public void Add(Card card)
        {
            if (Cards.Count == MaxSize) return;
            var minion = new Minion(Controller, card, null) {Zone = Zone.PLAY};
            Cards.Add(minion);
        }

        public void Insert(Minion card, int index)
        {
            if (Cards.Count == MaxSize) return;
            card.Zone = Zone.PLAY;
            Cards.Insert(index, card);
        }
    }
}
