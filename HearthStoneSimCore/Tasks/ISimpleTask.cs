using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Tasks
{
	public interface ISimpleTask
	{
		Controller Player { get; set; }
		Playable Source { get; set; }
		void Process();
	}
}
