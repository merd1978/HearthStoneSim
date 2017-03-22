using HearthStoneSim.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace HearthStoneSim.View
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : UserControl
    {
        private TargetPointerAdorner _targetPointer;

        public TableView()
        {
            InitializeComponent();
        }

        private void TableViewLoaded(object sender, EventArgs e)
        {
            var layer = AdornerLayer.GetAdornerLayer(TableViewListBox);
            _targetPointer = new TargetPointerAdorner((IInputElement)sender);
            layer.Add(_targetPointer);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var anchor = e.GetPosition((IInputElement)sender);
            _targetPointer.CaptureMouse();
            _targetPointer.StartSelection(anchor);
        }
    }
}


