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
    public class HandViewModel : ViewModelBase
    {
        public ObservableCollection<Card> Cards { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HandViewModel class.
        /// </summary>
        public HandViewModel()
        {
            Cards = new ObservableCollection<Card> {new Card(), new Card(), new Card(), new Card(), new Card(), new Card() };
        }
    }
}