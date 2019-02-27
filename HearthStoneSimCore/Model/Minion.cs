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

        public bool Poisonous
        {
            get => this[GameTag.POISONOUS] == 1;
            set => this[GameTag.POISONOUS] = value ? 1 : 0;
        }

        public override bool ToBeDestroyed
        {
            get => base.ToBeDestroyed;

            set
            {
                if (value == base.ToBeDestroyed)
                    return;
                base.ToBeDestroyed = value;
                if (value)
                    Game.DeadMinions.Add(this);
            }
        }

        public int LastBoardPosition
        {
            get => this[GameTag.TAG_LAST_KNOWN_POSITION_ON_BOARD];
            set => this[GameTag.TAG_LAST_KNOWN_POSITION_ON_BOARD] = value;
        }

        public Minion(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
        
        // Cloning copy constructor
        public Minion(Minion cloneFrom) : base(cloneFrom) { }
    }
}
