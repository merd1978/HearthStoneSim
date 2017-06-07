using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using HearthStoneSim.Model;

namespace HearthStoneSim.ViewModel
{
   /// <summary>
   /// This class contains properties that a View can data bind to.
   /// <para>
   /// See http://www.galasoft.ch/mvvm
   /// </para>
   /// </summary>
   public class TableViewModel : ViewModelBase
   {
      public ObservableCollection<ICard> BoardCards { get; private set; }
      public Board Board { get; set; }

      /// <summary>
      /// Initializes a new instance of the TableViewModel class.
      /// </summary>
      public TableViewModel(Board board)
      {
         Board = board;
         BoardCards = new ObservableCollection<ICard>(Board.Cards);
      }

      public TableViewModel()
      {
         BoardCards = new ObservableCollection<ICard> {Cards.All["EX1_306"] };
      }

      public void UpdateBoardState()
      {
         BoardCards = new ObservableCollection<ICard>(Board.Cards);
      }
   }
}