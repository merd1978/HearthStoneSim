using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Actions
{
	public static partial class GameAction
	{
		public static void SummonBlock(Controller player, Playable source, Minion minion)
		{
			if (!minion.HasCharge) minion.IsExhausted = true;

            player.Game.Log(LogLevel.INFO, BlockType.PLAY, "SummonBlock", $"Summon Minion {minion} to Board of {player.Name}.");

            /* Unlike when directly summoning minions, the placement of minions summoned by summon effects is not controlled by the player.
             * As a rule, summon effects tend to place minions to the right. In the case of minion summon effects such as Violet Teacher's
             * triggered effect, the minion is placed immediately to the right of the summoning minion; while for all other sources of summon
             * effects, including Hero Powers such as Totemic Call and spells such as Animal Companion, the minion is placed at the far right
             * of the battlefield.
             * There are a few notable exceptions to this behaviour: Summoning Onyxia will evenly distribute Whelps to Onyxia's left and right.
             * Dr. Boom will similarly attempt to place a Boom Bot to each side of himself. https://hearthstone.gamepedia.com/Summon */
		    int zonePosition = source.Zone.Type == Zone.PLAY ? source.ZonePosition + 1: -1;
		    player.Board.Add(minion, zonePosition);

			// add summon block show entity 
			//if (c.Game.History)
			//	c.Game.PowerHistory.Add(PowerHistoryBuilder.ShowEntity(minion));

			minion.IsSummoned = true;
			player.Game.DeathProcessingAndAuraUpdate();
		}
	}
}
