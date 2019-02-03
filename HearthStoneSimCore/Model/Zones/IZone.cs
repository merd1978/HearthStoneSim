using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
    /// <summary>
    /// Interface of an abstract area where <see cref="IPlayable"/> objects
    /// reside.
    /// A zone is owned by a controller and isn't shared.
    /// </summary>
    public interface IZone
    {
        /// <summary>
        /// Gets the kind of zone.
        /// </summary>
        /// <value><see cref="Zone"/></value>
        Zone Type { get; }

        /// <summary>
        /// Gets the amount of entities residing in this zone.
        /// </summary>
        /// <value>The count of entities.</value>
        int Count { get; }

        /// <summary>
        /// Gets a value indicating whether this zone is full.
        /// </summary>
        /// <value><c>true</c> if this zone reach the maximum amount of entities; otherwise, <c>false</c>.</value>
        bool IsFull { get; }

        /// <summary>
        /// Return <see cref="List{T}"/> that contains all entities in this zone.
        /// </summary>
        /// <value>The set of <see cref="Playable"/>.</value>
        //List<Playable> ToList();

        /// <summary>
        /// Adds the specified entity into this zone, at the given position.
        /// </summary>
        /// <param name="item">The entity.</param>
        /// <param name="zonePosition">The zone position.</param>
        /// <returns>The entity</returns>
        void Add(Playable item, int zonePosition = -1);

        /// <summary>
        /// Removes the specified entity from this zone.
        /// </summary>
        /// <param name="item">The entity.</param>
        /// <returns>The entity.</returns>
        Playable Remove(Playable item);

        /// <summary>
        /// Returns a string which contains a hash unique to this zone object.
        /// </summary>
        /// <param name="ignore">The <see cref="GameTag"/>s to ignore during hash creation.</param>
        /// <returns></returns>
        //string Hash(params GameTag[] ignore);
    }
}