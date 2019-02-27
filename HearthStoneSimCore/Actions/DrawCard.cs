using System;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Model.Factory;
using HearthStoneSimCore.Tasks;

namespace HearthStoneSimCore.Actions
{
    public static partial class GameAction
    {
        public static Playable DrawCard(Controller player, Card card)
        {
            var playable = CardFactory.FromCard(player, card);
            //c.NumCardsDrawnThisTurn++;
            AddHandPhase(player, playable);
            return playable;
        }

        public static Playable DrawBlock(Controller c, Playable cardToDraw = null)
        {
            if (!PreDrawPhase(c))
                return null;

            Playable playable = DrawPhase(c, cardToDraw);
            //c.NumCardsToDraw--; 

            if (AddHandPhase(c, playable))
            {
                // DrawTrigger vs TOPDECK ?? not sure which one is first

                //if (cardToDraw == null)
                //{
                //    c.Game.TaskQueue.StartEvent();
                //    c.Game.TriggerManager.OnDrawTrigger(playable);
                //    c.Game.ProcessTasks();
                //    c.Game.TaskQueue.EndEvent();
                //}

                //ISimpleTask clone = playable.Power?.TopdeckTask?.Clone();
                //if (clone != null)
                //{
                //    clone.Game = c.Game;
                //    clone.Controller = c;
                //    clone.Source = playable;

                //    if (c.Game.History)
                //    {
                //        // TODO: triggerkeyword: TOPDECK
                //        c.Game.PowerHistory.Add(
                //            PowerHistoryBuilder.BlockStart(BlockType.TRIGGER, playable.Id, "", 0, 0));
                //    }

                //    c.SetasideZone.Add(c.HandZone.Remove(playable));

                //    c.Game.Log(LogLevel.INFO, BlockType.TRIGGER, "TOPDECK",
                //        !c.Game.Logging ? "" : $"{playable}'s TOPDECK effect is activated.");
                //    clone.Process();
                //    if (c.Game.History)
                //        c.Game.PowerHistory.Add(
                //            PowerHistoryBuilder.BlockEnd());
                //}
            }

            return playable;
        }

        private static bool PreDrawPhase(Controller c)
        {
            if (c.Deck.IsEmpty)
            {
                int fatigueDamage = c.Hero.Fatigue == 0 ? 1 : c.Hero.Fatigue + 1;
                DamageCharFunc(c.Hero, c.Hero, fatigueDamage, false);
                return false;
            }
            return true;
        }

        private static Playable DrawPhase(Controller c, Playable cardToDraw)
        {
            Playable playable = c.Deck.Remove(cardToDraw ?? c.Deck.TopCard);

            c.Game.Log(LogLevel.INFO, BlockType.PLAY, "DrawPhase", $"{c.Name} draws {playable}");

            c.NumCardsDrawnThisTurn++;
            c.LastCardDrawn = playable.Id;

            return playable;
        }
    }
}