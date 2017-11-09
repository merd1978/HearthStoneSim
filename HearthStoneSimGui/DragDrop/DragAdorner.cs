using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace HearthStoneSimGui.DragDrop
{
    internal class DragAdorner : Adorner, IAdorner
    {
        private readonly AdornerLayer _adornerLayer;
        private readonly UIElement _adornment;
        private Point _mousePosition;
        private Point _adornerPos;
        private Size _adornerSize;

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

        void IAdorner.Move(Point position)
        {
            var tempAdornerPos = position;

            if (tempAdornerPos.X >= 0 && tempAdornerPos.Y >= 0)
            {
                _adornerPos = tempAdornerPos;
            }

            // Check if we need  to force a measure / layout pass.
            // After invoking UpdateLayout method RenderSize will have the correct values
            if (RenderSize.Width.Equals(0.0))
            {
                UpdateLayout();
            }
            _adornerSize = RenderSize;

            // move the adorner
            var offsetX = _adornerSize.Width * -0.5;
            var offsetY = _adornerSize.Height * -0.5;
            _adornerPos.Offset(offsetX, offsetY);
            var maxAdornerPosX = AdornedElement.RenderSize.Width;
            var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
            if (adornerPosRightX > maxAdornerPosX)
            {
                _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
            }
            if (_adornerPos.Y < 0)
            {
                _adornerPos.Y = 0;
            }

            MousePosition = _adornerPos;
            InvalidateVisual();
        }

        void IAdorner.Detatch()
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