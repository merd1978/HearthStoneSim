using HearthStoneSimCore.Actions;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks.SimpleTasks
{
    public class SummonTask : ISimpleTask
    {
	    public Playable Source { get; set; }
	    public Controller Player { get; set; }
	    public Card Card { get; set; }

		public SummonTask(string cardId)
	    {
		    Card = Cards.FromId(cardId);
	    }

	    public void Process()
	    {
			Minion summonMinion = (Minion)Playable.FromCard(Player, Card);
		    Summon.SummonBlock(Player, Source, summonMinion);
	    }
	}
}
