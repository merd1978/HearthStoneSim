using System.Collections.Generic;

namespace HearthStoneSimCore.Model
{
    public class Deck
    {
        public List<Card> Cards = new List<Card>();
	    public Controller Controller { get; set; }
        public int StartingCards { get; set; } = 30;

	    public Deck(Controller controller)
	    {
		    Controller = controller;
	    }

        public void Add(Card card)
        {
            Cards.Add(card);
        }
    }
}
