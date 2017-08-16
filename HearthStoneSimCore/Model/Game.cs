using System;
using System.Collections.Generic;
using System.ComponentModel;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Tasks;

namespace HearthStoneSimCore.Model
{
    public class Game : INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged;

		public Controller Player1 { get; private set; }
        public Controller Player2 { get; private set; }

        public Controller CurrentPlayer
	    {
		    get => Player1[GameTag.CURRENT_PLAYER] == 1
			    ? Player1
			    : Player2[GameTag.CURRENT_PLAYER] == 1 ? Player2 : null;
		    set
		    {
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
            Player1 = new PlayerHuman(this, "Player1", 1);
            Player2 = new PlayerHuman(this, "Player2", 2);
            TaskQueue = new TaskQueue(this);

            Player1.Hand.Add(Cards.FromName("Суккуб"));
            Player1.Hand.Add(Cards.FromName("Главарь банды бесов"));
            Player1.Hand.Add(Cards.FromName("Всадник на волке"));
            Player1.Hand.Add(Cards.FromName("Морозный йети"));
            Player1.Hand.Add(Cards.FromName("Герой Штормграда"));
            Player1.Hand.Add(Cards.FromName("К'Тун"));

            Player1.Board.Add(Cards.FromName("Суккуб"));
            Player2.Board.Add(Cards.FromName("Ящер Кровавой Топи"));

	        CurrentPlayer = Player1;
        }

        public void Process(IPlayerTask gameTask)
        {
            gameTask.Process();
            StateChanged = true;
        }

        public void DeathProcessingAndAuraUpdate()
        {
            //check for dead weapons, minions, heroes
            //GraveYard();

            //enable enchants and trigers
            //AuraUpdate();

            while (TaskQueue.Count > 0)
            {
                TaskQueue.Process();
                //GraveYard();
                //AuraUpdate();
            }
        }

        public void PlayMinion(int cardIndex, int insertIndex)
        {
            var card = Player1.Hand.Cards[cardIndex];
            Player1.Board.Add((Minion)card, insertIndex);
            //Player1.Hand.Cards.RemoveAt(indexCard);
            StateChanged = true;
        }

        public void ClearPreDamage()
        {
            foreach (var card in Player1.Board.Cards)
            {
                card.PreDamage = 0;
                card.IsDamaged = false;
            }
            foreach (var card in Player2.Board.Cards)
            {
                card.PreDamage = 0;
                card.IsDamaged = false;
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
