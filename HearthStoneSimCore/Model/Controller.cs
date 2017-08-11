using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public class Controller : Core
    {
	    public string FriendlyName { get; }
		public Game Game { get; private set; }
        public Deck Deck { get; private set; }
        public Hand Hand { get; private set; }
        public Board Board { get; private set; }
	    public Controller Opponent => Game.Player1 == this ? Game.Player2 : Game.Player1;

		public Controller(Game game, string name, int playerId) : base(Card.CardPlayer,
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
            Deck = new Deck(this);
            Hand = new Hand(this);
            Board = new Board(this);
        }
	}
}
