﻿using System;
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
    public class MainViewModel : ViewModelBase, IDropTarget
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Get the window title (name and assembly version)
        /// </summary>
        public string MainWindowTitle { get; private set; }

        public Game Game { get; private set; }

        public HandViewModel HandViewModelPlayer1 { get; private set; }
        public HandViewModel HandViewModelPlayer2 { get; private set; }
        public BoardViewModel BoardViewModelPlayer1 { get; private set; }
        public BoardViewModel BoardViewModelPlayer2 { get; private set; }

        public ObservableCollection<string> Log { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;

            //_dataService.GetCardDefs((cards, error) =>
            //{
            //    if (error != null)
            //    {
            //        // Report error here
            //        return;
            //    }
            //    Cards.All = cards;
            //});

            Version deploy = Assembly.GetExecutingAssembly().GetName().Version;
            MainWindowTitle = $"HearthStoneSim v{deploy.Major}.{deploy.Minor}.{deploy.Build}";

            Game = new Game();
            Game.PropertyChanged += GamePropertyChanged;

            HandViewModelPlayer1 = new HandViewModel(Game.Player1, Game.Player1.Hand);
            HandViewModelPlayer2 = new HandViewModel(Game.Player2, Game.Player2.Hand);
            BoardViewModelPlayer1 = new BoardViewModel(Game.Player1, Game, Game.Player1.Board);
            BoardViewModelPlayer2 = new BoardViewModel(Game.Player2, Game, Game.Player2.Board);

			Log = new ObservableCollection<string>();
			Game.Log(LogLevel.INFO, BlockType.PLAY, "Game", "Starting new game now!");
		}

        private void GamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
	        switch (e.PropertyName)
	        {
				case "StateChanged":
					// Update game state
					BoardViewModelPlayer1.UpdateBoardState();
					BoardViewModelPlayer2.UpdateBoardState();
					break;
				case "LogChanged":
					while (Game.Logs.Count > 0)
					{
						var logEntry = Game.Logs.Dequeue();
						if (logEntry.Level <= LogLevel.INFO)
						{
							Log.Add($"[{logEntry.BlockType}] - {logEntry.Location}: {logEntry.Text}\n");
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
        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}