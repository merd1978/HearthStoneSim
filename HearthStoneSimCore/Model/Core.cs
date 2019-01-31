﻿using System.Collections.Generic;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Zones;

namespace HearthStoneSimCore.Model
{
    public class Core
    {
        /// <summary>
        /// This object holds the original tag values, defined through the constructor 
        /// of this instance.
        /// These tags are usefull when values are needed without any buffs/debuffs applied.
        /// </summary>
        private readonly CoreData _data;

        /// <summary>Gets or sets the game instance from which this entity is part of.</summary>
        /// <value>The game instance.</value>
        public Game Game { get; set; }

        /// <summary>Gets or sets the owner of this entity, the controller who played the entity.</summary>
        /// <value>The controller/owner object.</value>
        public Controller Controller { get; set; }

        /// <summary>Gets or sets the zone in which the entity exists.</summary>
        /// <value>The zone, <see cref="T:Model.Zones.Zone" />.</value>
        public IZone Zone { get; set; }

        public int Id => _data.Card.Id;
        public string Name => _data.Card.Name;
        public string CardTextInHand => _data.Card.Text;
        public string ArtImageSource => _data.Card.ArtImageSource;
        public string FrameImageSource => _data.Card.FrameImageSource;

	    public int Cost => this[GameTag.COST];
		public Card Card => _data.Card;

        /// <summary> Get all enchants hooked onto this entity.</summary>
        /// <value>
        /// Enchants force a temporary effect, for as long as this entity is in play, onto the game.
        /// </value>
        public List<Enchant> Enchants { get; } = new List<Enchant>();

        public int this[GameTag tag]
        {
	        get
	        {
				int value = _data[tag];
		        for (int i = 0; i < Enchants.Count; i++)
			        value = Enchants[i].Apply(this, tag, value);
		        return value;
			}
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
