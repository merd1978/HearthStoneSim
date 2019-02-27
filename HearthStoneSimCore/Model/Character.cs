using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public abstract class Character : Playable
    {

        /// <summary>
        /// Character is dead or destroyed.
        /// </summary>
        public bool IsDead => Health <= 0 || ToBeDestroyed;

        public int AttackDamage
        {
            get => this[GameTag.ATK];
            set => this[GameTag.ATK] = value;
        }

        public bool IsDamaged => PreDamage > 0;
        /// <summary>
        /// The amount of damage this character is about to take.
        /// </summary>
        public int PreDamage
        {
            get => this[GameTag.PREDAMAGE];
            set => this[GameTag.PREDAMAGE] = value;
        }

        private int _damage;

        public int Damage
        {
            get => _damage;
            set
            {
                if (Health <= value)
                    ToBeDestroyed = true;
                this[GameTag.DAMAGE] = value;
                _damage = value;
            }
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

        public bool HasStealth
        {
            get => this[GameTag.STEALTH] == 1;
            set => this[GameTag.STEALTH] = value ? 1 : 0;
        }

        public int TakeDamage(Playable source, int damage)
        {
            var hero = this as Hero;
            var minion = this as Minion;

            if (minion != null && minion.Zone.Type != Enums.Zone.PLAY)
                return 0;

            bool fatigue = hero != null && this == source;

            if (fatigue)
                hero.Fatigue = damage;

            if (minion != null && minion.HasDivineShield)
            {
                Game.Log(LogLevel.INFO, BlockType.ATTACK, "Character", $"{this} divine shield absorbed incoming damage.");
                minion.HasDivineShield = false;
                return 0;
            }

            int armor = hero?.Armor ?? 0;

            int amount = hero == null ? damage : armor < damage ? damage - armor : 0;

            // Damage event is created
            // Collect all the tasks and sort them by order of play
            // Death phase and aura update are not emerge here

            // place event related data
            //Game.TaskQueue.StartEvent();
            //EventMetaData temp = game.CurrentEventData;
            //game.CurrentEventData = new EventMetaData(source, this, amount);

            // added pre damage
            //if (_history)
            PreDamage = amount;

            // Predamage triggers (e.g. Ice Block)
            //if (PreDamageTrigger != null)
            //{
            //    PreDamageTrigger.Invoke(this);
            //    game.ProcessTasks();
            //    amount = game.CurrentEventData.EventNumber;
            //    if (amount == 0 && armor == 0)
            //    {
            //        if (_history)
            //            PreDamage = 0;
            //        return 0;
            //    }
            //}
            //if (IsImmune)
            //{
            //    game.TaskQueue.EndEvent();
            //    game.CurrentEventData = temp;

            //    game.Log(LogLevel.INFO, BlockType.ACTION, "Character", !game.Logging ? "" : $"{this} is immune.");
            //    if (_history)
            //        PreDamage = 0;
            //    return 0;
            //}

            // reset predamage - moved to gui
            //if (_history)
            //    PreDamage = 0;

            // remove armor first from hero ....
            if (armor > 0 && hero != null)
                hero.Armor = armor < damage ? 0 : armor - damage;

            // final damage is beeing accumulated
            Damage += amount;

            Game.Log(LogLevel.INFO, BlockType.ATTACK,
                "Character", $"{this} took damage for {amount}({damage}). {(fatigue ? "(fatigue)" : "")}");

            //LastAffectedBy = source.Id;	TODO

            // on-damage triggers
            //TakeDamageTrigger?.Invoke(this);
            //game.TriggerManager.OnDamageTrigger(this);
            //game.TriggerManager.OnDealDamageTrigger(source);
            //game.ProcessTasks();
            //game.TaskQueue.EndEvent();
            //game.CurrentEventData = temp;

            // Check if the source is lifesteal
            //if (source.HasLifeSteal && !_lifestealChecker)
            //{
            //    if (_history)
            //        game.PowerHistory.Add(PowerHistoryBuilder.BlockStart(BlockType.TRIGGER, source.Id, source.Card.Id, -1, 0)); // TriggerKeyword=LIFESTEAL
            //    game.Log(LogLevel.VERBOSE, BlockType.ATTACK, "TakeDamage", !_logging ? "" : $"lifesteal source {source} has damaged target for {amount}.");
            //    source.Controller.Hero.TakeHeal(source, amount);
            //    if (_history)
            //        game.PowerHistory.Add(new PowerHistoryBlockEnd());

            //    if (source.Controller.Hero.ToBeDestroyed && source.Controller.Hero.Health > 0)
            //        source.Controller.Hero.ToBeDestroyed = false;
            //}

            if (hero != null)
                hero.DamageTakenThisTurn += amount;

            return amount;
        }

        protected Character(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
        
        // Cloning copy constructor
        protected Character(Character cloneFrom) : base(cloneFrom) { }
    }
}
