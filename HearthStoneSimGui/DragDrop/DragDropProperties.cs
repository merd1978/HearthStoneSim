using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HearthStoneSimGui.DragDrop
{
    public static partial class DragDrop
    {
        public static DataFormat DataFormat { get; } = DataFormats.GetDataFormat("HearthStoneSim.DragDrop");

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
                uiElement.PreviewMouseRightButtonDown += DragSourceOnMouseRightButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSourceOnMouseMove;
                uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseRightButtonDown -= DragSourceOnMouseRightButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourceOnMouseMove;
                uiElement.QueryContinueDrag -= DragSourceOnQueryContinueDrag;
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

        /// <summary>
        /// Gets whether the control can be used as drop target.
        /// </summary>
        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drop target.
        /// </summary>
        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;

                // use normal events for ItemsControls
                uiElement.DragEnter += DropTargetOnDragEnter;
                uiElement.DragLeave += DropTargetOnDragLeave;
                uiElement.DragOver += DropTargetOnDragOver;
                uiElement.Drop += DropTargetOnDrop;
                uiElement.GiveFeedback += DropTargetOnGiveFeedback;
            }
            else
            {
                uiElement.AllowDrop = false;

                uiElement.DragEnter -= DropTargetOnDragEnter;
                uiElement.DragLeave -= DropTargetOnDragLeave;
                uiElement.DragOver -= DropTargetOnDragOver;
                uiElement.Drop -= DropTargetOnDrop;
                uiElement.GiveFeedback -= DropTargetOnGiveFeedback;

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Gets or Sets whether an element under the mouse should be ignored for the drag action.
        /// </summary>
        public static readonly DependencyProperty DragSourceIgnoreProperty
           = DependencyProperty.RegisterAttached("DragSourceIgnore",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets whether an element under the mouse should be ignored for the drag action.
        /// </summary>
        public static bool GetDragSourceIgnore(UIElement source)
        {
            return (bool)source.GetValue(DragSourceIgnoreProperty);
        }

        /// <summary>
        /// Sets whether an element under the mouse should be ignored for the drag action.
        /// </summary>
        public static void SetDragSourceIgnore(UIElement source, bool value)
        {
            source.SetValue(DragSourceIgnoreProperty, value);
        }

        /// <summary>
        /// Gets or Sets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
        /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelOrientationProperty
           = DependencyProperty.RegisterAttached("ItemsPanelOrientation",
                                                  typeof(Orientation?),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(null));

        /// <summary>
        /// Gets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
        /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
        /// </summary>
        public static Orientation? GetItemsPanelOrientation(UIElement source)
        {
            return (Orientation?)source.GetValue(ItemsPanelOrientationProperty);
        }

        /// <summary>
        /// Sets the Orientation which should be used for the drag drop action (default null).
        /// Normally it will be look up to find the correct orientaion of the inner ItemsPanel,
        /// but sometimes it's necessary to force the oreintation, if the look up is wrong.
        /// </summary>
        public static void SetItemsPanelOrientation(UIElement source, Orientation? value)
        {
            source.SetValue(ItemsPanelOrientationProperty, value);
        }

        /// <summary>
        /// Gets or Sets whether if the default DragAdorner should be use.
        /// </summary>
        public static readonly DependencyProperty UseDefaultDragAdornerProperty
           = DependencyProperty.RegisterAttached("UseDefaultDragAdorner",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new PropertyMetadata(false));

        /// <summary>
        /// Gets whether if the default DragAdorner is used.
        /// </summary>
        public static bool GetUseDefaultDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultDragAdornerProperty);
        }

        /// <summary>
        /// Sets whether if the default DragAdorner should be use.
        /// </summary>
        public static void SetUseDefaultDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseDefaultDragAdornerProperty, value);
        }

        /// <summary>
        /// Gets or Sets a DataTemplate for the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateProperty
           = DependencyProperty.RegisterAttached("DragAdornerTemplate",
                                                  typeof(DataTemplate),
                                                  typeof(DragDrop));

        /// <summary>
        /// Gets the DataTemplate for the DragAdorner.
        /// </summary>
        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        /// <summary>
        /// Sets the DataTemplate for the DragAdorner.
        /// </summary>
        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        /// <summary>
        /// Gets the default DragHandler.
        /// </summary>
        public static IDragSource DefaultDragHandler { get; } = new DefaultDragHandler();

        /// <summary>
        /// Gets the default DropHandler.
        /// </summary>
        public static IDropTarget DefaultDropHandler { get; } = new DefaultDropHandler();

        /// <summary>
        /// Gets or Sets the handler for the drag action.
        /// </summary>
        public static readonly DependencyProperty DragHandlerProperty
           = DependencyProperty.RegisterAttached("DragHandler",
                                                  typeof(IDragSource),
                                                  typeof(DragDrop));

        /// <summary>
        /// Gets the handler for the drag action.
        /// </summary>
        public static IDragSource GetDragHandler(UIElement target)
        {
            return (IDragSource)target.GetValue(DragHandlerProperty);
        }

        /// <summary>
        /// Sets the handler for the drag action.
        /// </summary>
        public static void SetDragHandler(UIElement target, IDragSource value)
        {
            target.SetValue(DragHandlerProperty, value);
        }

        /// <summary>
        /// Gets or Sets the handler for the drop action.
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty
           = DependencyProperty.RegisterAttached("DropHandler",
                                                  typeof(IDropTarget),
                                                  typeof(DragDrop));

        /// <summary>
        /// Gets the handler for the drop action.
        /// </summary>
        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        /// <summary>
        /// Sets the handler for the drop action.
        /// </summary>
        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        /// <summary>
        /// Gets or Sets if the Preview mode should be used.
        /// </summary>
        public static readonly DependencyProperty UsePreviewProperty
            = DependencyProperty.RegisterAttached("UsePreview",
                typeof(bool),
                typeof(DragDrop),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets if the Preview mode should be used.
        /// </summary>
        public static bool GetUsePreview(UIElement target)
        {
            return (bool)target.GetValue(UsePreviewProperty);
        }

        /// <summary>
        /// Sets the Preview mode should be used
        /// </summary>
        public static void SetUsePreview(UIElement target, bool value)
        {
            target.SetValue(UsePreviewProperty, value);
        }
    }
}