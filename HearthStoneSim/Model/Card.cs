using System.Collections.Generic;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Cost => Tags[GameTag.COST];

        public uint Health { get; set; }
        public uint Attack { get; set; }
 
        public Dictionary<GameTag, int> Tags { get; set; }
        public Dictionary<PlayRequirements, int> Requirements { get; set; }

        //public card() {}

        public override string ToString()
        {
            return Name;

        }

    }
}
