using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class GraveyardZone : UnlimitedZone
    {
        public GraveyardZone(Controller controller) : base(controller)
        {
        }

        public override Zone Type => Zone.GRAVEYARD;
    }
}
