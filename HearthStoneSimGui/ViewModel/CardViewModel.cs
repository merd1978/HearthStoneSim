using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;

namespace HearthStoneSim.ViewModel
{
   /// <summary>
   /// This class contains properties that a View can data bind to.
   /// <para>
   /// See http://www.galasoft.ch/mvvm
   /// </para>
   /// </summary>
   public class CardViewModel : ViewModelBase
   {
      public Card Card { get; private set; }
      public string Id { get; } = "EX1_306";
      public string Name { get; } = "Эпичная mouse";
      public int Cost { get; } = 18;
      public int Attack { get; } = 28;
      public int Health { get; } = 88;
      public string CardTextInHand { get; private set; } = "Win button";
      public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
      public string FrameImageSource => @"../Images/inhand_minion_druid.png";
      public int PreDamage { get; } = -88;

      /// <summary>
      /// Initializes a new instance of the CardViewModel class.
      /// </summary>
      public CardViewModel()
      {
         //Card = new Card { Name = "murlock", Attack = 5, Health = 2 };
      }
   }
}