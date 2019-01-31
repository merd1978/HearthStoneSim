using System;
using System.Collections.Generic;
using System.ComponentModel;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Factory;
using HearthStoneSimCore.Tasks;

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

        public TaskQueue TaskQueue { get; }

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

            Player1 = new PlayerHuman(this, "Player1", 1, p1Dict);
            Player2 = new PlayerHuman(this, "Player2", 2, p2Dict);
            TaskQueue = new TaskQueue(this);

            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Суккуб"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Главарь банды бесов"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Всадник на волке"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Морозный йети"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Герой Штормграда"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Лик тлена"));
            Player1.HandZone.Add(CardFactory.PlayableFromName(Player1, "Священник син'дорай")); 

            Player2.BoardZone.Add(CardFactory.MinionFromName(Player2, "Суккуб"));
            Player2.BoardZone.Add(CardFactory.MinionFromName(Player2, "Ящер Кровавой Топи"));

	        CurrentPlayer = Player1;
        }

        public void Process(IPlayerTask gameTask)
        {
            gameTask.Process();
            StateChanged = true;
        }

        /// <summary>
        /// Checks for entities which are pending to be destroyed and updated 
        /// active auras accordingly.
        /// </summary>
        public void DeathProcessingAndAuraUpdate()
        {
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
            //do
            //{
            //    GraveYard();    // Death Creation Step

            //    ProcessTasks(); // Death Resolution Phase
            //} while (DeadMinions.Count != 0);
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
        }

        public void ClearPreDamage()
        {
            foreach (var playable in Player1.BoardZone.ToList())
            {
                if (playable is Minion minion)
                {
                    minion.PreDamage = 0;
                    minion.IsDamaged = false;
                }
            }
            foreach (var playable in Player2.BoardZone.ToList())
            {
                if (playable is Minion minion)
                {
                    minion.PreDamage = 0;
                    minion.IsDamaged = false;
                }
            }
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
    }
}
