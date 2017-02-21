using System.Collections.Generic;
using System.Windows.Media;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model
{
    public class Card
    {
        public int this[GameTag t] => Tags.ContainsKey(t) ? Tags[t] : 0;
        public string Id { get; set; }
        public string Name { get; set; }
        public string CardTextInHand { get; set; }
        public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
        public string FrameImageSource => @"../Images/inhand_minion_druid.png";

        public int Cost => this[GameTag.COST];
        public int Health => this[GameTag.HEALTH];
        public int Attack => this[GameTag.ATK];

        public Dictionary<GameTag, int> Tags { get; set; }
        public Dictionary<PlayRequirements, int> Requirements { get; set; }

        public override string ToString() { return Name; }
    }
}
