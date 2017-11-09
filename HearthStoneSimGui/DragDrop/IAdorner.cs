using System.Windows;

namespace HearthStoneSimGui.DragDrop
{
    public interface IAdorner
    {
        void Move(Point position);
        void Detatch();
        UIElement AdornedElement { get; }
    }
}
