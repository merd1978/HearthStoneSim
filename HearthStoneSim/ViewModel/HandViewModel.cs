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
        public ObservableCollection<Card> HandCards { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HandViewModel class.
        /// </summary>
        public HandViewModel()
        {
            HandCards = new ObservableCollection<Card> { Cards.All["AT_002"], Cards.All["CS2_072"], Cards.All["EX1_tk31"], Cards.All["CS2_203"] };
        }
    }
}