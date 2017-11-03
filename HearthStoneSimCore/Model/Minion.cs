using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Minion : Character
    {
        public bool IsHitTest { get; set; } = true;
 
        public bool HasDivineShield
        {
            get => this[GameTag.DIVINE_SHIELD] == 1;
            set => this[GameTag.DIVINE_SHIELD] = value ? 1 : 0;
        }

        public bool HasStealth
        {
            get => this[GameTag.STEALTH] == 1;
            set => this[GameTag.STEALTH] = value ? 1 : 0;
        }

        public bool HasCharge
        {
            get => this[GameTag.CHARGE] == 1;
            set => this[GameTag.CHARGE] = value ? 1 : 0;
        }

	    public bool HasDeathrattle
	    {
		    get => this[GameTag.DEATHRATTLE] == 1;
		    set => this[GameTag.DEATHRATTLE] = value ? 1 : 0;
	    }

	    public bool HasBattleCry
	    {
		    get => this[GameTag.BATTLECRY] != 0;
		    set => this[GameTag.BATTLECRY] = value ? 1 : 0;
	    }

		public bool IsSummoned
        {
            get => this[GameTag.SUMMONED] == 1;
            set => this[GameTag.SUMMONED] = value ? 1 : 0;
        }

        public Minion(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
        
        // Cloning copy constructor
        public Minion(Minion cloneFrom) : base(cloneFrom) { }
    }
}
