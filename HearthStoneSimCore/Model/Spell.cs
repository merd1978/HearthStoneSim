using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Spell : Playable
    {
        public bool ReceveivesDoubleSpellDamage => Card.ReceivesDoubleSpelldamageBonus;

        public Spell(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
	    {
	    }

	    public Spell(Playable cloneFrom) : base(cloneFrom)
	    {
	    }
    }
}
