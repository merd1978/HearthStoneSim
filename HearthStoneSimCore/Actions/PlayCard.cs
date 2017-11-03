using System;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Actions
{
	public static class PlayCard
	{
		public static bool PlayCardBlock(Controller player, Playable source, Character target, int zonePosition, int chooseOne)
		{
			if (!PrePlayPhase(player, source, target, zonePosition, chooseOne))
			{
				return false;
			}

			if (!PayPhase(player, source))
			{
				return false;
			}

			// play block
			//if (c.Game.History)
			//	player.Game.PowerHistory.Add(PowerHistoryBuilder.BlockStart(BlockType.PLAY, source.Id, "", 0, target?.Id ?? 0));

			player.NumCardsPlayedThisTurn++;
			player.LastCardPlayed = source.Id;

			// show entity
			//if (c.Game.History)
			//	c.Game.PowerHistory.Add(PowerHistoryBuilder.ShowEntity(source));

			// target is beeing set onto this gametag
			if (target != null)
			{
				source.CardTarget = target.Id;
			}
			switch (source)
			{
			    case Hero _:
			        PlayHero(player, (Hero)source, target);
			        break;
			    case Minion _:
			        PlayMinion(player, (Minion)source, target, zonePosition);
			        break;
			    case Weapon _:
			        // - OnPlay Phase --> OnPlay Trigger (Illidan)
			        //   (death processing, aura updates)
			        //OnPlayTrigger(player, (Weapon)source);
			        //RemoveFromZone(player, source);
			        //PlayWeapon(player, (Weapon)source);
			        break;
                case Spell _:
                    // - OnPlay Phase --> OnPlay Trigger (Illidan)
                    //   (death processing, aura updates)
                    //OnPlayTrigger(player, (Spell)source);
                    //source[GameTag.TAG_LAST_KNOWN_COST_IN_HAND] = source[GameTag.COST];
                    // remove from hand zone
	                //RemoveFromZone(player, source);
					//PlaySpell(player, (Spell)source, target);
                    break;
			}
			source.CardTarget = -1;

			//player.NumOptionsPlayedThisTurn++;
			//player.IsComboActive = true;

			//if (c.Game.History)
			//	c.Game.PowerHistory.Add(PowerHistoryBuilder.BlockEnd());

			return true;
		}

		public static bool PrePlayPhase(Controller player, Playable source, Character target, int zonePosition, int chooseOne)
		{
			// can't play because we got already board full
			if (source is Minion && player.BoardZone.IsFull)
			{
				player.Game.Log(LogLevel.WARNING, BlockType.PLAY, "PrePlayPhase", $"Board has already {player.BoardZone.MaxSize} minions.");
				return false;
			}

			// set choose one option
			//Playable subSource = chooseOne > 0 ? source.ChooseOnePlayables[chooseOne - 1] : source;

			// check if we can play this card and the target is valid
			//if (!source.IsPlayableByPlayer || !subSource.IsPlayableByCardReq || !subSource.IsValidPlayTarget(target))
			//{
			//	return false;
			//}

			// copy choose one enchantment to the actual source
			//if (source.ChooseOne)
			//{
			//	// [OG_044] Fandral Staghelm, Aura active 
			//	if (c.ChooseBoth
			//	&& !source.Card.Id.Equals("EX1_165") // OG_044a, using choose one 0 option
			//	&& !source.Card.Id.Equals("BRM_010") // OG_044b, using choose one 0 option
			//	&& !source.Card.Id.Equals("AT_042")) // OG_044c, using choose one 0 option
			//	{
			//		source.Enchantments.AddRange(source.ChooseOnePlayables[0].Enchantments);
			//		source.Enchantments.AddRange(source.ChooseOnePlayables[1].Enchantments);
			//	}
			//	else
			//	{
			//		source.Enchantments = subSource.Enchantments;
			//	}
			//}

			// replace enchantments with the no combo or combo one ..
			//if (source.Combo && !(source is Minion))
			//{
			//	if (source.Enchantments.Count > 1)
			//	{
			//		source.Enchantments = new List<Enchantment> { source.Enchantments[c.IsComboActive ? 1 : 0] };
			//	}
			//	else if (c.IsComboActive && source.Enchantments.Count > 0)
			//	{
			//		source.Enchantments = new List<Enchantment> { source.Enchantments[0] };
			//	}
			//	else
			//	{
			//		source.Enchantments = new List<Enchantment> { };
			//	}
			//}

			return true;
		}

		public static bool PayPhase(Controller player, Playable source)
		{
			player.OverloadOwed += source.Overload;
			int cost = source.Cost;
			if (cost > 0)
			{
				int tempUsed = Math.Min(player.TemporaryMana, cost);
				player.TemporaryMana -= tempUsed;
				player.UsedMana += cost - tempUsed;
				player.TotalManaSpentThisGame += cost;
			}
			player.Game.Log(LogLevel.INFO, BlockType.PLAY, "PayPhase", $"Paying {source} for {source.Cost} Mana, remaining mana is {player.RemainingMana}.");
			return true;
		}

		public static void PlayHero(Controller player, Hero hero, Character target)
		{
			
		}

		public static void PlayMinion(Controller player, Minion minion, Character target, int zonePosition)
		{
		    // - PreSummon Phase --> PreSummon Trigger (TideCaller)
		    //   (death processing, aura updates)

		    // remove from hand zone
            player.HandZone.Remove(minion);

		    if (!minion.HasCharge)
		        minion.IsExhausted = true;

		    player.Game.Log(LogLevel.INFO, BlockType.PLAY, "PlayMinion",
		        $"{player.Name} plays Minion {minion} with target {target} to board position {zonePosition}");

            // - PreSummon Phase --> PreSummon Phase Trigger (Tidecaller)
            //   (death processing, aura updates)
            player.BoardZone.Add(minion, zonePosition);
		    player.Game.DeathProcessingAndAuraUpdate();

		    // - OnPlay Phase --> OnPlay Trigger (Illidan)
		    //   (death processing, aura updates)
		    //  OnPlayTrigger(player, minion);

		    // - BattleCry Phase --> Battle Cry Resolves
		    //   (death processing, aura updates)
		    //minion.ApplyEnchantments(EnchantmentActivation.BATTLECRY, Zone.PLAY, target);
		    //if (minion.Combo && c.IsComboActive)
		    //    minion.ApplyEnchantments(EnchantmentActivation.COMBO, Zone.PLAY, target);
		    // check if [LOE_077] Brann Bronzebeard aura is active
		    //if (c.ExtraBattlecry)
		    //    //if (minion[GameTag.BATTLECRY] == 2)
		    //{
		    //    minion.ApplyEnchantments(EnchantmentActivation.BATTLECRY, Zone.PLAY, target);
		    //}
		    //player.Game.DeathProcessingAndAuraUpdate();

		    // - After Play Phase --> After play Trigger / Secrets (Mirror Entity)
		    //   (death processing, aura updates)
		    //minion.JustPlayed = false;
		    //player.Game.DeathProcessingAndAuraUpdate();

		    // - After Summon Phase --> After Summon Trigger
		    //   (death processing, aura updates)
		    //AfterSummonTrigger(player, minion);

		    player.NumMinionsPlayedThisTurn++;

		    //switch (minion.Race)
		    //{
		    //    case Race.ELEMENTAL:
		    //        c.NumElementalsPlayedThisTurn++;
		    //        break;
		    //    case Race.MURLOC:
		    //        c.NumMurlocsPlayedThisGame++;
		    //        break;
		    //}
        }

		public static void PlaySpell(Controller player, Spell spell, Character target)
		{

		}

		public static void PlayWeapon(Controller c, Weapon weapon)
		{

		}
	}
}
