using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HearthStoneSim.DragDrop
{
    public static partial class DragDrop
    {
        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty
          = DependencyProperty.RegisterAttached("IsDragSource",
                                                typeof(bool),
                                                typeof(DragDrop),
                                                new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsDragSource(UIElement target)
        {
          return (bool)target.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsDragSource(UIElement target, bool value)
        {
          target.SetValue(IsDragSourceProperty, value);
        }

    private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSourceOnMouseMove;
                //uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourceOnMouseMove;
                //uiElement.QueryContinueDrag -= DragSourceOnQueryContinueDrag;
            }
        }

        /// <summary>
        /// Gets or Sets whether the control can be used as drop target.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty
          = DependencyProperty.RegisterAttached("IsDropTarget",
                                                typeof(bool),
                                                typeof(DragDrop),
                                                new UIPropertyMetadata(false, IsDropTargetChanged));

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}