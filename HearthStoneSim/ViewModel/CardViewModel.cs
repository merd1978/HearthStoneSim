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

        /// <summary>
        /// Initializes a new instance of the CardViewModel class.
        /// </summary>
        public CardViewModel()
        {
            Card = new Card { Name = "murlock", Attack = 5, Health = 2 };
        }
    }
}