using System.Collections.Generic;
using System.Linq;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks
{
    public class TaskQueue
    {
        public List<ISimpleTask> TaskList = new List<ISimpleTask>();
        public Game Game { get; set; }
	    public int Count => TaskList.Count;

		public TaskQueue(Game game)
        {
            Game = game;
        }

	    /// <summary>
	    /// Enqueue tasks that will be proccess by the DeathAndAuraProcessing.
	    /// IMPORTANT: only enqueue cloned tasks ....
	    /// </summary>
	    /// <param name="task"></param>
	    /// <param name="source"></param>
	    public void Enqueue(ISimpleTask task, Playable source)
	    {
		    task.Player = source.Controller;
			task.Source = source;
			TaskList.Add(task);
        }

        public void Process()
        {
            //CurrentTask = TaskList.OrderBy(p => p.Source.OrderOfPlay).First();
	        ISimpleTask currentTask = TaskList.First();
            TaskList.Remove(currentTask);
            Game.Log(LogLevel.VERBOSE, BlockType.TRIGGER, "TaskQueue", $"LazyTask[{currentTask.Source}]: '{currentTask.GetType().Name}' is processed!" +
                                                                       $"'{currentTask.Source.Card.Text?.Replace("\n", " ")}'");


            // power block
            //if (Game.History)
            //    Game.PowerHistory.Add(PowerHistoryBuilder.BlockStart(BlockType.POWER, CurrentTask.Source.Id, "", -1, CurrentTask.Target?.Id ?? 0));

            currentTask.Process();

            //if (Game.History)
            //    Game.PowerHistory.Add(PowerHistoryBuilder.BlockEnd());
        }
    }
}
