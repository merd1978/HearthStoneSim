using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HearthStoneSim.DragDrop
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
            uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
            uiElement.PreviewMouseMove += DragSourceOnMouseMove;
            uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
         }
         else
         {
            uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
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
   }
}