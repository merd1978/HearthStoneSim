using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model
{
    public struct Tag
    {
        public GameTag Name { get; }
        public TagValue Value { get; }

        public Tag(GameTag name, TagValue value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
