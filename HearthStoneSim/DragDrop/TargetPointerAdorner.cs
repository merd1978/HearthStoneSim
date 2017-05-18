using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HearthStoneSim.DragDrop
{
   public class TargetPointerAdorner : Adorner
   {
      private readonly AdornerLayer _adornerLayer;
      private readonly LineGeometry _targetPointer;
      protected override int VisualChildrenCount => 1;
      public Path Rubberband { get; }
      public Point EndPoint
      {
         get => _targetPointer.EndPoint;
         set
         {
            if (_targetPointer.EndPoint == value) return;
            _targetPointer.EndPoint = value;
            _adornerLayer.Update(AdornedElement);
         }
      }

      public TargetPointerAdorner(UIElement adornedElement, Point startPoint)
          : base(adornedElement)
      {
         _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
         _adornerLayer.Add(this);
         _targetPointer = new LineGeometry {StartPoint = startPoint};
         Rubberband = new Path
         {
            Data = _targetPointer,
            StrokeThickness = 2,
            Stroke = Brushes.Yellow,
            Opacity = .6,
         };
         AddVisualChild(Rubberband);
      }

      public void Detatch()
      {
         _adornerLayer.Remove(this);
      }

      protected override Size ArrangeOverride(Size size)
      {
         var finalSize = base.ArrangeOverride(size);
         ((UIElement)GetVisualChild(0))?.Arrange(new Rect(finalSize));
         return finalSize;
      }

      private void EndSelection(object sender, MouseButtonEventArgs e)
      {
         ReleaseMouseCapture();
      }

      protected override Visual GetVisualChild(int index) => Rubberband;
   }
}