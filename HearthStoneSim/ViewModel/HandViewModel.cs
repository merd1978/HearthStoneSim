using GalaSoft.MvvmLight;
using HearthStoneSim.Model;
using System.Collections.ObjectModel;

namespace HearthStoneSim.ViewModel
{
   /// <summary>
   /// This class contains properties that a View can data bind to.
   /// <para>
   /// See http://www.galasoft.ch/mvvm
   /// </para>
   /// </summary>
   public class HandViewModel : ViewModelBase
   {
      public ObservableCollection<ICard> HandCards { get; private set; }
      public Hand Hand { get; set; }

      /// <summary>
      /// Initializes a new instance of the HandViewModel class.
      /// </summary>
      public HandViewModel(Hand hand)
      {
         Hand = hand;
         HandCards = new ObservableCollection<ICard>(hand.Cards);
      }

      public HandViewModel()
      {
         HandCards = new ObservableCollection<ICard>
         {
            //Cards.All["EX1_306"], Cards.All["CS2_172"], Cards.All["CS2_124"], Cards.All["CS2_182"],
            Cards.All["CS2_222"], Cards.All["OG_279"]
         };
      }
   }
}