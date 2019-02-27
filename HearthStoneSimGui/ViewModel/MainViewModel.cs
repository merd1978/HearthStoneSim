using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using GalaSoft.MvvmLight;
using HearthStoneSimCore.Enums;
using HearthStoneSimGui.DragDrop;
using HearthStoneSimCore.Model;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IDropTarget, IDragSource
    {
        public bool IfFollowTail { get; set; } = true;  //scrolls the listbox to the bottom when a new item is added

        /// <summary>
        /// Get the window title (name and assembly version)
        /// </summary>
        public string MainWindowTitle { get; private set; }

        public Game Game { get; private set; }

        public HeroViewModel HeroPlayer1ViewModel { get; private set; }
        public HeroViewModel HeroPlayer2ViewModel { get; private set; }
        public HandViewModel HandPlayer1ViewModel { get; private set; }
        public HandViewModel HandPlayer2ViewModel { get; private set; }
        public BoardViewModel BoardPlayer1ViewModel { get; private set; }
        public BoardViewModel BoardPlayer2ViewModel { get; private set; }
        public ManaBarViewModel ManaBarPlayer1ViewModel { get; private set; }
        public ManaBarViewModel ManaBarPlayer2ViewModel { get; private set; }

        private ObservableCollection<Playable> _deckPlayer1;
        public ObservableCollection<Playable> DeckPlayer1
        {
            get => _deckPlayer1;
            set => Set(nameof(DeckPlayer1), ref _deckPlayer1, value);
        }

        private ObservableCollection<Playable> _deckPlayer2;
        public ObservableCollection<Playable> DeckPlayer2
        {
            get => _deckPlayer2;
            set => Set(nameof(DeckPlayer2), ref _deckPlayer2, value);
        }

        public ObservableCollection<string> Log { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Version gui =Assembly.GetExecutingAssembly().GetName().Version;
            string core = 
                Assembly.GetAssembly(typeof(Core)).GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            MainWindowTitle = $"HearthStoneSim GUI v{gui.Major}.{gui.Minor}.{gui.Build} Core v{core}";

            Game = new Game();
            Game.PropertyChanged += GamePropertyChanged;

            DeckPlayer1 = new ObservableCollection<Playable>(Game.Player1.Deck.ToList());
            DeckPlayer2 = new ObservableCollection<Playable>(Game.Player2.Deck.ToList());

            HeroPlayer1ViewModel = new HeroViewModel(Game.Player1.Hero);
            HeroPlayer2ViewModel = new HeroViewModel(Game.Player2.Hero);
            HandPlayer1ViewModel = new HandViewModel(Game.Player1);
            HandPlayer2ViewModel = new HandViewModel(Game.Player2);
            BoardPlayer1ViewModel = new BoardViewModel(Game.Player1);
            BoardPlayer2ViewModel = new BoardViewModel(Game.Player2);
            ManaBarPlayer1ViewModel = new ManaBarViewModel(Game.Player1);
            ManaBarPlayer2ViewModel = new ManaBarViewModel(Game.Player2);

            Log = new ObservableCollection<string>();
            Game.Log(LogLevel.INFO, BlockType.PLAY, "Game", "Starting new game now!");
        }

        private void GamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StateChanged":
                    //Set Board IsBusy until animation compleated
                    if (Game.HasDamagedOrDead())
                    {
                        BoardPlayer1ViewModel.IsBusy = true;
                        BoardPlayer2ViewModel.IsBusy = true;
                            HandPlayer1ViewModel.IsBusy = true;
                            HandPlayer2ViewModel.IsBusy = true;
                    }
                    // Update game state
                    HeroPlayer1ViewModel.UpdateState();
                    HeroPlayer2ViewModel.UpdateState();
                        BoardPlayer1ViewModel.UpdateState();
                        BoardPlayer2ViewModel.UpdateState();
                            HandPlayer1ViewModel.UpdateState();
                            HandPlayer2ViewModel.UpdateState();
                                ManaBarPlayer1ViewModel.UpdateState();
                                ManaBarPlayer2ViewModel.UpdateState();
                    break;
                case "LogChanged":
                    while (Game.Logs.Count > 0)
                    {
                        var logEntry = Game.Logs.Dequeue();
                        if (logEntry.Level <= LogLevel.INFO)
                        {
                            Log.Add($"[{logEntry.BlockType}] - {logEntry.Location}: {logEntry.Text}");
                        }
                    }
                    break;
            }
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        public void Drop(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        public void StartDrag(IDragInfo dragInfo)
        {
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return false;
        }

        public void Dropped(IDropInfo dropInfo)
        {
        }

        public void DragCancelled()
        {
        }

        public void DragLeave(object sender)
        {
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}