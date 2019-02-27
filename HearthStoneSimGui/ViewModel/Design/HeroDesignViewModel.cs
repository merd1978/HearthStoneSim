using GalaSoft.MvvmLight;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class HeroDesignViewModel : ViewModelBase
    {
        public string Id { get; } = "HERO_04";
        public string Name { get; } = "Утер";
        public int Health { get; } = 30;
        public string ArtImageSource => @"d:/CardArt/Full/" + Id + ".png";
        public string FrameImageSource => @"../Images/hero_frame.png";
        public int PreDamage { get; } = -88;
        public bool IsDamaged { get;  } = false;
        public bool IsDead { get; } = false;
    }
}