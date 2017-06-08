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
      public ObservableCollection<Core> BoardCards { get; private set; }
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
         BoardCards.Clear();
         Board.Cards.ToList().ForEach(BoardCards.Add);
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
            Game.Move(dropInfo.DragInfo.SourceIndex);
            //sourceItem.Zone = Zone.PLAY;
            //target.Add(sourceItem);
            MessengerInstance.Send(new NotificationMessage("ViewRefresh"));
            return;
         }
         var targetItem = dropInfo.TargetItem as Core;
         if (targetItem == null) return;
         if (sourceItem.Zone == Zone.PLAY)
         {
            //targetItem.Damage += 1;
            //BoardCards[0].Health = 0;
            //sourceItem.Damage += 1;
            Game.Attack(0,0);
            MessengerInstance.Send(new NotificationMessage("ViewRefresh"));
         }
         //if (dropInfo.DragInfo.SourceItem != null) var i = 5;
      }
      #endregion
   }
}