using System.Collections;
using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model.Zones
{
	public abstract class Zone<T> : IEnumerable<T> where T : Playable
	{
		public readonly List<T> Elements = new List<T>();
		public Controller Controller { get; set; }
		public int MaxSize { get; set; }
		private readonly Zone _myZone;

		public int Count => Elements.Count;
		public bool IsFull => Elements.Count == MaxSize;

		/// <summary>
		/// Returns true if this zone has a dragon
		/// </summary>
		public bool HasDragon
		{
			get
			{
				foreach (var c in Elements)
				{
					if (c.Race == Race.DRAGON) return true;
				}
				return false;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected Zone(Controller controller, Zone myZone)
		{
			Controller = controller;
			_myZone = myZone;
		}

		public void Add(T element, int zonePosition = -1)
		{
			if (Elements.Count == MaxSize) return;
			element.Zone = _myZone;
            zonePosition = zonePosition < 0 ? Elements.Count : zonePosition;
			Elements.Insert(zonePosition, element);
			UpdateZonePositions(zonePosition);
		}

		public void Remove(T element)
		{
			Elements.Remove(element);
			UpdateZonePositions(element.ZonePosition);
			element.Zone = Zone.INVALID;
		}

		private void UpdateZonePositions(int zonePosition = 0)
		{
			for (var i = zonePosition; i < Elements.Count; i++)
			{
				Elements[i].ZonePosition = i;
			}
		}
	}
}
