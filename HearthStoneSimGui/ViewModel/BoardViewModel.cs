using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimGui.DragDrop;
using HearthStoneSimCore.Model;
using HearthStoneSimCore.Enums;
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
		public Board Board { get; set; }
        public Game Game { get; set; }

        private bool _insertionActive;
        private int _insertionIndex;

        /// <summary>
        /// Initializes a new instance of the TableViewModel class.
        /// </summary>
        public BoardViewModel(Controller controller, Game game, Board board)
        {
	        Controller = controller;
			Game = game;
            Board = board;
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
            BoardCards = new ObservableCollection<Minion>(Board.Cards);
            //BoardCards = new ObservableCollection<Minion>();
            //foreach (var card in Board.Cards)
            //{
            //    BoardCards.Add(new Minion(card));
            //}

        }

        #region DragDrop
        public void NotifyMe(NotificationMessage notificationMessage)
        {
            //string notification = notificationMessage.Notification;
            if (!_insertionActive) return;
            _insertionActive = false;
            BoardCards[_insertionIndex].IsHitTest = true;
            BoardCards.RemoveAt(_insertionIndex);
        }

	    public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Minion;
            if (sourceItem == null) return;
            dropInfo.Effects = DragDropEffects.Copy;

			// Attack by minion
			if (sourceItem.Zone == Zone.PLAY) return;

			// Play minion
	        if (sourceItem.Controller != Controller) return;		// Ignoring opponents minion
			var target = (ObservableCollection<Minion>)dropInfo.TargetCollection;
            if (_insertionActive)
            {
                if (_insertionIndex == dropInfo.InsertIndex) return;
                target.RemoveAt(_insertionIndex);
                _insertionIndex = dropInfo.InsertIndex > _insertionIndex
                    ? dropInfo.InsertIndex - 1
                    : dropInfo.InsertIndex;
                target.Insert(_insertionIndex, sourceItem);
            }
            else
            {
                _insertionActive = true;
                _insertionIndex = dropInfo.InsertIndex;
                sourceItem.IsHitTest = false;
                target.Insert(_insertionIndex, sourceItem);
            }

        }

	    public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Minion;
            var target = dropInfo.TargetCollection as ObservableCollection<Minion>;
            if (sourceItem == null || target == null) return;
			// Play Minion
			if (sourceItem.Controller == Controller && sourceItem.Zone == Zone.HAND)
            {
                _insertionActive = false;
                sourceItem.IsHitTest = true;
	            Game.ClearPreDamage();      //resetting predemage will stop the damage animation
				Game.PlayMinion(dropInfo.DragInfo.SourceIndex, _insertionIndex);
                return;
            }
            //Check is it attacking enemy minion
            var targetItem = dropInfo.TargetItem as Minion;
            if (targetItem == null) return;
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
}