using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Weapon : Playable
    {
        public int AttackDamage
        {
            get => this[GameTag.ATK];
            set => this[GameTag.ATK] = value;
        }

        public int Damage
        {
            get => this[GameTag.DAMAGE];
            set
            {
                //Game.TriggerManager.OnDamageTrigger(this);
                this[GameTag.DAMAGE] = value;
                if (this[GameTag.DURABILITY] <= value)
                {
                    ToBeDestroyed = true;
                }
            }
        }

        public int Durability
	    {
		    get => this[GameTag.DURABILITY];
		    set => this[GameTag.DURABILITY] = value;
	    }

        public bool IsImmune
        {
            get => this[GameTag.IMMUNE] == 1;
            set => this[GameTag.IMMUNE] = value ? 1 : 0;
        }

        public bool Poisonous
        {
            get => Card.Poisonous;
            set => this[GameTag.POISONOUS] = value ? 1 : 0;
        }

        public Weapon(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
    }
}
