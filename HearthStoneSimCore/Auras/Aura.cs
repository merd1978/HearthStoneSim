
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Auras
{
    public class Aura
    {
        protected bool On = true;

        /// <summary>
        /// Notices this aura instance that the given entity is added to the corresponding zone.
        /// </summary>
        public void EntityAdded(Playable playable)
        {
            if (!On)
                return;

            //var instruction = new AuraUpdateInstruction(playable, Instruction.Add);

            //if (!AuraUpdateInstructionsQueue.Contains(in instruction))
            //    AuraUpdateInstructionsQueue.Enqueue(instruction, 2);
        }

        /// <summary>
        /// Notices this aura instance that the given entity is removed from the corresponding zone.
        /// </summary>
        public void EntityRemoved(Playable playable)
        {
            if (!On)
                return;
            //if (playable == Owner)
            //    return;

            //AuraUpdateInstructionsQueue.Enqueue(new AuraUpdateInstruction(playable, Instruction.Remove), 1);
        }
    }
}
