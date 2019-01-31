using System;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Actions
{
    public static partial class GameAction
    {
        public static bool AddHandPhase(Controller player, Playable playable)
        {
            if (player.HandZone.IsFull)
            {
                player.Game.Log(LogLevel.INFO, BlockType.PLAY, "AddHandPhase",
                    $"Hand ist full. Card {playable} drawn is burnt to graveyard.");
                player.GraveyardZone.Add(playable);
                return false;
            }

            // add draw block show entity 
            //if (c.Game.History && playable != null)
            //    c.Game.PowerHistory.Add(PowerHistoryBuilder.ShowEntity(playable));

            player.Game.Log(LogLevel.INFO, BlockType.PLAY, "AddHandPhase", $"adding to hand {playable}.");
            player.HandZone.Add(playable);

            return true;
        }

        public static int DamageCharFunc(Playable source, Character target, int amount, bool applySpellDmg)
        {
            if (applySpellDmg)
            {
                amount += ((Spell)source).ReceveivesDoubleSpellDamage
                    ? source.Controller.CurrentSpellPower * 2
                    : source.Controller.CurrentSpellPower;
                //if (source.Controller.ControllerAuraEffects[GameTag.SPELLPOWER_DOUBLE] > 0)
                //    amount *= (int)Math.Pow(2, source.Controller.ControllerAuraEffects[GameTag.SPELLPOWER_DOUBLE]);
            }
            //else if (source is HeroPower)
            //{
            //    // TODO: Consider this part only when TGT or Rumble is loaded
            //    // amount += source.Controller.Hero.HeroPowerDamage; 
            //    if (source.Controller.ControllerAuraEffects[GameTag.HERO_POWER_DOUBLE] > 0)
            //        amount *= (int)Math.Pow(2, source.Controller.ControllerAuraEffects[GameTag.HERO_POWER_DOUBLE]);
            //}
            return target.TakeDamage(source, amount);
        }
    }
}