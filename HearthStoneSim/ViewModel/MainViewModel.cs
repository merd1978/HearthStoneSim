using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight;
using HearthStoneSim.DragDrop;
using HearthStoneSim.Model;
using HearthStoneSim.Model.GameCore;

namespace HearthStoneSim.ViewModel
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
        public TableViewModel TableViewModelPlayer1 { get; private set; }
        public TableViewModel TableViewModelPlayer2 { get; private set; }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        public void Drop(IDropInfo dropInfo)
        {
            dropInfo.NotHandled = true;
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;

            Version deploy = Assembly.GetExecutingAssembly().GetName().Version;
            MainWindowTitle = $"HearthStoneSim v{deploy.Major}.{deploy.Minor}.{deploy.Build}";
            _dataService.GetCardDefs((cards, error) =>
            {
                if (error != null)
                {
                    // Report error here
                    return;
                }
                Cards.All = cards;
            });
            Game = new Game();

            HandViewModelPlayer1 = new HandViewModel(Game.Player1.Hand);
            HandViewModelPlayer2 = new HandViewModel(Game.Player1.Hand);
            TableViewModelPlayer1 = new TableViewModel(Game.Player1.Board);
            TableViewModelPlayer2 = new TableViewModel(Game.Player1.Board);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}