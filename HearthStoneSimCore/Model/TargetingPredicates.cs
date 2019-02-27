using System;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
	public delegate bool AvailabilityPredicate(Controller controller);
	public delegate bool TargetingPredicate(Minion target);

	public static class TargetingPredicates
	{
		private static readonly TargetingPredicate ReqMurlocTarget
			= t => t.Race == Race.MURLOC;
		private static readonly TargetingPredicate ReqDemonTarget
			= t => t.Race == Race.DEMON;
		private static readonly TargetingPredicate ReqMechTarget
			= t => t.Race == Race.MECHANICAL;
		private static readonly TargetingPredicate ReqElementalTarget
			= t => t.Race == Race.ELEMENTAL;
		private static readonly TargetingPredicate ReqBeastTarget
			= t => t.Race == Race.BEAST;
		private static readonly TargetingPredicate ReqTotemTarget
			= t => t.Race == Race.TOTEM;
		private static readonly TargetingPredicate ReqPirateTarget
			= t => t.Race == Race.PIRATE;
		private static readonly TargetingPredicate ReqDragonTarget
			= t => t.Race == Race.DRAGON;

		public static readonly TargetingPredicate ReqFrozenTarget
			= t => t.IsFrozen;
		public static readonly TargetingPredicate ReqDamagedTarget
			= t => t.Damage > 0;
		public static readonly TargetingPredicate ReqUndamagedTarget
			= t => t.Damage == 0;
		public static readonly TargetingPredicate ReqMustTargetTaunter
			= t => t.HasTaunt;
		public static readonly TargetingPredicate ReqStealthedTarget
			= t => t.HasStealth;
		public static readonly TargetingPredicate ReqTargetWithDeathrattle
			= t => t.HasDeathrattle;
		public static readonly TargetingPredicate ReqLegendaryTarget
			= t => t.Card.Rarity == Rarity.LEGENDARY;

		public static TargetingPredicate ReqTargetWithRace(int race)
		{
			switch ((Race) race)
			{
				case Race.MURLOC:
					return ReqMurlocTarget;
				case Race.DEMON:
					return ReqDemonTarget;
				case Race.MECHANICAL:
					return ReqMechTarget;
				case Race.ELEMENTAL:
					return ReqElementalTarget;
				case Race.BEAST:
					return ReqBeastTarget;
				case Race.TOTEM:
					return ReqTotemTarget;
				case Race.PIRATE:
					return ReqPirateTarget;
				case Race.DRAGON:
					return ReqDragonTarget;
				// Ignores
				case Race.UNDEAD:
				case Race.EGG:
					return null;
				default:
					throw new IndexOutOfRangeException(
						$@"Targeting Race {(Race)race} is not implemented! Please Check TargetingPredicates.cs");
			}
		}

		public static TargetingPredicate ReqTargetMaxAttack(int value)
		{
			return t => t.AttackDamage <= value;
		}

		public static TargetingPredicate ReqTargetMinAttack(int value)
		{
			return t => t.AttackDamage >= value;
		}

		private static readonly AvailabilityPredicate ReqMin1EnemyMinion
			= c => c.Opponent.Board.Count >= 1;
		private static readonly AvailabilityPredicate ReqMin2EnemyMinions
			= c => c.Opponent.Board.Count >= 2;
		private static readonly AvailabilityPredicate ReqMin1TotalMinion
			= c => c.Board.Count + c.Opponent.Board.Count > 0;

		public static readonly AvailabilityPredicate ReqTargetForCombo
			= c => c.IsComboActive;

		public static readonly AvailabilityPredicate ElementalPlayedLastTurn
			= c => c.NumElementalsPlayedLastTurn > 0;

		public static readonly AvailabilityPredicate DragonInHand
			= c => c.DragonInHand;

		public static readonly AvailabilityPredicate ReqNumMinionSlots
			= c => !c.Board.IsFull;

		public static readonly AvailabilityPredicate ReqHandNotFull
			= c => !c.Hand.IsFull;

		public static readonly AvailabilityPredicate ReqWeaponEquipped
			= c => c.Hero.Weapon != null;

		public static readonly AvailabilityPredicate ReqFriendlyMinionDiedThisGame
			= c => c.Graveyard.Any(q => q is Minion m && m.ToBeDestroyed);

		public static readonly AvailabilityPredicate ReqSecretZoneCapForNonSecret
			= c => !c.Secret.IsFull;

		public static AvailabilityPredicate MinimumFriendlyMinions(int value)
		{
			if (value == 1)
				return c => c.Board.Count > 0;
			if (value == 4)
				return c => c.Board.Count >= 4;

			throw new NotImplementedException(
				$@"REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS = {value} is not implemented. Please Check \Loader\TargetingPredicates.cs");
		}

		public static AvailabilityPredicate MinimumFriendlySecrets(int value)
		{
			if (value == 1)
				return c => c.Secret.Count > 0;
			throw new NotImplementedException(
				$@"REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS = {value} is not implemented. Please Check \Loader\TargetingPredicates.cs");
		}

		public static AvailabilityPredicate ReqMinimumEnemyMinions(int value)
		{
			if (value == 1)
				return ReqMin1EnemyMinion;
			if (value == 2)
				return ReqMin2EnemyMinions;
			if (value == 0)
				return c => true;
			throw new NotImplementedException(
				$@"REQ_MINIMUM_ENEMY_MINIONS = {value} is not implemented. Please Check \Loader\TargetingPredicates.cs");
		}

		public static AvailabilityPredicate ReqMinimumTotalMinions(int value)
		{
			if (value == 1)
				return ReqMin1TotalMinion;
			return c => c.Board.Count + c.Opponent.Board.Count >= value;
		}
	}
}
