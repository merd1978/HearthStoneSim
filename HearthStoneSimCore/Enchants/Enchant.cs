using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Tasks;

namespace HearthStoneSimCore.Enchants
{
	public class Enchant
	{
	    public EnchantmentActivation Activation { get; set; } = EnchantmentActivation.BOARD;
	    //public Enchant Enchant { get; set; }
	    public TriggerType Trigger { get; set; }
        public ISimpleTask Effect { get; set; }
    }

    public enum EnchantmentActivation
    {
        BATTLECRY,
        DEATHRATTLE,
        BOARD,
        HAND,
        DECK,
        SECRET_OR_QUEST,
        SPELL,
        WEAPON,
        SETASIDE,
        NONE
    }
}
