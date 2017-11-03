using System.Collections.Generic;

namespace HearthStoneSimCore.Model.Zones
{
	public class SecretZone
	{
		public Controller Controller { get; set; }
	    public List<Spell> Secrets = new List<Spell>();

	    public int Count => Secrets.Count;

        public SecretZone(Controller controller)
		{
			Controller = controller;
		}
	}
}
