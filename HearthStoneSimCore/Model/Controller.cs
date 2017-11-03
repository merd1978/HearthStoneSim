using System.Collections.Generic;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Zones;

namespace HearthStoneSimCore.Model
{
    public class Controller : Core
    {
	    public string FriendlyName { get; }
	    /// <summary>
	    /// The hero entity representing this player.
	    /// </summary>
	    public Hero Hero { get; set; }
		public DeckZone DeckZone { get; private set; }
        public HandZone HandZone { get; private set; }
        public BoardZone BoardZone { get; private set; }
		public SecretZone SecretZone { get; private set; }
	    public Controller Opponent => Game.Player1 == this ? Game.Player2 : Game.Player1;

        #region Mana
        /// <summary>
        /// Total amount of mana available to this player.
        /// This value DOES NOT contain temporary mana!
        /// This value is limited to 1 turn and should be reset in the next turn.
        /// </summary>
        public int BaseMana
        {
            get => this[GameTag.RESOURCES];
            set => this[GameTag.RESOURCES] = value;
        }

        /// <summary>
        /// Additionall mana gained during this turn.
        /// </summary>
        public int TemporaryMana
        {
            get => this[GameTag.TEMP_RESOURCES];
            set => this[GameTag.TEMP_RESOURCES] = value;
        }

        /// <summary>
        /// Amount of mana used by this player.
        /// 
        /// This value is limited to 1 turnand should be reset in the next turn.
        /// </summary>
        public int UsedMana
        {
            get => this[GameTag.RESOURCES_USED];
            set => this[GameTag.RESOURCES_USED] = value;
        }

        /// <summary>
        /// Amount of mana crystals which will be locked during the next turn.
        /// </summary>
        public int OverloadOwed
        {
            get => this[GameTag.OVERLOAD_OWED];
            set => this[GameTag.OVERLOAD_OWED] = value;
        }

        /// <summary>
        /// Amount of mana crystals locked this turn.
        /// 
        /// The subtraction of BASE_MANA and this value gives the available
        /// resources during this turn.
        /// </summary>
        public int OverloadLocked
        {
            get => this[GameTag.OVERLOAD_LOCKED];
            set => this[GameTag.OVERLOAD_LOCKED] = value;
        }

        /// <summary>
        /// The amount of mana available to actually use after calculating all resource factors.
        /// </summary>
        public int RemainingMana => BaseMana + TemporaryMana - (UsedMana + OverloadLocked);

        public int TotalManaSpentThisGame
        {
            get => this[GameTag.NUM_RESOURCES_SPENT_THIS_GAME];
            set => this[GameTag.NUM_RESOURCES_SPENT_THIS_GAME] = value;
        }
        #endregion Mana

        public int NumCardsPlayedThisTurn
	    {
		    get => this[GameTag.NUM_CARDS_PLAYED_THIS_TURN];
		    set => this[GameTag.NUM_CARDS_PLAYED_THIS_TURN] = value;
	    }

	    public int NumMinionsPlayedThisTurn
	    {
		    get => this[GameTag.NUM_MINIONS_PLAYED_THIS_TURN];
		    set => this[GameTag.NUM_MINIONS_PLAYED_THIS_TURN] = value;
	    }

	    public int NumElementalsPlayedLastTurn
	    {
		    get => this[GameTag.NUM_ELEMENTAL_PLAYED_LAST_TURN];
		    set => this[GameTag.NUM_ELEMENTAL_PLAYED_LAST_TURN] = value;
	    }

		public int LastCardPlayed
        {
            get => this[GameTag.LAST_CARD_PLAYED];
            set => this[GameTag.LAST_CARD_PLAYED] = value;
        }

	    /// <summary>
	    /// Indicates if combo enchantment effects of next cards should be executed or not.
	    /// 
	    /// Combo is active if at least one card has been played this turn.
	    /// </summary>
	    public bool IsComboActive
	    {
		    get => this[GameTag.COMBO_ACTIVE] == 1;
		    set => this[GameTag.COMBO_ACTIVE] = value ? 1 : 0;
	    }

		public Controller(Game game, string name, int playerId) : base(game, Card.CardPlayer,
	        new Dictionary<GameTag, int>
	        {
		        //[GameTag.HERO_ENTITY] = heroId,
		        [GameTag.MAXHANDSIZE] = 10,
		        [GameTag.STARTHANDSIZE] = 4,
		        [GameTag.PLAYER_ID] = playerId,
		        [GameTag.TEAM_ID] = playerId,
		        [GameTag.ZONE] = (int)Zone.PLAY,
		        [GameTag.CONTROLLER] = playerId,
		        //[GameTag.ENTITY_ID] = id,
		        [GameTag.MAXRESOURCES] = 10,
		        [GameTag.CARDTYPE] = (int)CardType.PLAYER

	        })
        {
	        FriendlyName = name;
			Game = game;
            DeckZone = new DeckZone(this);
            HandZone = new HandZone(this);
            BoardZone = new BoardZone(this);
			SecretZone = new SecretZone(this);
        }
	}
}
