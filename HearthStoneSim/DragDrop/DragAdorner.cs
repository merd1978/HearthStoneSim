using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace HearthStoneSim.DragDrop
{
   internal class DragAdorner : Adorner
   {
      private readonly AdornerLayer _adornerLayer;
      private readonly UIElement _adornment;
      private Point _mousePosition;

      public DragAdorner(UIElement adornedElement, UIElement adornment, DragDropEffects effects = DragDropEffects.None)
      : base(adornedElement)
      {
         _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
         _adornerLayer.Add(this);
         _adornment = adornment;
         IsHitTestVisible = false;
         Effects = effects;
      }

      public DragDropEffects Effects { get; private set; }

      public Point MousePosition
      {
         get => _mousePosition;
         set
         {
            if (_mousePosition == value) return;
            _mousePosition = value;
            _adornerLayer.Update(AdornedElement);
         }
      }

      public void Detatch()
      {
         _adornerLayer.Remove(this);
      }

      protected override Size ArrangeOverride(Size finalSize)
      {
         _adornment.Arrange(new Rect(finalSize));
         return finalSize;
      }

      public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
      {
         var result = new GeneralTransformGroup();
         result.Children.Add(base.GetDesiredTransform(transform));
         result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

         return result;
      }

      protected override Visual GetVisualChild(int index)
      {
         return _adornment;
      }

      protected override Size MeasureOverride(Size constraint)
      {
         _adornment.Measure(constraint);
         return _adornment.DesiredSize;
      }

      protected override int VisualChildrenCount => 1;
   }
}