using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class DeckZone : LimitedZone<Playable>
    {
        public const int StartingCards = 30;

        public override Zone Type => Zone.DECK;

        public Playable TopCard => _items[_count - 1];

        public DeckZone(Controller controller, int maxSize = 60) : base(controller, maxSize)
	    {
	    }

        public override void Add(Playable entity, int zonePosition = -1)
        {
            base.Add(entity, zonePosition);
        }
    }
}
