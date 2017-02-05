using System.Collections.Generic;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }

        //public int Cost => Tags[GameTag.COST];
        public int Cost => this[GameTag.COST];

        public int Health => this[GameTag.HEALTH];
        public int Attack => this[GameTag.ATK];

        public Dictionary<GameTag, int> Tags { get; set; }
        public Dictionary<PlayRequirements, int> Requirements { get; set; }

        public int this[GameTag t] => Tags.ContainsKey(t) ? Tags[t] : 0;
        //public card() {}

        public override string ToString()
        {
            return Name;

        }

    }
}
