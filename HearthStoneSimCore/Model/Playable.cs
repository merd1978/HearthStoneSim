using System.Collections.Generic;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public abstract class Playable : Targeting
    {
	    public List<Enchantment> Enchantment { get; } = new List<Enchantment>();

        public Race Race
        {
            get => (Race)this[GameTag.CARDRACE];
            set => this[GameTag.CARDRACE] = (int)value;
        }

        /// <summary> Gets or sets the entity ID target.</summary>
        /// <value><see cref="Core.Id"/></value>
        public int CardTarget
		{
			get => this[GameTag.CARD_TARGET];
			set => this[GameTag.CARD_TARGET] = value;
		}

		/// <summary>
		/// Playable is overloading mana.
		/// </summary>
		public int Overload
        {
            get => this[GameTag.OVERLOAD];
            set => this[GameTag.OVERLOAD] = value;
        }


        protected Playable(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
		{
			Controller = controller;
			Enchantment.Add(Card.Enchantment);
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
                case CardType.SPELL:
                    result = new Spell(controller, card, tags);
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
