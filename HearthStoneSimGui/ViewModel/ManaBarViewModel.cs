using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using HearthStoneSimCore.Model;

namespace HearthStoneSimGui.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class ManaBarViewModel : ViewModelBase
    {
        private ObservableCollection<bool> _manaBar;

        public ObservableCollection<bool> ManaBar
        {
            get => _manaBar;
            set => Set(nameof(ManaBar), ref _manaBar, value);
        }

        /// <summary>
        /// Total amount of mana available to this player.
        /// </summary>
        private int _baseMana;
        public int BaseMana
        {
            get => _baseMana;
            set => Set(nameof(BaseMana), ref _baseMana, value);
        }

        /// <summary>
        /// The amount of mana available to actually use.
        /// </summary>
        private int _remainingMana;
        public int RemainingMana
        {
            get => _remainingMana;
            set => Set(nameof(RemainingMana), ref _remainingMana, value);
        }

        public Controller Controller { get; }

        /// <summary>
        /// Initializes a new instance of the TableViewModel class.
        /// </summary>
        public ManaBarViewModel(Controller controller)
        {
            Controller = controller;
            UpdateState();
        }

        public ManaBarViewModel()
        {
#if DEBUG
            if (ViewModelBase.IsInDesignModeStatic)
            {
                BaseMana = 10;
                RemainingMana = 3;
                ManaBar = new ObservableCollection<bool> {true, true, true,
                    false, false, false, false, false, false, false};
            }
#endif
        }

        public void UpdateState()
        {
            BaseMana = Controller.BaseMana;
            RemainingMana = Controller.RemainingMana;

            if (_baseMana < 0 || _baseMana > 10) return;
            if (_remainingMana <0 || _remainingMana >10) return;

            ManaBar = new ObservableCollection<bool>();
            for (int i = 0; i < _remainingMana; i++)
            {
                ManaBar.Add(true);
            }
            for (int i = _remainingMana; i < _baseMana; i++)
            {
                ManaBar.Add(false);
            }
        }
    }
}
