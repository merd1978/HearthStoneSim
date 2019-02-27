using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public struct Tag
    {
        public GameTag GameTag { get; }
        public TagValue TagValue { get; }

        public Tag(GameTag gameTag, TagValue tagValue)
        {
            GameTag = gameTag;
            TagValue = tagValue;
        }
    }
}
