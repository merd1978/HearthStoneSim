using System;
using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSimCore.Enums;
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
        public ObservableCollection<Playable> HandCards { get; private set; }
	    public Controller Controller { get; set; }
		public Hand Hand { get; set; }

		/// <summary>
		/// Initializes a new instance of the HandViewModel class.
		/// </summary>
		public HandViewModel(Controller controller, Hand hand)
		{
			Controller = controller;
			Hand = hand;
            HandCards = new ObservableCollection<Playable>(hand.Cards);
        }

        public HandViewModel()
        {
            HandCards = new ObservableCollection<Playable>
            {
                Playable.FromName(null, "Суккуб"),
                Playable.FromName(null, "Всадник на волке")
            };
        }

        #region DragDrop

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1) dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();

            dragInfo.Effects = (dragInfo.Data != null) ?
                                DragDropEffects.Copy | DragDropEffects.Move :
                                DragDropEffects.None;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
        }

        void IDragSource.DragCancelled()
        {
            Messenger.Default.Send(new NotificationMessage("DragCanceled"));
        }

        void IDragSource.DragLeave(object sender)
        {
            Messenger.Default.Send(new NotificationMessage("DragCanceled"));
        }

        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        #endregion
    }
}