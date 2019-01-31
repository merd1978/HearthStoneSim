using System.Collections.Generic;
using HearthStoneSimCore.Enchants;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Card
    {
        public int this[GameTag t] => Tags.ContainsKey(t) ? Tags[t] : 0;
        /// <summary>
        /// Unique id of that card alphanumeric representation.
        /// </summary>
        public string CardId { get; set; }
        /// <summary>
        /// Unique id of that card nummeric representation.
        /// </summary>
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string ArtImageSource => @"d:/CardArt/Full/" + CardId + ".png";
        public string FrameImageSource => @"../Images/inhand_minion_druid.png";

        public int ATK { get; private set; }
        public int Health { get; private set; }
        public int SpellPower { get; private set; }
        public bool Taunt { get; private set; }
        public bool Charge { get; private set; }
        public bool Stealth { get; private set; }
        public bool Poisonous { get; private set; }
        public bool DivineShield { get; private set; }
        public bool Windfury { get; private set; }
        public bool LifeSteal { get; private set; }
        public bool Echo { get; private set; }
        public bool Rush { get; private set; }
        public bool CantBeTargetedBySpells { get; private set; }
        public bool CantBeTargetedByHeroPowers => CantBeTargetedBySpells;
        public bool CantAttack { get; private set; }
        public bool Modular { get; private set; }
        public bool ChooseOne { get; private set; }
        public bool Combo { get; private set; }
        public bool IsSecret { get; private set; }
        public bool IsQuest { get; private set; }
        public bool Deathrattle { get; }
        public bool Untouchable { get; private set; }
        public bool HideStat { get; private set; }
        public bool ReceivesDoubleSpelldamageBonus { get; private set; }
        public bool Freeze { get; }

        public CardType Type => (CardType)this[GameTag.CARDTYPE];
	    public Rarity Rarity => (Rarity)this[GameTag.RARITY];

		public Dictionary<GameTag, int> Tags { get; set; }
        /// <summary>
        /// Requirements that must have been met before this card can be moved into
        /// play zone.
        /// <see cref="PlayRequirements"/> for all possibilities.
        /// </summary>
        public Dictionary<PlayReq, int> PlayRequirements { get; set; }
		public Enchantment Enchantment { get; set; }
        public override string ToString() { return Name; }

        #region Tergeting
        public bool RequiresTarget => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_TO_PLAY);

        /// <summary>
        /// Requires a target for combo
        /// </summary>
        public bool RequiresTargetForCombo => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_FOR_COMBO);

        /// <summary>
        /// Requires a target if available
        /// </summary>
        public bool RequiresTargetIfAvailable => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABLE);

        /// <summary>
        /// Requires a target if available and dragon in hand
        /// </summary>
        public bool RequiresTargetIfAvailableAndDragonInHand
            => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND);

        /// <summary>
        /// Requires a target if available and element played last turn
        /// </summary>
        public bool RequiresTargetIfAvailableAndElementalPlayedLastTurn
            => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN);

        /// <summary>
        /// Requires a target if available and minimum friendly minions
        /// </summary>
        public bool RequiresTargetIfAvailableAndMinimumFriendlyMinions
            => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS);

        /// <summary>
        /// Requires a target if available and minimum friendly secrets
        /// </summary>
        public bool RequiresTargetIfAvailableAndMinimumFriendlySecrets
            => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS);

        public bool RequiresTargetIfAvailableAndNo3CostCardInDeck
            => PlayRequirements.ContainsKey(PlayReq.REQ_TARGET_IF_AVAILABLE_AND_NO_3_COST_CARD_IN_DECK);

        #endregion Targeting

        //default constructor
        public Card()
        {
        }

        // Cloning copy constructor
        public Card(Card cloneFrom)
        {
            CardId = cloneFrom.CardId;
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Text = cloneFrom.Text;
            Tags = new Dictionary<GameTag, int>(cloneFrom.Tags);
        }

	    internal static Card CardPlayer => new Card()
	    {
		    Name = "Player",
		    Tags = new Dictionary<GameTag, int> { [GameTag.CARDTYPE] = (int)CardType.PLAYER },
		    //PlayRequirements = new Dictionary<PlayReq, int>(),
	    };
	}
}
