using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HearthStoneSim.DragDrop;
using HearthStoneSim.Model;
using HearthStoneSim.Model.Enums;
using HearthStoneSim.Model.GameCore;

namespace HearthStoneSim.ViewModel
{
   /// <summary>
   /// This class contains properties that a View can data bind to.
   /// <para>
   /// See http://www.galasoft.ch/mvvm
   /// </para>
   /// </summary>
   public class BoardViewModel : ViewModelBase, IDropTarget
   {
      private ObservableCollection<Core> _boardCards;
      public ObservableCollection<Core> BoardCards
      {
         get => _boardCards;
         set => Set(nameof(BoardCards), ref _boardCards, value);
      }

      public Board Board { get; set; }
      public Game Game { get; set; }

      /// <summary>
      /// Initializes a new instance of the TableViewModel class.
      /// </summary>
      public BoardViewModel(Game game, Board board)
      {
         Game = game;
         Board = board;
         BoardCards = new ObservableCollection<Core>(Board.Cards);
      }

      public BoardViewModel()
      {
         //BoardCards = new ObservableCollection<Core> {Cards.All["EX1_306"] };
      }

      public void UpdateBoardState()
      {
         BoardCards = new ObservableCollection<Core>(Board.Cards);
         //.._boardCards.CollectionChanged()
      }

      #region DragDrop

      public void DragOver(IDropInfo dropInfo)
      {
         var sourceItem = dropInfo.Data as Core;
         //var targetItem = dropInfo.TargetItem as Core;

         if (sourceItem != null)
         {
            dropInfo.Effects = DragDropEffects.Copy;
         }
      }

      public void Drop(IDropInfo dropInfo)
      {
         var sourceItem = dropInfo.Data as Core;
         var target = dropInfo.TargetCollection as ObservableCollection<Core>;
         if (sourceItem == null || target == null) return;
         if (sourceItem.Zone == Zone.HAND)
         {
            Game.ClearPreDamage();
            Game.PlayMinion(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
            //sourceItem.Zone = Zone.PLAY;
            //target.Add(sourceItem);
            MessengerInstance.Send(new NotificationMessage("ViewRefresh"));
            return;
         }
         //Check is it attacking enemy minion
         var targetItem = dropInfo.TargetItem as Core;
         if (targetItem == null) return;
         if (ReferenceEquals(dropInfo.DragInfo.SourceCollection, dropInfo.TargetCollection)) return;
         if (sourceItem.Zone == Zone.PLAY)
         {
            Game.ClearPreDamage();
            Game.Attack(dropInfo.DragInfo.SourceIndex, dropInfo.TargetItemIndex);
            MessengerInstance.Send(new NotificationMessage("ViewRefresh"));
         }
         //if (dropInfo.DragInfo.SourceItem != null) var i = 5;
      }
      #endregion

      private void Timeline_OnCompleted(object sender, EventArgs e)
      {
         throw new NotImplementedException();
      }
   }
}