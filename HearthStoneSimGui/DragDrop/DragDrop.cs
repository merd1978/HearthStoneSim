using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Linq;

namespace HearthStoneSimGui.DragDrop
{
    public static partial class DragDrop
    {
        private static DragInfo _dragInfo;
        private static Point _dragStartPosition;        //drag start point relative to the RootElement
        private static bool _dragInProgress;
        private static bool _clickToDrag;               //drag card by click (mouse button not pressed during drag)
        private static Point _adornerPos;
        private static Size _adornerSize;
        private static UIElement _rootElement;
        private static UIElement RootElement
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
        private const double DragAdornerScale = 1.3;    //scale sourse size up to 130%
        private static TargetPointerAdorner _targetPointerAdorner;
        private static TargetPointerAdorner TargetPointerAdorner
        {
            get => _targetPointerAdorner;
            set
            {
                _targetPointerAdorner?.Detatch();
                _targetPointerAdorner = value;
            }
        }

        private static void CreateDragAdorner()
        {
            DataTemplate template = GetDragAdornerTemplate(_dragInfo.VisualSource);
            UIElement adornment = null;

            if (template == null)
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

            if (template != null)
            {

                var contentPresenter = new ContentPresenter
                {
                    Content = _dragInfo.Data,
                    ContentTemplate = template,
                    Height = _dragInfo.VisualSource.RenderSize.Height * DragAdornerScale
                };

                adornment = contentPresenter;
            }

            if (adornment != null)
            {
                DragAdorner = new DragAdorner(_rootElement, adornment);
            }
        }

        private static void CreateTargetPointerAdorner()
        {
            TargetPointerAdorner = new TargetPointerAdorner(_rootElement, _dragStartPosition);
        }

        // Helper to generate the image - I grabbed this off Google 
        // somewhere. -- Chris Bordeman cbordeman@gmail.com
        private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection)
        {
            if (target == null) return null;

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

        /// <summary>
        /// Gets the drag handler from the drag info or from the sender, if the drag info is null
        /// </summary>
        /// <param name="dragInfo">the drag info object</param>
        /// <param name="sender">the sender from an event, e.g. mouse down, mouse move</param>
        /// <returns></returns>
        private static IDragSource TryGetDragHandler(DragInfo dragInfo, UIElement sender)
        {
            IDragSource dragHandler = null;
            if (dragInfo?.VisualSource != null)
            {
                dragHandler = GetDragHandler(dragInfo.VisualSource);
            }
            if (dragHandler == null && sender != null)
            {
                dragHandler = GetDragHandler(sender);
            }
            return dragHandler ?? DefaultDragHandler;
        }

        /// <summary>
        /// Gets the drop handler from the drop info or from the sender, if the drop info is null
        /// </summary>
        /// <param name="dropInfo">the drop info object</param>
        /// <param name="sender">the sender from an event, e.g. drag over</param>
        /// <returns></returns>
        private static IDropTarget TryGetDropHandler(DropInfo dropInfo, UIElement sender)
        {
            IDropTarget dropHandler = null;
            if (dropInfo?.VisualTarget != null)
            {
                dropHandler = GetDropHandler(dropInfo.VisualTarget);
            }
            if (dropHandler == null && sender != null)
            {
                dropHandler = GetDropHandler(sender);
            }
            return dropHandler ?? DefaultDropHandler;
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
            //get center of VisualSourceItem related to RootElement
            var visualSourceItem = _dragInfo.VisualSourceItem as FrameworkElement;
            if (visualSourceItem != null)
            {
                var visualSourceItemCenter = new Point(visualSourceItem.ActualWidth / 2, visualSourceItem.ActualHeight / 2);
                _dragStartPosition = visualSourceItem.TransformToAncestor(RootElement).Transform(visualSourceItemCenter);
            }

            if (_dragInfo.VisualSourceItem == null)
            {
                _dragInfo = null;
                return;
            }

            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
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

            // current mouse position
            var position = e.GetPosition((IInputElement)sender);

            // prevent selection changing while drag operation
            _dragInfo.VisualSource?.ReleaseMouseCapture();

            // only if the sender is the source control and the mouse point differs from an offset
            if (_dragInfo.VisualSource == sender
                     && (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
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
                    _clickToDrag = e.LeftButton != MouseButtonState.Pressed;
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
        }

        // Enables the drag source to determine whether the drag-and-drop operation should be canceled.
        private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            /*The QueryContinueDrag event is raised continuously while the drag source is being dragged.
             * You can handle this event to determine what action ends the drag-and-drop operation based on the state of the ESC, SHIFT, CTRL,
             * and ALT keys, as well as the state of the mouse buttons.The default handler for this event cancels the drag-and-drop operation
             * if the ESC key is pressed, and drops the data if the mouse button is released.If you handle this event to change the default
             * behavior, be sure to provide an equivalent mechanism in your handler to end the drag-and-drop operation.Otherwise, the DoDragDrop
             * method will not return and your application will stop responding. If you handle this event, you must mark it as handled to prevent
             * the default behavior from overriding your handler.*/
            e.Handled = true;

            // Now, default to actually continue drag
            e.Action = DragAction.Continue;

            if (e.EscapePressed || (e.KeyStates & DragDropKeyStates.RightMouseButton) != DragDropKeyStates.None)
            {
                DragAdorner = null;
                TargetPointerAdorner = null;
                Mouse.OverrideCursor = null;
                _dragInfo = null;
                e.Action = DragAction.Cancel;
                return;
            }

            bool leftMouseButtonPressed = (e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.None;

            //normal drag drop operation
            if (!_clickToDrag && !leftMouseButtonPressed)
            {
                e.Action = DragAction.Drop;
            }

            //click to drag mode
            if (_clickToDrag && leftMouseButtonPressed)
            {
                e.Action = DragAction.Drop;
            }
        }

        private static void DropTargetOnDragEnter(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e);
        }

        private static void DropTargetOnDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            TargetPointerAdorner = null;

            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
            dragHandler.DragLeave(sender);
        }

        private static void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, _dragInfo);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);

            dropHandler.DragOver(dropInfo);

            if (DragAdorner == null && _dragInfo != null)
            {
                var useDefaultDragAdorner = GetUseDefaultDragAdorner(_dragInfo.VisualSource);
                if (useDefaultDragAdorner)
                {
                    CreateDragAdorner();
                }
                else
                {
                    CreateTargetPointerAdorner();
                }
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
            }

            if (TargetPointerAdorner != null)
            {
                var tempAdornerPos = e.GetPosition(TargetPointerAdorner.AdornedElement);
                TargetPointerAdorner.EndPoint = tempAdornerPos;
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
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);

            DragAdorner = null;
            TargetPointerAdorner = null;

            dropHandler.DragOver(dropInfo);
            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = !dropInfo.NotHandled;
        }
    }
}
