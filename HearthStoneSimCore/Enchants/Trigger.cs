namespace HearthStoneSimCore.Enchants
{
    public interface IPlayable
    {
        /// <summary>
        /// Remove this object from the Game and unsubscribe from the related event.
        /// </summary>
        void Remove();
    }

    public class Trigger : IPlayable
    {
        /// <summary>
        /// Remove this object from the Game and unsubscribe from the related event.
        /// </summary>
        public virtual void Remove()
        {
        }
    }
}
