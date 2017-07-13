using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
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
