using System.Collections.Generic;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Card
    {
        public int this[GameTag t] => Tags.ContainsKey(t) ? Tags[t] : 0;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
        public string FrameImageSource => @"../Images/inhand_minion_druid.png";

        public CardType Type => (CardType)this[GameTag.CARDTYPE];

        public Dictionary<GameTag, int> Tags { get; set; }
        public Dictionary<PlayRequirements, int> Requirements { get; set; }
		public Enchant Enchant { get; set; }
        public override string ToString() { return Name; }

        //default constructor
        public Card()
        {
        }

        // Cloning copy constructor
        public Card(Card cloneFrom)
        {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Text = cloneFrom.Text;
            Tags = new Dictionary<GameTag, int>(cloneFrom.Tags);
        }

	    internal static Card CardPlayer => new Card()
	    {
		    Id = "Player",
		    Name = "Player",
		    Tags = new Dictionary<GameTag, int> { [GameTag.CARDTYPE] = (int)CardType.PLAYER },
		    //PlayRequirements = new Dictionary<PlayReq, int>(),
	    };
	}
}
