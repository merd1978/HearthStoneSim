using System.Collections.Generic;
using System.Linq;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    public class BoardZone : PositioningZone<Minion>
    {
	    public BoardZone(Controller controller, int maxSize = 7) : base(controller, maxSize)
	    {
	    }

        public override Zone Type => Zone.PLAY;

        public override void Add(Minion entity, int zonePosition = -1)
        {
            base.Add(entity, zonePosition);

            //if (entity.Controller == Game.CurrentPlayer)
            //{
            //    if (!entity.HasCharge)
            //    {
            //        if (entity.IsRush)
            //        {
            //            entity.AttackableByRush = true;
            //            Game.RushMinions.Add(entity.Id);
            //        }
            //        else
            //            entity.IsExhausted = true;
            //    }
            //}

            //entity.OrderOfPlay = Game.NextOop;

            //ActivateAura(entity);

            //for (int i = AdjacentAuras.Count - 1; i >= 0; i--)
            //    AdjacentAuras[i].BoardChanged = true;

            //Game.TriggerManager.OnZoneTrigger(entity);

            //if (entity.Card.Untouchable)
            //{
            //    ++_untouchableCount;
            //    _hasUntouchables = true;
            //}
        }
    }
}
