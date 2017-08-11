using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public abstract class Playable : Core
    {
	    public Controller Controller { get; set; }

		protected Playable(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(card, tags)
		{
			Controller = controller;
		}

        public static Playable FromCard(Controller controller, Card card, Dictionary<GameTag, int> tags = null, Zone zone = Zone.INVALID)
        {
            tags = tags ?? new Dictionary<GameTag, int>();
            tags[GameTag.ZONE] = (int)zone;
            Playable result = null;
            switch (card.Type)
            {
                case CardType.MINION:
                    result = new Minion(controller, card, tags);
                    break;
            }
            return result;
        }

        public static Playable FromName(Controller controller, string cardName)
        {
            return FromCard(controller, Cards.FromName(cardName));
        }
        
        // Cloning copy constructor
        protected Playable(Playable cloneFrom) : base(cloneFrom) { }
    }
}
