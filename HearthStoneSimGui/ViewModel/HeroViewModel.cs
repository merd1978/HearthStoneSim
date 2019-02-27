using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;
using HearthStoneSimGui.DragDrop;

namespace HearthStoneSimGui.ViewModel
{
    public class HeroViewModel : ViewModelBase, IDropTarget
    {
        public Game Game { get; }
        public Controller Controller { get; }


        public Hero Hero {get; }

        public RelayCommand AnimationCompleatedCommand { get; set; }
        private void ExecuteAnimationCompleatedCommand()
        {
            Game.ClearPreDamage(Controller);
            UpdateState();
        }

        public HeroViewModel(Hero hero)
        {
            Hero = hero;
            Controller = hero.Controller;
            Game = hero.Game;
            AnimationCompleatedCommand = new RelayCommand(ExecuteAnimationCompleatedCommand);
        }

        public void UpdateState()
        {
            RaisePropertyChanged(nameof(Hero));
        }

        public void DragOver(IDropInfo dropInfo)
        {
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is Minion sourceItem)) return;

            //Check is it attacking enemy minion
            if (sourceItem.Controller == Controller) return;
            if (sourceItem.Zone.Type == Zone.PLAY)
            {
                RaisePropertyChanged(nameof(Hero));
                Controller.MinionAttack(sourceItem, Hero);
            }
        }
    }
}
