using System.Collections.Generic;
using System.Linq;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public abstract class Character : Playable
    {

        public bool TakeDamage(Playable source, int damage)
        {
            var hero = this as Hero;
            var minion = this as Minion;

            var fatigue = hero != null && this == source;

            if (fatigue)
            {
                hero.Fatigue = damage;
            }

            if (minion != null && minion.HasDivineShield)
            {
                Game.Log(LogLevel.INFO, BlockType.ATTACK, "Character", $"{this} divine shield absorbed incoming damage.");
                minion.HasDivineShield = false;
                return false;
            }

            if (minion != null && minion.IsImmune || hero != null && hero.IsImmune)
            {
                Game.Log(LogLevel.INFO, BlockType.ATTACK, "Character", $"{this} is immune.");
                return false;
            }

            // remove armor first from hero ....
            if (hero != null && hero.Armor > 0)
            {
                if (hero.Armor < damage)
                {
                    damage = damage - hero.Armor;
                    hero.Armor = 0;
                }
                else
                {
                    hero.Armor = hero.Armor - damage;
                    damage = 0;
                }
            }

            // added pre damage to be able to interact
            PreDamage = damage;

            // final damage is beeing accumulated
            Damage += PreDamage;

			Game.Log(LogLevel.INFO, BlockType.ATTACK, "Character", $"{this} took damage for {PreDamage}({damage}). {(fatigue ? "(fatigue)" : "")}");

			// check if there was damage done
			var tookDamage = PreDamage > 0;

            if (tookDamage)
            {
                IsDamaged = true;
                //Enchantment enchantment = Enchants.First();
                //if (enchant != null && enchant.Trigger == TriggerType.OnDamage) Game.TaskQueue.Enqueue(enchant.Effect, this);
            }

            // reset predamage - moved to gui
            // PreDamage = 0;

            return tookDamage;
        }

        public bool IsDamaged { get; set; } = false;
        public bool IsDead { get; set; } = false;

        public int AttackDamage
        {
            get => this[GameTag.ATK];
            set => this[GameTag.ATK] = value;
        }

        public int PreDamage
        {
            get => this[GameTag.PREDAMAGE];
            set => this[GameTag.PREDAMAGE] = value;
        }

        public int Damage
        {
            get => this[GameTag.DAMAGE];
            set => this[GameTag.DAMAGE] = value;
        }

        public int Health
        {
            get => this[GameTag.HEALTH] - this[GameTag.DAMAGE];
            set
            {
                // Absolute change in health, removes damage dealt (eg. Equality)
                this[GameTag.HEALTH] = value;
                this[GameTag.DAMAGE] = 0;
            }
        }

		public bool IsAttacking
        {
            get => this[GameTag.ATTACKING] == 1;
            set => this[GameTag.ATTACKING] = value ? 1 : 0;
        }

        public bool IsDefending
        {
            get => this[GameTag.DEFENDING] == 1;
            set => this[GameTag.DEFENDING] = value ? 1 : 0;
        }

        public int ProposedAttacker
        {
            get => this[GameTag.PROPOSED_ATTACKER];
            set => this[GameTag.PROPOSED_ATTACKER] = value;
        }

        public int ProposedDefender
        {
            get => this[GameTag.PROPOSED_DEFENDER];
            set => this[GameTag.PROPOSED_DEFENDER] = value;
        }

        public bool IsImmune
        {
	        get => this[GameTag.IMMUNE] == 1;
	        set => this[GameTag.IMMUNE] = value ? 1 : 0;
        }

        public bool IsFrozen
        {
            get => this[GameTag.FROZEN] == 1;
            set => this[GameTag.FROZEN] = value ? 1 : 0;
        }

        public bool IsExhausted
        {
            get => this[GameTag.EXHAUSTED] == 1;
            set => this[GameTag.EXHAUSTED] = value ? 1 : 0;
        }

        public bool Freeze
        {
            get => this[GameTag.FREEZE] == 1;
            set => this[GameTag.FREEZE] = value ? 1 : 0;
        }

        public int NumAttacksThisTurn
        {
            get => this[GameTag.NUM_ATTACKS_THIS_TURN];
            set => this[GameTag.NUM_ATTACKS_THIS_TURN] = value;
        }

        public virtual bool HasWindfury
        {
            get => this[GameTag.WINDFURY] == 1;
            set => this[GameTag.WINDFURY] = value ? 1 : 0;
        }

	    public bool HasTaunt
	    {
		    get => this[GameTag.TAUNT] == 1;
		    set => this[GameTag.TAUNT] = value ? 1 : 0;
	    }

		protected Character(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
        
        // Cloning copy constructor
        protected Character(Character cloneFrom) : base(cloneFrom) { }
    }
}
