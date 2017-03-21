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
        public HandViewModel(Deck Deck)
        {
            HandCards = new ObservableCollection<Card>(Deck.Cards);
            //HandCards = new ObservableCollection<Card>
            //{
            //    Cards.All["EX1_306"], Cards.All["CS2_172"], Cards.All["CS2_124"], Cards.All["CS2_182"],
            //    Cards.All["CS2_222"], Cards.All["OG_279"]
            //};
        }
    }
}