using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks
{
    public interface ISimpleTask
    {
    }

    public abstract class SimpleTask : ISimpleTask
    {
        public abstract bool Process(Controller ccontroller, Playable source, Playable target);
    }
}
