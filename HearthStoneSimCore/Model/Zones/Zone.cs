using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HearthStoneSimCore.Auras;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Exceptions;
using HearthStoneSimCore.Model.Utils;

namespace HearthStoneSimCore.Model.Zones
{
    public abstract class Zone<T> : IZone, IEnumerable<T> where T : Playable
    {
        protected Zone(Controller player)
        {
            Controller = player;
            Game = player.Game;
        }

        /// <summary>
        /// Get the number of entities in this zone.
        /// </summary>
        public abstract int Count { get; }
        public abstract bool IsFull { get; }
        //public abstract List<Playable> ToList();
        public abstract void Add(T playable, int zonePosition = -1);
        public abstract T Remove(T playable);
        public abstract Zone Type { get; }

        void IZone.Add(Playable entity, int zonePosition)
        {
            Add((T)entity, zonePosition);
        }

        Playable IZone.Remove(Playable entity)
        {
            return Remove((T)entity);
        }

        /// <summary>
        /// Gets a value indicating whether this contains entities or not.
        /// </summary>
        /// <value><c>true</c> if this zone is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets a random entity in this zone.
        /// </summary>
        public abstract T Random { get; }

        /// <summary>
        /// Gets the <see cref="Playable"/> with the specified zone position.
        /// </summary>
        /// <value>The <see cref="Playable"/>.</value>
        /// <param name="zonePosition">The zero-based position inside the zone.</param>
        /// <returns></returns>
        public abstract T this[int zonePosition] { get; }

        /// <summary>Gets the game which contains the zone.</summary>
        /// <value><see cref="Model.Game"/></value>
        public Game Game { get; protected set; }

        /// <summary>
        /// Gets the owner of the zone.
        /// </summary>
        /// <value><see cref="Model.Controller"/></value>
        public Controller Controller { get; set; }

        /// <summary>
        /// Moves the specified entity to a new position.
        /// </summary>
        /// <param name="playable">The playable entity.</param>
        /// <param name="zonePosition">The zone position.</param>
        public abstract void MoveTo(Playable playable, int zonePosition = -1);

        /// <summary>
        /// Returns TRUE if at least one of entities
        ///	in this Zone satisfies the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public abstract bool Any(Func<T, bool> predicate);

        //public string Hash(params GameTag[] ignore)
        //{
        //    var str = new StringBuilder();
        //    str.Append("[Z:");
        //    str.Append($"{Type}");
        //    str.Append("][E:");
        //    var list = this.ToList();
        //    if (Type != Zone.PLAY)
        //    {
        //        list = list.OrderBy(p => p.Id).ToList();
        //        Array.Resize(ref ignore, ignore.Length + 1);
        //        ignore[ignore.Length - 1] = GameTag.ZONE_POSITION;
        //    }
        //    list.ForEach(p => str.Append(p.Hash(ignore)));
        //    if (this is PositioningZone<T> pZone)
        //    {
        //        str.Append("[As:");
        //        pZone.Auras.OrderBy(p => p.Owner.Id).ToList().ForEach(p => str.Append(p));
        //        str.Append("]");
        //    }
        //    str.Append("]");
        //    return str.ToString();
        //}

        public override string ToString()
        {
            return $"[ZONE {Type} '{Controller.Name}']";
        }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// This kind of zones never be full.
    /// </summary>
    public abstract class UnlimitedZone : Zone<Playable>
    {
        private readonly List<Playable> _items;

        public override int Count => _items.Count;
        public override bool IsFull => false;

        protected UnlimitedZone(Controller controller) : base(controller)
        {
            _items = new List<Playable>();
        }

        //protected UnlimitedZone(Controller c, UnlimitedZone zone) : base(c)
        //{
        //    var entities = new List<IPlayable>(zone.Count);
        //    IList<IPlayable> src = zone._entities;
        //    for (int i = 0; i < src.Count; ++i)
        //    {
        //        IPlayable copy = src[i].Clone(c);
        //        copy.Zone = this;
        //        entities.Add(copy);
        //    }
        //    _entities = entities;
        //}

        public override Playable this[int zonePosition] => 
            zonePosition >= Count ? throw new IndexOutOfRangeException() : _items[zonePosition];

        public override Playable Random => IsEmpty ? default : _items[ThreadLocalRandom.Next(Count)];

