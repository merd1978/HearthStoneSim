using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HearthStoneSim.DragDrop
{
  public class TargetPointerAdorner : Adorner
   {
      private readonly AdornerLayer _adornerLayer;
      private readonly LineGeometry _targetPointer;
      protected override int VisualChildrenCount => 1;
      private readonly Path _rubberband;
      public Point EndPoint
      {
         get => _targetPointer.EndPoint;
         set
         {
            if (_targetPointer.EndPoint == value) return;
            _targetPointer.EndPoint = value;
            //_adornerLayer.Update(AdornedElement);
            //_adornerLayer.InvalidateArrange();
         }
      }

      public TargetPointerAdorner(UIElement adornedElement, Point startPoint)
          : base(adornedElement)
      {
         IsHitTestVisible = false;
         AllowDrop = false;
         //SnapsToDevicePixels = true;
         _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
         _adornerLayer.Add(this);
         _targetPointer = new LineGeometry { StartPoint = startPoint };
         _rubberband = new Path
         {
            Data = _targetPointer,
            StrokeThickness = 10,
            Stroke = Brushes.Red,
            Opacity = .6,
         };
         AddVisualChild(_rubberband);
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

      protected override Visual GetVisualChild(int index) => _rubberband;
   }
}