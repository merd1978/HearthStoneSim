using System.Windows;
using System.Windows.Controls;

namespace HearthStoneSimGui.View
{
    public class DeckPanel : Panel
    {
        // This Panel lays its children one above the other
        // MeasureOverride is called before ArrangeOverride.

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
            {

                //Give Infinite size as the avaiable size for all the children
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }

        // the child elements in finalsize
        protected override Size ArrangeOverride(Size finalSize)
        {
            const double margin = 1;
            double childPointX = finalSize.Width,
                childPointY = 0;

            if (Children.Count == 0) return finalSize;

            // place the cards one above the other with an offset
            else
            {
                foreach (UIElement uie in Children)
                {
                    uie.Arrange(new Rect(new Point(childPointX - uie.DesiredSize.Width, childPointY),
                        new Size(uie.DesiredSize.Width, uie.DesiredSize.Height)));
                    childPointX -= margin;
                }
            }

            return finalSize;
        }
    }
}
