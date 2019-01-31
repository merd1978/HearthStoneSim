using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Factory
{
    public static class CardFactory
    {
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

        public static Minion MinionFromName(Controller controller, string cardName)
        {
            return FromCard(controller, Cards.FromName(cardName)) as Minion;
        }

        public static Playable PlayableFromName(Controller controller, string cardName)
        {
            return FromCard(controller, Cards.FromName(cardName));
        }
    }
}