using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimGui.DragDrop;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Factory;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class BoardViewModel : ViewModelBase, IDropTarget, IDragSource
    {
        private ObservableCollection<Minion> _boardCards;
        public ObservableCollection<Minion> BoardCards
        {
            get => _boardCards;
            set => Set(nameof(BoardCards), ref _boardCards, value);
        }

	    public Controller Controller { get; }
        public Game Game { get; }

        private BoardMode _boardMode;
        public bool IsBusy;

        public RelayCommand AnimationCompleatedCommand { get; set; }
        private void ExecuteAnimationCompleatedCommand()
        {
            Game.ClearPreDamage(Controller);
            UpdateState();
            Messenger.Default.Send(new NotificationMessage("BoardAnimationCompleated"));
            IsBusy = false;
        }

        /// <summary>
        /// Initializes a new instance of the TableViewModel class.
        /// </summary>
        public BoardViewModel(Controller controller)
        {
	        Controller = controller;
            Game = controller.Game;
            UpdateState();
            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
            AnimationCompleatedCommand = new RelayCommand(ExecuteAnimationCompleatedCommand);
        }

        public BoardViewModel()
        {
            #if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
            {
                var game = new Game();
                BoardCards = new ObservableCollection<Minion>
                {
                    CardFactory.MinionFromName(game.Player1, "Суккуб")
                };
            }
            #endif
        }

        public void UpdateState()
        {
            BoardCards = new ObservableCollection<Minion>(Controller.Board.ToList());
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
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "DragCanceled":
                    if (_boardMode != BoardMode.INSERTION) return;
                    _boardMode = BoardMode.NORMAL;
                    BoardCards[PreviewInsertIndex].IsHitTest = true;
                    BoardCards.RemoveAt(PreviewInsertIndex);
                    break;
            }
            
        }

	    public void DragOver(IDropInfo dropInfo)
        {
            //Game.Log(LogLevel.INFO, BlockType.PLAY, "BoardViewModel", "DragOver");

            if (!(dropInfo.Data is Minion sourceItem)) return;
            dropInfo.Effects = DragDropEffects.Copy;

            // Attack by minion
            if (sourceItem.Zone.Type == Zone.PLAY) return;

            // Play minion
            if (sourceItem.Controller != Controller) return;		// Ignoring opponents minion
            if (Controller.Board.IsFull) return;
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
			if (sourceItem.Controller == Controller && sourceItem.Zone.Type == Zone.HAND)
			{
			    _boardMode = BoardMode.NORMAL;
			    sourceItem.IsHitTest = true;
                Controller.PlayCard(sourceItem, null, PreviewInsertIndex);
                return;
            }
            //Check is it attacking enemy minion
            if (!(dropInfo.TargetItem is Minion targetItem)) return;
	        if (sourceItem.Controller == Controller) return;
            if (sourceItem.Zone.Type == Zone.PLAY)
            {
                Controller.MinionAttack(sourceItem, targetItem);
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
            return Game.CurrentPlayer == Controller && !IsBusy;
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