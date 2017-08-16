using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Core
    {
        private readonly CoreData _data;
	    public Game Game { get; set; }
        public string Id => _data.Card.Id;
        public string Name => _data.Card.Name;
        public string CardTextInHand => _data.Card.Text;
        public string ArtImageSource => _data.Card.ArtImageSource;
        public string FrameImageSource => _data.Card.FrameImageSource;

	    public int Cost => this[GameTag.COST];
		public Card Card => _data.Card;

		public Zone Zone
        {
            get => (Zone) this[GameTag.ZONE];
            set => this[GameTag.ZONE] = (int) value;
        }
        public int ZonePosition
        {
            get => this[GameTag.ZONE_POSITION];
            set => this[GameTag.ZONE_POSITION] = value;
        }


        public int this[GameTag tag]
        {
            get => _data[tag];
            set
            {
                // if (value < 0) value = 0;
                // Ignore unchanged data
                var oldValue = _data[tag];
                if (value == oldValue) return;
                //Changing(t, oldValue, value);
                _data[tag] = value;
                //Game?.CoreChanged(this, t, oldValue, value);
            }
        }

        protected internal Core(Game game, Card card, Dictionary<GameTag, int> tags)
        {
	        Game = game;
			_data = new CoreData(card, tags);
        }

        // Cloning copy constructor
        protected internal Core(Core cloneFrom)
        {
	        Game = cloneFrom.Game;
			_data = new CoreData(cloneFrom._data);
        }

	    public override string ToString()
	    {
		    return $"'{Card.Name}[{Id}]'";
	    }
	}
}
