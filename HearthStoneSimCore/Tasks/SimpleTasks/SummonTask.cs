using HearthStoneSimCore.Actions;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Model.Factory;

namespace HearthStoneSimCore.Tasks.SimpleTasks
{
    public class SummonTask : ISimpleTask
    {
	    public Playable Source { get; set; }
	    public Controller Player { get; set; }
	    public Card Card { get; set; }

		public SummonTask(string cardId)
	    {
		    Card = Cards.FromCardId(cardId);
	    }

	    public void Process()
	    {
            Minion summonMinion = (Minion)CardFactory.FromCard(Player, Card);
            GameAction.SummonBlock(Player, Source, summonMinion);
	    }
	}
}
