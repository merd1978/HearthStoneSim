using System;

namespace HearthStoneSim.Model
{
	public struct TagValue : IEquatable<TagValue>
	{
		public bool HasValue { get; private set; }
		public bool HasBoolValue { get; private set; }
		public bool HasIntValue { get; private set; }
		public bool HasStringValue { get; private set; }

		private bool _boolValue;
		private int _intValue;
		private string _stringValue;

		public static implicit operator TagValue(int x) {
			return new TagValue {HasValue = true, HasIntValue = true, _intValue = x};
		}

		public static implicit operator TagValue(bool x) {
			return new TagValue {HasValue = true, HasBoolValue = true, _boolValue = x};
		}

		public static implicit operator TagValue(string x) {
			return new TagValue {HasValue = true, HasStringValue = true, _stringValue = x};
		}

		public static implicit operator int(TagValue a) {
			return a._intValue;
		}

		public static implicit operator bool(TagValue a) {
			return a._boolValue;
		}

		public static implicit operator string(TagValue a) {
			return a._stringValue;
		}

		public static bool operator == (TagValue x, TagValue y) {
		    return x.Equals(y);
		}

		public static bool operator != (TagValue x, TagValue y) {
			return !(x == y);
		}

		public override bool Equals(object o) {
			if (!(o is TagValue))
				return false;
			return Equals((TagValue) o);
		}

		public bool Equals(TagValue o) {
		    // Both must have a value or no value
			if (HasValue != o.HasValue)
				return false;
			// If neither have a value, they are equal
			if (!(HasValue || o.HasValue))
				return true;
			// Precedence order: int -> bool -> string
			if (HasIntValue && o.HasIntValue)
				return _intValue == o._intValue;
			if (HasBoolValue && o.HasBoolValue)
				return _boolValue == o._boolValue;
			if (HasStringValue && o.HasStringValue)
				return _stringValue == o._stringValue;
			return false;
		}

		public override string ToString() {
			if (!HasValue)
				return "null";
			if (HasBoolValue)
				return _boolValue.ToString();
			if (HasIntValue)
				return _intValue.ToString();
			if (HasStringValue)
				return _stringValue;
			return "unknown";
		}

		public override int GetHashCode() {
			return ToString().GetHashCode();
		}
	}
}
