using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Core
    {
        private readonly CoreData _data;
        //public int Id { get; set; }
        public string Id => _data.Card.Id;
        public string Name => _data.Card.Name;
        public string CardTextInHand => _data.Card.CardTextInHand;
        public string ArtImageSource => _data.Card.ArtImageSource;
        public string FrameImageSource => _data.Card.FrameImageSource;

	    public int Cost => this[GameTag.COST];
        public Zone Zone
        {
            get => (Zone) this[GameTag.ZONE];
            set => this[GameTag.ZONE] = (int) value;
        }
        //public Zone Zone;

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

        protected internal Core(Card card, Dictionary<GameTag, int> tags)
        {
            _data = new CoreData(card, tags);
        }

        // Cloning copy constructor
        protected internal Core(Core cloneFrom)
        {
            _data = new CoreData(cloneFrom._data);
        }
    }
}
