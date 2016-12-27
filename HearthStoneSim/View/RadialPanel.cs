using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HearthStoneSim.View
{
    public class RadialPanel : Panel
    {
        // This Panel lays its children out in a circle
        // keeping the angular distance from each child
        // equal; MeasureOverride is called before ArrangeOverride.

        //double _maxChildHeight, _perimeter, _radius;

        protected override Size MeasureOverride(Size availableSize)
        {
            //_perimeter = 0;
            //_maxChildHeight = 0;

            //// Find the tallest child and determine the perimeter
            //// based on the width of all of the children after
            //// measuring all of the them and letting them size
            //// to content by passing Double.PositiveInfinity as
            //// the available size.

            //foreach (UIElement uie in Children)
            //{
            //    uie.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            //    _perimeter += uie.DesiredSize.Width;
            //    _maxChildHeight = Math.Max(_maxChildHeight, uie.DesiredSize.Height);
            //}

            //// If the marginal angle is not 0, 90 or 180
            //// then the adjustFactor is needed.

            //if (Children.Count > 2 && Children.Count != 4)
            //    _adjustFactor = 10;

            //// Determine the radius of the circle layout and determine
            //// the RadialPanel's Size.

            //_radius = _perimeter / (2 * Math.PI) + _adjustFactor;
            //double _squareSize = 2 * (_radius + _maxChildHeight);
            //return new Size(_squareSize, _squareSize);

            foreach (UIElement elem in Children)
            {

                //Give Infinite size as the avaiable size for all the children

                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }

        // размещаем дочерние элементы в размерах finalsize
        protected override Size ArrangeOverride(Size finalSize)
        {
            const double rad = Math.PI / 180,
                   maxAngle = 44,  //угол сектора окружности в который вписываются элементы (в градусах)
                   margin = 3;
            double childPointX = 0,
                childPointY = 0,
                centerX = finalSize.Width / 2,
                currentAngle = -maxAngle / 2,
                radius = centerX / Math.Tan(maxAngle * rad / 2),
                sumWidth = 0;
            
            if (Children.Count == 0) return finalSize;
            
            foreach (UIElement uie in Children) sumWidth += uie.DesiredSize.Width;
            sumWidth += margin * (Children.Count - 1);
            
            //если карты не помещаются по ширине, размещаем их в сектору окружности
            if (sumWidth > finalSize.Width)
            {
                // Шаг угла поворота элементов
                double stepAngle = maxAngle / (Children.Count);

                foreach (UIElement uie in Children)
                {
                    // координата левого верхнего угла каждого элемента (точка лежит на окружности радиусом _radius)
                    childPointX = Math.Cos((currentAngle - 90) * rad) * radius;
                    childPointY = Math.Sin((currentAngle - 90) * rad) * radius;

                    // вращаем элементы вокруг левого верхнего угла и задаем размещение
                    uie.RenderTransform = new RotateTransform(currentAngle);
                    uie.Arrange(new Rect(new Point(childPointX + centerX, childPointY + radius), new Size(uie.DesiredSize.Width, uie.DesiredSize.Height)));

                    currentAngle += stepAngle;
                }
            }
            //размещаем карты в ряд
            else
            {
                double leftMargin = (finalSize.Width - sumWidth) / 2;
                childPointX += leftMargin;
                foreach (UIElement uie in Children)
                {
                    uie.Arrange(new Rect(new Point(childPointX, childPointY), new Size(uie.DesiredSize.Width, uie.DesiredSize.Height)));
                    childPointX += uie.DesiredSize.Width + margin;
                }
            }

            return finalSize;
        }
    }
}
