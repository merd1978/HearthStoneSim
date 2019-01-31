using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Hero : Character
    {
        public Weapon Weapon { get; set; }

        public int TotalAttackDamage => AttackDamage + (Weapon?.AttackDamage ?? 0);

        public int Armor
        {
            get => this[GameTag.ARMOR];
            set => this[GameTag.ARMOR] = value;
        }

        public int Fatigue
        {
            get => this[GameTag.FATIGUE];
            set => this[GameTag.FATIGUE] = value;
        }

        public int DamageTakenThisTurn { get; set; }

        public Hero(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
    }
}
