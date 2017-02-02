using System;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model
{
    public struct Tag
    {
        public GameTag Name { get; }
        public TagValue Value { get; }

        public Tag(GameTag Name, TagValue Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
