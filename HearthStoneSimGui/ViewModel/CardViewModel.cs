using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CardViewModel : ViewModelBase
    {
        //public Playable Card { get; }
        public string Id { get; } = "EX1_306";
        public string Name { get; } = "Эпичная mouse";
        public int Cost { get; } = 18;
        public int AttackDamage { get; } = 28;
        public int Health { get; } = 88;
        public string CardTextInHand { get; } = "Win button";
        public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
        public string FrameImageSource => @"../Images/inhand_minion_druid.png";
        public int PreDamage { get; } = -88;
        public bool IsDamaged { get;  } = false;
        public bool IsDead { get; } = false;

        /// <summary>
        /// Initializes a new instance of the CardViewModel class.
        /// </summary>
        public CardViewModel()
        {
            //Card = card;
        }
    }
}