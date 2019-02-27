using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HearthStoneSimGui.View
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        public BoardView()
        {
            InitializeComponent();
        }

        public static DependencyProperty AnimationCompleatedCommandProperty =
            DependencyProperty.Register(
                "AnimationCompleatedCommand",
                typeof(ICommand),
                typeof(BoardView),
                new PropertyMetadata(null));

        public ICommand AnimationCompleatedCommand
        {
            get => (ICommand)GetValue(AnimationCompleatedCommandProperty);

            set => SetValue(AnimationCompleatedCommandProperty, value);
        }

        private void MinionView_OnAnimationCompleated(object sender, RoutedEventArgs e)
        {
            AnimationCompleatedCommand?.Execute(null);
        }
    }
}


