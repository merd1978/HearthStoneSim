using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Linq;

namespace HearthStoneSim.DragDrop
{
   public static partial class DragDrop
   {
      private static DragInfo _dragInfo;
      private static Point _dragStartPosition;     //drag start point relative to the RootElement
      private static bool _dragInProgress;
      private static Point _adornerPos;
      private static Size _adornerSize;
      private static UIElement _rootElement;
      public static UIElement RootElement
      {
         get
         {
            if (_rootElement != null) return _rootElement;
            var parentWindow = Window.GetWindow(_dragInfo.VisualSource);
            _rootElement = parentWindow?.Content as UIElement;
            return _rootElement;
         }
      }
      private static DragAdorner _dragAdorner;
      private static DragAdorner DragAdorner
      {
         get => _dragAdorner;
         set
         {
            _dragAdorner?.Detatch();
            _dragAdorner = value;
         }
      }
      public static IDropTarget DefaultDropHandler { get; } = new DefaultDropHandler();
      public static IDragSource DefaultDragHandler { get; } = new DefaultDragHandler();

      private static void CreateDragAdorner()
      {
         DataTemplate template = null;

         var useDefaultDragAdorner = GetUseDefaultDragAdorner(_dragInfo.VisualSource);

         if (useDefaultDragAdorner)
         {
            var bs = CaptureScreen(_dragInfo.VisualSourceItem, _dragInfo.VisualSourceFlowDirection);
            if (bs != null)
            {
               var factory = new FrameworkElementFactory(typeof(Image));
               factory.SetValue(Image.SourceProperty, bs);
               factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
               factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
               factory.SetValue(FrameworkElement.WidthProperty, bs.Width);
               factory.SetValue(FrameworkElement.HeightProperty, bs.Height);
               factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
               factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
               template = new DataTemplate { VisualTree = factory };
            }
         }

         var contentPresenter = new ContentPresenter
         {
            Content = _dragInfo.Data,
            ContentTemplate = template
         };

         UIElement adornment = contentPresenter;

         DragAdorner = new DragAdorner(_rootElement, adornment);
   
         //DragAdorner = new TargetPointerAdorner(_rootElement, _dragStartPosition);
      }

      // Helper to generate the image - I grabbed this off Google 
      // somewhere. -- Chris Bordeman cbordeman@gmail.com
      private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection)
      {
         if (target == null)
         {
            return null;
         }

         var dpiX = DpiHelper.DpiX;
         var dpiY = DpiHelper.DpiY;

         var bounds = VisualTreeHelper.GetDescendantBounds(target);
         var dpiBounds = DpiHelper.LogicalRectToDevice(bounds);

         var pixelWidth = (int)Math.Ceiling(dpiBounds.Width);
         var pixelHeight = (int)Math.Ceiling(dpiBounds.Height);
         if (pixelWidth < 0 || pixelHeight < 0)
         {
            return null;
         }

         var rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);

