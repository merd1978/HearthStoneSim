using HearthStoneSimCore.Actions;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks.PlayerTasks
{
    public class MinionAttackTask : IPlayerTask
    {
        public Controller Player { get; }
        public Minion Source { get; set; }
        public Character Target { get; set; }

        public MinionAttackTask(Controller player, Minion source, Character target)
        {
            Player = player;
            Source = source;
            Target = target;
        }

        public void Process()
        {
            GameAction.AttackBlock(Player, Source, Target);
        }
    }
}
