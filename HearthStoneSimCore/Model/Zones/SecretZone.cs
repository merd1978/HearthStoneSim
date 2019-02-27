using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
	public class SecretZone : LimitedZone<Spell>
    {
        public override Zone Type => Zone.SECRET;

        public SecretZone(Controller controller, int maxSize = 5) :base(controller, maxSize)
		{
			Controller = controller;
		}
    }
}
