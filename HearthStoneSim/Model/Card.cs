namespace HearthStoneSim.Model
{
    public class Card
    {
        public string Name { get; set; }
        public uint Cost { get; set; }
        public uint Health { get; set; }
        public uint Attack { get; set; }
        public string Kind { get; set; }
        
        //public card() {}

        public override string ToString()
        {
            return Name;

        }

    }
}
