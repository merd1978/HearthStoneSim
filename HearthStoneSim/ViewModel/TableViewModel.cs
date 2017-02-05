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
    public class TableViewModel : ViewModelBase
    {
        public ObservableCollection<Card> Cards { get; private set; }
        /// <summary>
        /// Initializes a new instance of the TableViewModel class.
        /// </summary>
        public TableViewModel()
        {
            Cards = new ObservableCollection<Card> {  };
        }
    }
}