using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HearthStoneSimGui.View
{
    /// <summary>
    /// Description for CardOnTableView.
    /// </summary>
    public partial class HeroView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CardOnTableView class.
        /// </summary>
        public HeroView()
        {
            InitializeComponent();
        }

        private bool _animationInProgress;

        public static DependencyProperty AnimationCompleatedCommandProperty =
            DependencyProperty.Register(
                "AnimationCompleatedCommand",
                typeof(ICommand),
                typeof(HeroView),
                new PropertyMetadata(null));

        public ICommand AnimationCompleatedCommand
        {
            get => (ICommand)GetValue(AnimationCompleatedCommandProperty);

            set => SetValue(AnimationCompleatedCommandProperty, value);
        }

        private void DamageAnimation_OnCompleted(object sender, EventArgs e)
        {
            if (_animationInProgress)
            {
                _animationInProgress = false;
                AnimationCompleatedCommand.Execute(null);
            }
        }

        private void DamageAnimation_OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            var clock = (ClockGroup)sender;
            if (clock.CurrentState == ClockState.Active && !_animationInProgress)
            {
                _animationInProgress = true;
            }
        }
    }
}