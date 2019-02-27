using System;
using System.Collections.Generic;
using System.ComponentModel;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Factory;

namespace HearthStoneSimCore.Model
{
    public class Game : INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged;

		public Controller Player1 { get; private set; }
        public Controller Player2 { get; private set; }

        private Controller _currentPlayer;
        public Controller CurrentPlayer
	    {
            get => _currentPlayer;
            private set
            {
                _currentPlayer = value;
                //if (!History) return;
                value.Opponent[GameTag.CURRENT_PLAYER] = 0;
                value[GameTag.CURRENT_PLAYER] = 1;
            }
        }

        /// <summary>
        /// List of Minions that ready to be destroyed and to be removed from the BoardZone.
        /// </summary>
        public readonly List<Minion> DeadMinions = new List<Minion>();

        //Indicates when to update gui
        private bool _stateChanged;
        public bool StateChanged
        {
            get => _stateChanged;
            set
            {
                _stateChanged = value;
                NotifyChanged("StateChanged");
            }
        }
        private void NotifyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Queue<LogEntry> Logs { get; set; } = new Queue<LogEntry>();

        #region Property

        /// <summary>
        /// Gets or sets the number of killed minions for this turn.
        /// </summary>
        /// <value>The amount of killed minions.</value>
        public int NumMinionsKilledThisTurn { get; set; }

        #endregion

        public Game()
        {
            //player1 settings
            var p1Dict = new Dictionary<GameTag, int>
            {
                //[GameTag.HERO_ENTITY] = heroId,
                [GameTag.MAXHANDSIZE] = 10,
                [GameTag.STARTHANDSIZE] = 4,
                [GameTag.PLAYER_ID] = 1,
                [GameTag.TEAM_ID] = 1,
                [GameTag.ZONE] = (int) Zone.PLAY,
                [GameTag.CONTROLLER] = 1,
                [GameTag.MAXRESOURCES] = 10,
                [GameTag.CARDTYPE] = (int) CardType.PLAYER
            };

            //player2 settings
            var p2Dict = new Dictionary<GameTag, int>
            {
                //[GameTag.HERO_ENTITY] = heroId,
                [GameTag.MAXHANDSIZE] = 10,
                [GameTag.STARTHANDSIZE] = 4,
                [GameTag.PLAYER_ID] = 2,
                [GameTag.TEAM_ID] = 2,
                [GameTag.ZONE] = (int) Zone.PLAY,
                [GameTag.CONTROLLER] = 2,
                [GameTag.MAXRESOURCES] = 10,
                [GameTag.CARDTYPE] = (int) CardType.PLAYER
            };

            Player1 = new PlayerHuman(this, "Player1", 1, CardClass.PALADIN ,p1Dict);
            Player2 = new PlayerHuman(this, "Player2", 2, CardClass.HUNTER, p2Dict);

            // setting up the decks ...
            Player1.FillDeck();
            Player2.FillDeck();

            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Succubus"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Imp Gang Boss"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Wolfrider"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Chillwind Yeti"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Stormwind Champion"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Dalaran Mage"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Elven Archer"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "River Crocolisk"));
            Player1.Hand.Add(CardFactory.PlayableFromName(Player1, "Raid Leader"));

            Player2.Board.Add(CardFactory.MinionFromName(Player2, "Bloodfen Raptor"));
            Player2.Board.Add(CardFactory.MinionFromName(Player2, "War Golem"));

	        CurrentPlayer = Player1;
        }

        /// <summary>
        /// Checks for entities which are pending to be destroyed and updated 
        /// active auras accordingly.
        /// </summary>
        public void DeathProcessingAndAuraUpdate()
        {
            //Inter-Phase steps

            //AuraUpdate();

            // Summon Resolution Step
            //if (TriggerManager.HasOnSummonTrigger)
            //{
            //    List<Minion> minions = SummonedMinions;
            //    TaskQueue.StartEvent();
            //    for (int i = 0; i < minions.Count; i++)
            //    {
            //        TriggerManager.OnSummonTrigger(minions[i]);
            //    }
            //    ProcessTasks();
            //    TaskQueue.EndEvent();
            //}
            //SummonedMinions.Clear();

            //TaskQueue.StartEvent();
            do
            {
                GraveYard();    // Death Creation Step

                //ProcessTasks(); // Death Resolution Phase
            } while (DeadMinions.Count != 0);
            //TaskQueue.EndEvent();

            //AuraUpdate();	// Aura Update (Other) step(Not implemented)
        }

        /// <summary>
        /// Move destroyed entities from <see cref="Zone.PLAY"/>
        /// into <see cref="Zone.GRAVEYARD"/>
        /// Death Creation Step (Death event is created but not resolved here)
        /// </summary>
        public void GraveYard()
        {
            foreach (Minion minion in DeadMinions)
            {
                Log(LogLevel.INFO, BlockType.PLAY, "Game", $"{minion} is Dead! Graveyard say 'Hello'!");

                // Death event created
                //TriggerManager.OnDeathTrigger(minion);

                minion.LastBoardPosition = minion.ZonePosition;
                minion.Zone.Remove(minion);

                //if (minion.HasDeathrattle)
                //    minion.ActivateTask(PowerActivation.DEATHRATTLE);

                minion.Controller.Graveyard.Add(minion);
                minion.Controller.NumFriendlyMinionsThatDiedThisTurn++;
                CurrentPlayer.NumMinionsPlayerKilledThisTurn++;
                NumMinionsKilledThisTurn++;
            }

            DeadMinions.Clear();
        }

        public void ClearPreDamage(Controller controller)
        {
            controller.Board.ForEach(p => p.PreDamage = 0);
            controller.Hero.PreDamage = 0;
        }

        public void Log(LogLevel level, BlockType block, string location, string text)
        {
            //if (!_gameConfig.Logging)
            //    return;

            Logs.Enqueue(new LogEntry()
            {
                TimeStamp = DateTime.Now,
                Level = level,
                Location = location,
                BlockType = block,
                Text = text
            });
	        NotifyChanged("LogChanged");
		}

        public bool HasDamagedOrDead()
        {
            if (DeadMinions.Count > 0) return true;
            //if (Player1.Board.Any(p => p.IsDamaged) || Player2.Board.Any(p => p.IsDamaged)) return true;
            //if (Player1.Hero.IsDamaged || Player2.Hero.IsDamaged) return true;
            return false;
        }
    }
}