        public override void Add(Playable playable, int zonePosition = -1)
        {
            if (playable.Controller != Controller)
                throw new ZoneException("Can't add an opponent's entity to own Zones");
            MoveTo(playable, zonePosition);
            Game.Log(LogLevel.DEBUG, BlockType.PLAY, "Zone",
                $"Entity '{playable} ({playable.Card.Type})' has been added to zone '{Type}'.");
        }

        public override Playable Remove(Playable playable)
        {
            if (playable.Zone == null || playable.Zone.Type != Type)
                throw new ZoneException("Couldn't remove entity from zone");

            _items.Remove(playable);

            return playable;
        }

        public override void MoveTo(Playable playable, int zonePosition = -1)
        {
            _items.Add(playable);
            playable.Zone = this;
            //if (Game.History)
            //    entity[GameTag.ZONE] = (int)Type;
        }

        public override bool Any(Func<Playable, bool> predicate)
        {
            List<Playable> playable = _items;
            for (int i = 0; i < playable.Count; i++)
                if (predicate(playable[i]))
                    return true;

            return false;
        }

        public override IEnumerator<Playable> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public List<Playable> ToList()
        {
            return _items;
        }
    }

    /// <summary>
    /// Base implementation of zones which have a maximum size.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LimitedZone<T> : Zone<T> where T : Playable
    {
        protected readonly T[] _items;
        protected int _count;
        /// <summary>
        /// Gets the maximum amount of entities this zone can hold.
        /// </summary>
        /// <value>The maximum size.</value>
        public int MaxSize { get; }

        public override bool IsFull => _count == MaxSize;

        public List<T> ToList()
        {
            var lst = new List<T>();
            for (int i = 0; i < _count; i++)
            {
                lst.Add(_items[i]);
            }
            return lst;
        }

        protected LimitedZone(Controller player, int maxSize) : base(player)
        {
            MaxSize = maxSize;
            _items = new T[maxSize];
        }

        public override T this[int zonePosition] => zonePosition >= _count ? throw new IndexOutOfRangeException() : _items[zonePosition];

        public override int Count => _count;

        public override T Random => _count == 0 ? default : _items[ThreadLocalRandom.Next(_count)];

        public int FreeSpace => MaxSize - _count;

        public override void Add(T playable, int zonePosition = -1)
        {
            if (zonePosition > _count)
                throw new ZoneException($"Zoneposition '{zonePosition}' isn't in a valid range.");
            if (playable.Controller != Controller)
                throw new ZoneException("Can't add an opponent's entity to own Zones");

            MoveTo(playable, zonePosition < 0 ? _count : zonePosition);

            Game.Log(LogLevel.DEBUG, BlockType.PLAY, "Zone",
                $"Entity '{playable} ({playable.Card.Type})' has been added to zone '{Type}' in position '{playable.ZonePosition}'.");
        }

        public override void MoveTo(Playable playable, int zonePosition = -1)
        {
            if (playable == null)
                throw new ZoneException();

            if (_count == MaxSize)
                throw new ZoneException($"Can't move {playable} to {this}. The Zone is full.");

            if (zonePosition < 0 || zonePosition == _count)
                _items[_count] = (T) playable;
            else
            {
                T[] items = _items;
                //for (int i = c - 1; i >= zonePosition; --i)
                //	entities[i + 1] = entities[i];
                Array.Copy(items, zonePosition, items, zonePosition + 1, _count - zonePosition);
                items[zonePosition] = (T) playable;
            }

            _count++;

            playable.Zone = this;
            //if (Game.History)
            //    playable[GameTag.ZONE] = (int)Type;
        }

        public override T Remove(T playable)
        {
            //if (entity.Zone == null || entity.Zone.Type != Type)
            if (playable.Zone != this)
                throw new ZoneException("Couldn't remove entity from zone.");

            T[] items = _items;
            int pos;
            for (pos = _count - 1; pos >= 0; --pos)
                if (ReferenceEquals(items[pos], playable)) break;

            if (pos < --_count)
                Array.Copy(items, pos + 1, items, pos, _count - pos);

            playable.Zone = null;

            //playable.ActivatedTrigger?.Remove();

            return playable;
        }

        public override bool Any(Func<T, bool> predicate)
        {
            T[] items = _items;
            for (int i = 0; i < _count; i++)
                if (predicate(items[i]))
                    return true;
            return false;
        }

        public virtual T[] GetAll()
        {
            T[] array = new T[_count];
            Array.Copy(_items, array, array.Length);
            return array;
        }

        public virtual T[] GetAll(Func<T, bool> predicate)
        {
            T[] buffer = new T[_count];
            int i = 0;
            T[] items = _items;
            for (int k = 0; k < buffer.Length; ++k)
            {
                if (!predicate(items[k])) continue;
                buffer[i] = items[k];
                ++i;
            }

            if (i != _count)
            {
                T[] array = new T[i];
                Array.Copy(buffer, array, i);
                return array;
            }
            return buffer;
        }

        //public ReadOnlySpan<T> GetSpan()
        //{
        //    var span = new ReadOnlySpan<T>(_entities);
        //    return span.Slice(0, _count);
        //}

        internal virtual void CopyTo(Array destination, int index)
        {
            Array.Copy(_items, 0, destination, index, _count);
        }

        public void ForEach(Action<T> action)
        {
            T[] entities = _items;
            for (int i = 0; i < _count; ++i)
                action(entities[i]);
        }

        public void ForEach<T2>(Action<T, T2> action, T2 arg2)
        {
            T[] entities = _items;
            for (int i = 0; i < _count; ++i)
                action(entities[i], arg2);
        }

        public void ForEach<T2, T3>(Action<T, T2, T3> action, T2 arg2, T3 arg3)
        {
            T[] entities = _items;
            for (int i = 0; i < _count; ++i)
                action(entities[i], arg2, arg3);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            T[] entities = _items;
            for (int i = 0; i < _count; i++)
                yield return entities[i];
        }
    }

