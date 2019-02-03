using System;
using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimCore.Model.Factory;
using HearthStoneSimCore.Model.Zones;
using HearthStoneSimGui.DragDrop;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class HandViewModel : ViewModelBase, IDragSource
    {
        private ObservableCollection<Playable> _handCards;
        public ObservableCollection<Playable> HandCards
        {
            get => _handCards;
            set => Set(nameof(HandCards), ref _handCards, value);
        }
        public Controller Controller { get; set; }

		/// <summary>
		/// Initializes a new instance of the HandViewModel class.
		/// </summary>
		public HandViewModel(Controller controller)
		{
			Controller = controller;
		    UpdateState();
        }

        public HandViewModel()
        {
            #if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
            {
                var game = new Game();
                HandCards = new ObservableCollection<Playable>
                {
                    CardFactory.PlayableFromName(game.Player1, "Суккуб"),
                    CardFactory.PlayableFromName(game.Player2, "Всадник на волке"),
                    CardFactory.PlayableFromName(game.Player2, "Всадник на волке"),
                    CardFactory.PlayableFromName(game.Player1, "Суккуб"),
                };
            }
            #endif
        }

        public void UpdateState()
        {
            HandCards = new ObservableCollection<Playable>(Controller.HandZone.ToList());
        }

        #region DragDrop

        public void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1) dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();

            dragInfo.Effects = (dragInfo.Data != null) ?
                                DragDropEffects.Copy | DragDropEffects.Move :
                                DragDropEffects.None;

            //activtion of target selection mode if source requires
            if (dragInfo.Data is Playable playable && playable.NeedsTargetList)
            {
                switch (dragInfo.Data)
                {
                    case Minion _:
                        DragDrop.DragDrop.SelectTargetAfterDrop = true;
                        break;
                    case Spell _:
                        DragDrop.DragDrop.SelectTargetForce = true;
                        break;
                }
            }
        }

	    public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

	    public void Dropped(IDropInfo dropInfo)
        {
        }

	    public void DragCancelled()
        {
            Messenger.Default.Send(new NotificationMessage("DragCanceled"));
            DragDrop.DragDrop.SelectTargetAfterDrop = false;
            DragDrop.DragDrop.SelectTargetForce = false;
        }

	    public void DragLeave(object sender)
        {
            Messenger.Default.Send(new NotificationMessage("DragCanceled"));
        }

	    public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion
    }
}