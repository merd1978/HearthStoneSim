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

        public Weapon(Controller controller, Card card, Dictionary<GameTag, int> tags) : base(controller, card, tags)
        {
        }
    }
}
