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

        public Minion(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
        
        // Cloning copy constructor
        public Minion(Minion cloneFrom) : base(cloneFrom) { }
    }
}
