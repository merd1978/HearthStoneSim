using System.Windows.Data;
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
    public class CardViewModel : ViewModelBase
    {
        public Card Card { get; private set; }
        public string Id = "EX1_306";
        public string Name = "Эпичная мышь";
        public string CardTextInHand = "Win button";
        public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
        public string FrameImageSource => @"../Images/inhand_minion_druid.png";

        /// <summary>
        /// Initializes a new instance of the CardViewModel class.
        /// </summary>
        public CardViewModel()
        {
            //Card = new Card { Name = "murlock", Attack = 5, Health = 2 };
        }
    }
}