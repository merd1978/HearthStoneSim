using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HearthStoneSimGui.DragDrop
{
    internal class TargetPointerAdorner : Adorner, IAdorner
    {
        private readonly AdornerLayer _adornerLayer;
        private const double ArrowLength = 12;
        private const double ArrowAngle = 45;
        private readonly Point _startPoint;
        private readonly Point _endPoint;
        private readonly PathGeometry _pathgeo;
        private readonly PathFigure _pathfigLine;
        private readonly PolyLineSegment _polysegLine;
        private readonly PathFigure _pathfigHead2;
        protected override int VisualChildrenCount => 1;
        private readonly Path _rubberband;
        public Point EndPoint
        {
            get => _endPoint;
            set
            {
                if (_endPoint == value) return;

                // Clear out the PathGeometry.
                _pathgeo.Figures.Clear();

                // Define a single PathFigure with the points.
                _polysegLine.Points.Clear();
                _polysegLine.Points.Add(value);
                _pathgeo.Figures.Add(_pathfigLine);
                _pathgeo.Figures.Add(CalculateArrow(_pathfigHead2, _startPoint, value));

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

            _endPoint = _startPoint = startPoint;

            _pathgeo = new PathGeometry();
            _pathfigLine = new PathFigure();
            _polysegLine = new PolyLineSegment();
            _pathfigLine.Segments.Add(_polysegLine);

            _pathfigHead2 = new PathFigure();
            var polysegHead2 = new PolyLineSegment();
            _pathfigHead2.Segments.Add(polysegHead2);

            _pathfigLine.StartPoint = _startPoint;
            _pathgeo.Figures.Add(_pathfigLine);

            _rubberband = new Path
            {
                Data = _pathgeo,
                StrokeThickness = 10,
                Stroke = Brushes.Red,
                Opacity = .7,
            };
            AddVisualChild(_rubberband);
        }

        void IAdorner.Move(Point position)
        {
            var tempAdornerPos = position;
            EndPoint = tempAdornerPos;
        }

        private static PathFigure CalculateArrow(PathFigure pathfig, Point pt1, Point pt2)
        {
            Matrix matx = new Matrix();
            Vector vect = pt1 - pt2;
            vect.Normalize();
            vect *= ArrowLength;

            PolyLineSegment polyseg = pathfig.Segments[0] as PolyLineSegment;
            polyseg.Points.Clear();
            matx.Rotate(ArrowAngle / 2);
            pathfig.StartPoint = pt2 + vect * matx;
            polyseg.Points.Add(pt2);

            matx.Rotate(-ArrowAngle);
            polyseg.Points.Add(pt2 + vect * matx);
            pathfig.IsClosed = false;

            return pathfig;
        }

        void IAdorner.Detatch()
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