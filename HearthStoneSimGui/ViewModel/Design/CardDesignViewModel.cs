using GalaSoft.MvvmLight;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class CardDesignViewModel : ViewModelBase
    {
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
    }
}