using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Tasks.SimpleTasks;

namespace HearthStoneSimCore.CardSets
{
	public static class BasicSet
	{
		public static void Load()
	    {
			// --------------------------------------- MINION - NEUTRAL
		    // [CS2_222] Stormwind Champion - COST:7 [ATK:6/HP:6]
		    // - Fac: alliance, Set: core, Rarity: free
		    // --------------------------------------------------------
		    // Text: Your other minions have +1/+1.
		    // --------------------------------------------------------
		    // GameTag:
		    // - AURA = 1
		    // --------------------------------------------------------
		    Cards.All["CS2_222"].Enchantment = new Enchantment
		    {
			    //Area = EnchantmentArea.SELF,
			    //Trigger = TriggerType.OnDamage,
			    //Task = new SummonTask("BRM_006t")
		    };

			// --------------------------------------- MINION - NEUTRAL
			// [EX1_019] Shattered Sun Cleric - COST:3 [ATK:3/HP:2]
			// - Fac: neutral, Set: core, Rarity: free
			// --------------------------------------------------------
			// Text: <b>Battlecry:</b> Give a friendly minion +1/+1.
			// --------------------------------------------------------
			// GameTag:
			// - BATTLECRY = 1
			// --------------------------------------------------------
			// PlayReq:
			// - REQ_MINION_TARGET = 0
			// - REQ_FRIENDLY_TARGET = 0
			// - REQ_TARGET_IF_AVAILABLE = 0
			// --------------------------------------------------------
			Cards.All["EX1_019"].Enchantment = new Enchantment
			{
				//Area = EnchantmentArea.TARGET,
				//Activation = EnchantmentActivation.BATTLECRY,
				//Enchant = Buff.AttackHealth(1)
			};
		}
	}
}
