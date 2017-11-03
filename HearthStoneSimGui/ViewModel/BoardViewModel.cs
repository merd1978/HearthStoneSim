using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimGui.DragDrop;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Zones;
using HearthStoneSimCore.Tasks.PlayerTasks;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class BoardViewModel : ViewModelBase, IDropTarget, IDragSource
    {
        private ObservableCollection<Minion> _boardCards;
        public ObservableCollection<Minion> BoardCards
        {
            get => _boardCards;
            set => Set(nameof(BoardCards), ref _boardCards, value);
        }

	    public Controller Controller { get; set; }
		public BoardZone BoardZone { get; set; }
        public Game Game { get; set; }

        private BoardMode _boardMode;

        /// <summary>
        /// Initializes a new instance of the TableViewModel class.
        /// </summary>
        public BoardViewModel(Controller controller, Game game, BoardZone board)
        {
	        Controller = controller;
			Game = game;
            BoardZone = board;
            UpdateBoardState();
            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
            //BoardCards = new ObservableCollection<Minion>(Board.Cards);
        }

        public BoardViewModel()
        {
            BoardCards = new ObservableCollection<Minion> { new Minion(null, Cards.FromName("Суккуб"), null) };
        }

        public void UpdateBoardState()
        {
            BoardCards = new ObservableCollection<Minion>(BoardZone.Elements);
        }

        #region DragDrop
        //preview position where to insert element if the drop occurred, position unknown if -1
        private int _previewInsertIndex;
        public int PreviewInsertIndex
        {
            get => _previewInsertIndex;
            set => DragDrop.DragDrop.PreviewInsertIndex = _previewInsertIndex = value;
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            //string notification = notificationMessage.Notification;
            if (_boardMode != BoardMode.INSERTION) return;
            _boardMode = BoardMode.NORMAL;
            BoardCards[PreviewInsertIndex].IsHitTest = true;
            BoardCards.RemoveAt(PreviewInsertIndex);
        }

	    public void DragOver(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is Minion sourceItem)) return;
            dropInfo.Effects = DragDropEffects.Copy;

			// Attack by minion
			if (sourceItem.Zone == Zone.PLAY) return;

			// Play minion
	        if (sourceItem.Controller != Controller) return;		// Ignoring opponents minion
            if (Controller.BoardZone.IsFull) return;
			var target = (ObservableCollection<Minion>)dropInfo.TargetCollection;
            if (_boardMode == BoardMode.INSERTION)
            {
                if (PreviewInsertIndex == dropInfo.InsertIndex) return;
                target.RemoveAt(PreviewInsertIndex);
                PreviewInsertIndex = dropInfo.InsertIndex > PreviewInsertIndex
                    ? dropInfo.InsertIndex - 1
                    : dropInfo.InsertIndex;
                target.Insert(PreviewInsertIndex, sourceItem);
            }
            else
            {
                _boardMode = BoardMode.INSERTION;
                PreviewInsertIndex = dropInfo.InsertIndex;
                sourceItem.IsHitTest = false;
                target.Insert(PreviewInsertIndex, sourceItem);
            }
        }

	    public void Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is Minion sourceItem) || !(dropInfo.TargetCollection is ObservableCollection<Minion>)) return;
			// Play Minion
			if (sourceItem.Controller == Controller && sourceItem.Zone == Zone.HAND)
			{
			    _boardMode = BoardMode.NORMAL;
			    sourceItem.IsHitTest = true;
                Game.ClearPreDamage();      //resetting predemage will stop the damage animation

				Game.Process(new PlayCardTask(Game.Player1, sourceItem, null, PreviewInsertIndex));
                return;
            }
            //Check is it attacking enemy minion
            if (!(dropInfo.TargetItem is Minion targetItem)) return;
	        if (sourceItem.Controller == Controller) return;
            if (sourceItem.Zone == Zone.PLAY)
            {
                Game.ClearPreDamage();
                Game.Process(new MinionAttackTask(Game.Player1, sourceItem, targetItem));
			}
        }

	    public void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1) dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();

            dragInfo.Effects = (dragInfo.Data != null) ?
                DragDropEffects.Copy | DragDropEffects.Move :
                DragDropEffects.None;
        }

	    public bool CanStartDrag(IDragInfo dragInfo)
        {
            return Game.CurrentPlayer == Controller;
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
    }

    internal enum BoardMode
    {
        NORMAL = 1,
        INSERTION = 2,
        SELECT_TARGET = 3
    }
}