using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
   }
}