         var dv = new DrawingVisual();
         using (var ctx = dv.RenderOpen())
         {
            var vb = new VisualBrush(target);
            if (flowDirection == FlowDirection.RightToLeft)
            {
               var transformGroup = new TransformGroup();
               transformGroup.Children.Add(new ScaleTransform(-1, 1));
               transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width - 1, 0));
               ctx.PushTransform(transformGroup);
            }
            ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
         }

         rtb.Render(dv);

         return rtb;
      }

      private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         // Ignore the click if clickCount != 1 or source is ignored.
         if (e.ClickCount != 1
            || (sender as UIElement).IsDragSourceIgnored()
            || (e.Source as UIElement).IsDragSourceIgnored()
            || (e.OriginalSource as UIElement).IsDragSourceIgnored())
         {
            _dragInfo = null;
            return;
         }

         _dragInfo = new DragInfo(sender, e);
         _dragStartPosition = e.GetPosition(RootElement);


         if (_dragInfo.VisualSourceItem == null)
         {
            _dragInfo = null;
            return;
         }

         var dragHandler = DefaultDragHandler;
         if (!dragHandler.CanStartDrag(_dragInfo))
         {
            _dragInfo = null;
            return;
         }
      }

      private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
      {
         if (_dragInfo == null || _dragInProgress) return;
         // the start from the source
         var dragStart = _dragInfo.DragStartPosition;

         // do nothing if mouse left button is released or the pointer is captured
         if (_dragInfo.MouseButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
         {
            _dragInfo = null;
            return;
         }

         // current mouse position
         var position = e.GetPosition((IInputElement)sender);

         // prevent selection changing while drag operation
         _dragInfo.VisualSource?.ReleaseMouseCapture();

         // only if the sender is the source control and the mouse point differs from an offset
         if (_dragInfo.VisualSource == sender
                  && (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance))
         {
            var dragHandler = DefaultDragHandler;
            if (!dragHandler.CanStartDrag(_dragInfo)) return;

            dragHandler.StartDrag(_dragInfo);

            if (_dragInfo.Effects == DragDropEffects.None || _dragInfo.Data == null) return;

            var data = _dragInfo.DataObject;

            if (data == null)
            {
               data = new DataObject(DataFormat.Name, _dragInfo.Data);
            }
            else
            {
               data.SetData(DataFormat.Name, _dragInfo.Data);
            }

            try
            {
               _dragInProgress = true;
               var result = System.Windows.DragDrop.DoDragDrop(_dragInfo.VisualSource, data, _dragInfo.Effects);
               if (result == DragDropEffects.None)
                  dragHandler.DragCancelled();
            }
            catch (Exception ex)
            {
               if (!dragHandler.TryCatchOccurredException(ex))
               {
                  throw;
               }
            }
            finally
            {
               _dragInProgress = false;
            }

            _dragInfo = null;
         }
      }

      private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         _dragInfo = null;
      }

      private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
      {
         if (e.Action == DragAction.Cancel || e.EscapePressed)
         {
            DragAdorner = null;
            //DropTargetAdorner = null;
            Mouse.OverrideCursor = null;
         }
      }

      private static void DropTargetOnDragEnter(object sender, DragEventArgs e)
      {
         DropTargetOnDragOver(sender, e);
      }

      private static void DropTargetOnDragLeave(object sender, DragEventArgs e)
      {
         DragAdorner = null;
         //DropTargetAdorner = null;
      }

      private static void DropTargetOnDragOver(object sender, DragEventArgs e)
      {
         var dropInfo = new DropInfo(sender, e, _dragInfo);
         var dropHandler = DefaultDropHandler;

         dropHandler.DragOver(dropInfo);

         if (DragAdorner == null && _dragInfo != null)
         {
            CreateDragAdorner();
         }

         if (DragAdorner != null)
         {
            var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

            if (tempAdornerPos.X >= 0 && tempAdornerPos.Y >= 0)
            {
               _adornerPos = tempAdornerPos;
            }

            // Fixed the flickering adorner - Size changes to zero 'randomly'...?
            if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
            {
               _adornerSize = DragAdorner.RenderSize;
            }

            if (_dragInfo != null)
            {
               // move the adorner
               var offsetX = _adornerSize.Width * -0.5;
               var offsetY = _adornerSize.Height * -0.5;
               _adornerPos.Offset(offsetX, offsetY);
               var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
               var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
               if (adornerPosRightX > maxAdornerPosX)
               {
                  _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
               }
               if (_adornerPos.Y < 0)
               {
                  _adornerPos.Y = 0;
               }
            }

            DragAdorner.MousePosition = _adornerPos;
            DragAdorner.InvalidateVisual();

            //DragAdorner.EndPoint = tempAdornerPos;
         }
         e.Effects = dropInfo.Effects;
         e.Handled = !dropInfo.NotHandled;
      }

      private static void DropTargetOnGiveFeedback(object sender, GiveFeedbackEventArgs e)
      {
         e.UseDefaultCursors = false;
         e.Handled = true;
         if (Mouse.OverrideCursor != Cursors.Arrow)
         {
            Mouse.OverrideCursor = Cursors.Arrow;
         }
      }

      private static void DropTargetOnDrop(object sender, DragEventArgs e)
      {
         var dropInfo = new DropInfo(sender, e, _dragInfo);
         var dropHandler = DefaultDropHandler;
         var dragHandler = DefaultDragHandler;

         DragAdorner = null;
         //DropTargetAdorner = null;

         dropHandler.DragOver(dropInfo);
         dropHandler.Drop(dropInfo);
         dragHandler.Dropped(dropInfo);

         Mouse.OverrideCursor = null;
         e.Handled = !dropInfo.NotHandled;
      }
   }
}
