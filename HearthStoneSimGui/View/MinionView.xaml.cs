using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HearthStoneSimGui.View
{
    /// <summary>
    /// Description for CardOnTableView.
    /// </summary>
    public partial class MinionView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CardOnTableView class.
        /// </summary>
        public MinionView()
        {
            InitializeComponent();
        }

        private bool _animationInProgress;

        public static readonly RoutedEvent AnimationCompleatedEvent =
            EventManager.RegisterRoutedEvent("AnimationCompleatedEvent", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(MinionView));

        public event RoutedEventHandler AnimationCompleated
        {
            add => AddHandler(AnimationCompleatedEvent, value);
            remove => RemoveHandler(AnimationCompleatedEvent, value);
        }

        private void DamageAnimation_OnCompleted(object sender, EventArgs e)
        {
            if (_animationInProgress)
            {
                _animationInProgress = false;
                RaiseEvent(new RoutedEventArgs(AnimationCompleatedEvent));
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