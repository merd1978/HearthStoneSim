using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class HandZone : PositioningZone<Playable>
    {
        public override Zone Type => Zone.HAND;

        public HandZone(Controller controller, int maxSize = 10) : base(controller, maxSize)
	    {
	    }

        public override void Add(Playable entity, int zonePosition = -1)
        {
            base.Add(entity, zonePosition);

            //if (entity.Power?.Aura is AdaptiveCostEffect e)
            //    e.Activate((Playable)entity);
            //entity.Power?.Trigger?.Activate(entity, TriggerActivation.HAND);

            //Game.TriggerManager.OnZoneTrigger(entity);
        }
    }
}
