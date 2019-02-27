using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HearthStoneSimGui.View
{
    public class RadialPanel : Panel
    {
        // This Panel lays its children out in a circle
        // keeping the angular distance from each child
        // equal; MeasureOverride is called before ArrangeOverride.

        protected override Size MeasureOverride(Size availableSize)
        {
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
            
            //если карты не помещаются по ширине, размещаем их в секторе окружности
            if (sumWidth > finalSize.Width)
            {
                // Шаг угла поворота элементов
                double stepAngle = maxAngle / Children.Count;

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
                var zeroTransform = new RotateTransform(0);
                double leftMargin = (finalSize.Width - sumWidth) / 2;
                childPointX += leftMargin;
                foreach (UIElement uie in Children)
                {
                    uie.RenderTransform = zeroTransform;        //поворачиваем вертикально, т.к элементы могли быть уже повернуты
                    uie.Arrange(new Rect(new Point(childPointX, childPointY), new Size(uie.DesiredSize.Width, uie.DesiredSize.Height)));
                    childPointX += uie.DesiredSize.Width + margin;
                }
            }

            return finalSize;
        }
    }
}