    /// <summary>
    /// Base implementation of zones performing strict recalculation of its containing entities' ZonePosition when any member comes and goes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PositioningZone<T> : LimitedZone<T> where T : Playable
    {
        protected PositioningZone(Controller player, int maxSize) : base(player, maxSize) { }

        public readonly List<Aura> Auras = new List<Aura>();

        private void Reposition(int zonePosition = 0)
        {
            if (zonePosition < 0)
            {
                _items[_count - 1].ZonePosition = _count - 1;
                return;
            }

            T[] entities = _items;
            for (int i = _count - 1; i >= zonePosition; --i)
                entities[i].ZonePosition = i;
        }

        public override void Add(T entity, int zonePosition = -1)
        {
            base.Add(entity, zonePosition);

            Reposition(zonePosition);

            for (int i = Auras.Count - 1; i >= 0; i--)
                Auras[i].EntityAdded(entity);
        }

        public override T Remove(T entity)
        {
            if (entity.Zone != this)
                throw new ZoneException("Couldn't remove entity from zone.");

            int pos = entity.ZonePosition;
            int count = _count;
            T[] entities = _items;

            if (pos < --count)
                Array.Copy(entities, pos + 1, entities, pos, count - pos);

            _count = count;

            Reposition(pos);

            entity.Zone = null;

            entity.ActivatedTrigger?.Remove();

            for (int i = Auras.Count - 1; i >= 0; i--)
                Auras[i].EntityRemoved(entity);

            return entity;
        }

        /// <summary>
        /// Swaps the positions of both entities in this zone.
        /// Both entities must be contained by this zone.
        /// </summary>
        /// <param name="oldEntity">The one entity.</param>
        /// <param name="newEntity">The other entity.</param>
        public void Swap(T oldEntity, T newEntity)
        {
            if (oldEntity.Zone.Type != newEntity.Zone.Type)
                throw new ZoneException("Swap not possible because of zone mismatch");

            int oldPos = oldEntity.ZonePosition;
            int newPos = newEntity.ZonePosition;
            newEntity.ZonePosition = oldPos;
            oldEntity.ZonePosition = newPos;
            _items[newPos] = oldEntity;
            _items[oldPos] = newEntity;
        }

        /// <summary>
        /// Replaces an entity in the given position internally. (i.e. not create any history packets)
        /// </summary>
        internal void ChangeEntity(T oldEntity, T newEntity)
        {
            int pos = oldEntity.ZonePosition;
            _items[pos] = newEntity;
            newEntity.ZonePosition = pos;
            newEntity.Zone = this;
        }
    }
}
