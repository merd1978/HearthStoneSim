using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Tasks.SimpleTasks;

namespace HearthStoneSimCore.CardSets
{
	public static class CoreCardSet
	{
		public static void LoadCoreCardSet()
	    {
		    // --------------------------------------- MINION - WARLOCK
		    // [BRM_006] Imp Gang Boss - COST:3 [ATK:2/HP:4] 
		    // - Race: demon, Set: fp2, Rarity: common
		    // --------------------------------------------------------
		    // Text: Whenever this minion takes damage, summon a 1/1 Imp.
		    // --------------------------------------------------------
	        Cards.All["BRM_006"].Enchant = new Enchant
	        {
	            //Area = EnchantmentArea.SELF,
	            Activation = EnchantmentActivation.BOARD,
	            Trigger = TriggerType.OnDamage,
	            Effect = new SummonTask("BRM_006t")
	        };
	    }
	}
}
