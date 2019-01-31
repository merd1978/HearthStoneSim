using System.Diagnostics;
using HearthStoneSimCore.Actions;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks.PlayerTasks
{
    public class PlayCardTask : IPlayerTask
    {
        public Controller Player { get; }
        public Playable Source { get; set; }
        public Character Target { get; set; }
        public int ZonePosition { get; set; } = -1;
        public int ChooseOne { get; set; }

        public PlayCardTask(Controller player, Playable source, Character target = null, int zonePosition = -1, int chooseOne = 0)
        {
            Player = player;
            Source = source;
            Target = target;
	        ZonePosition = zonePosition;
	        ChooseOne = chooseOne;
		}

        public void Process()
        {
            GameAction.PlayCardBlock(Player, Source, Target, ZonePosition, ChooseOne);
        }
    }
}
