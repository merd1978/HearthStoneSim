using System;
using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model.Zones;
using HearthStoneSimGui.DragDrop;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
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
		public HandZone HandZone { get; set; }

		/// <summary>
		/// Initializes a new instance of the HandViewModel class.
		/// </summary>
		public HandViewModel(Controller controller, HandZone hand)
		{
			Controller = controller;
			HandZone = hand;
		    UpdateState();
            HandCards = new ObservableCollection<Playable>(hand.Elements);
        }

        public HandViewModel()
        {
            HandCards = new ObservableCollection<Playable>
            {
                Playable.FromName(null, "Суккуб"),
                Playable.FromName(null, "Всадник на волке")
            };
        }

        public void UpdateState()
        {
            HandCards = new ObservableCollection<Playable>(HandZone.Elements);
        }

        #region DragDrop

        public void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1) dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();

            dragInfo.Effects = (dragInfo.Data != null) ?
                                DragDropEffects.Copy | DragDropEffects.Move :
                                DragDropEffects.None;

            if (dragInfo.Data is Minion minion && minion.NeedsTargetList)
            {
                DragDrop.DragDrop.SelectTargetAfterDrop = true;
            }
            else if (dragInfo.Data is Spell spell && spell.NeedsTargetList)
            {
                DragDrop.DragDrop.SelectTargetForce = true;
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