using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HearthStoneSimGui.DragDrop
{
    public static partial class DragDrop
    {
        private const double DragAdornerScale = 1.3;           //scale sourse size in DragAdorner

        public static int PreviewInsertIndex = -1;      //preview position where to insert element if the drop occurred, position unknown if -1
        /// <summary>
        /// Force using TargetPointer, ignoring UseDefaultAdorner
        /// </summary>
        public static bool SelectTargetForce;
        public static bool SelectTargetAfterDrop;
        public static UIElement LastDroppedSource;
        public static int LastDroppedIndex = -1;

        private static DragInfo _dragInfo;
        private static Point _dragStartPosition;        //drag start point relative to the RootElement
        private static Point? _lastDropPosition;        //used in DropTargetOnDragOver to prevent call handler when mouse doesnt move
        private static bool _dragInProgress;
        private static bool _clickToDrag;               //drag card by click (mouse button not pressed during drag)
        private static bool _previewMode;               //show zoomed card under mouse
        private static int _previewIndex;

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
        private static IAdorner _usedAdorner;
        private static IAdorner UsedAdorner
        {
            get => _usedAdorner;
            set
            {
                _usedAdorner?.Detatch();
                _usedAdorner = value;
            }
        }

        public static void AfterDrop()
        {
            if (SelectTargetAfterDrop)
            {
                // Wait for the rendering UI to be done. 
                // The rendering of the UI is perform on the UI thread via the Dispatcher. It can be considered as a tasks processer,
                // each of these task being assigned a priority.The rendering of the UI is one of these tasks and all you have to do is tell
                // the Dispatcher: “perform an action now with a priority less than the rendering”. The current work will then wait for the
                // rendering to be done.
                LastDroppedSource?.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                MouseButtonEventArgs arg =
                    new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                    {
                        RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                        Source = LastDroppedSource
                    };
                LastDroppedSource?.RaiseEvent(arg);

                SelectTargetAfterDrop = false;
            }
        }

        #region Adorners
        private static void CreateAdorner(double scale = DragAdornerScale, bool preview = false)
        {
            if (_dragInfo == null || UsedAdorner != null) return;
            var useDefaultDragAdorner = GetUseDefaultDragAdorner(_dragInfo.VisualSource);
            if (useDefaultDragAdorner && !SelectTargetForce || preview)
            {
                UsedAdorner = CreateDragAdorner(scale);
            }
            else UsedAdorner = CreateTargetPointerAdorner();
        }

        private static IAdorner CreateDragAdorner(double scale)
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
                    Height = _dragInfo.VisualSource.RenderSize.Height * scale
                };

                adornment = contentPresenter;
            }

            if (adornment != null)
            {
                var result = new DragAdorner(RootElement, adornment);
                return result;
            }

            return null;
        }

        private static IAdorner CreateTargetPointerAdorner()
        {
            return new TargetPointerAdorner(RootElement, _dragStartPosition);
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
        #endregion

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

            _previewMode = false;

            _dragInfo = new DragInfo(sender, e);

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

            //get center of VisualSourceItem relative to RootElement
            if (_dragInfo.VisualSourceItem is FrameworkElement visualSourceItem)
            {
                var visualSourceItemCenter = new Point(visualSourceItem.ActualWidth / 2, visualSourceItem.ActualHeight / 2);
                _dragStartPosition = visualSourceItem.TransformToAncestor(RootElement).Transform(visualSourceItemCenter);
            }

            UsedAdorner = null;
            _dragInfo.Data = _dragInfo.SourceItem;
            CreateAdorner();
            UsedAdorner?.Move(e.GetPosition(UsedAdorner.AdornedElement));
        }

        private static void DragSourceOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            UsedAdorner = null;
            _dragInfo = null;
        }

        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragInProgress) return;

            #region PreviewMode

            if (_dragInfo == null || _previewMode)
            {
                _dragInfo = new DragInfo(sender, e);
                if (GetUsePreview(_dragInfo.VisualSource) == false || _dragInfo.VisualSourceItem == null)
                {
                    UsedAdorner = null;
                    _dragInfo = null;
                    _previewMode = false;
                    return;
                }

                _dragInfo.Data = _dragInfo.SourceItem;
                double previewScale = GetPreviewScale(_dragInfo.VisualSource);

                if (UsedAdorner == null)
                {
                    CreateAdorner(previewScale, true);
                    _previewIndex = _dragInfo.SourceIndex;
                }
                if (_dragInfo.SourceIndex != _previewIndex)
                {
                    UsedAdorner = null;
                    CreateAdorner(previewScale, true);
                    _previewIndex = _dragInfo.SourceIndex;
                }
                if (_dragInfo.VisualSourceItem is FrameworkElement visualSourceItem)
                {
                    var visualSourceItemCenter = new Point(visualSourceItem.ActualWidth / 2, visualSourceItem.ActualHeight / 2);
                    //calculate center position of preview adorner, transform coordinates relative to RootElement
                    var previewCenter = visualSourceItem.TransformToAncestor(RootElement).Transform(visualSourceItemCenter);
                    if (GetPreviewHorizontalAlignmentn(_dragInfo.VisualSource) == HorizontalAlignment.Right)
                    {
                        previewCenter.X += visualSourceItem.ActualWidth / 2 * (1 + previewScale);

                    }
                    else if (GetPreviewHorizontalAlignmentn(_dragInfo.VisualSource) == HorizontalAlignment.Center)
                    {
                        if (previewCenter.Y + visualSourceItem.ActualHeight * previewScale / 2 > RootElement.RenderSize.Height)
                            previewCenter.Y = RootElement.RenderSize.Height - visualSourceItem.ActualHeight * previewScale / 2;
                    }
                    UsedAdorner?.Move(previewCenter);
                }
                _previewMode = true;
            }
            
            #endregion

            // the start from the source
            var dragStart = _dragInfo.DragStartPosition;

            // current mouse position
            var position = e.GetPosition((IInputElement)sender);

            // prevent selection changing while drag operation
            _dragInfo.VisualSource?.ReleaseMouseCapture();

            // only if the sender is the source control and the mouse point differs from an offset
            if (Equals(_dragInfo.VisualSource, sender)
                     && (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
                if (!dragHandler.CanStartDrag(_dragInfo)) return;

                dragHandler.StartDrag(_dragInfo);

                //if SelectTargetForce replace adorner with TargetPointer
                if (SelectTargetForce)
                {
                    UsedAdorner = null;
                    CreateAdorner();
                    UsedAdorner?.Move(e.GetPosition(UsedAdorner.AdornedElement));
                }

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

                DragDropEffects result = DragDropEffects.None;
                try
                {
                    _dragInProgress = true;
                    _clickToDrag = e.LeftButton != MouseButtonState.Pressed;
                    result = System.Windows.DragDrop.DoDragDrop(_dragInfo.VisualSource, data, _dragInfo.Effects);
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
                if (result != DragDropEffects.None) AfterDrop();
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
                UsedAdorner = null;
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
            UsedAdorner = null;

            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
            dragHandler.DragLeave(sender);

            PreviewInsertIndex = -1;
            //System.Diagnostics.Debug.WriteLine($"==> DragDrop: Leave");
        }

        //While dragging an element, the DragOver event fires every 350 milliseconds, even when mouse doesn't move
        private static void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, _dragInfo);

            if (_lastDropPosition.HasValue && dropInfo.DropPosition == _lastDropPosition)
            {
                e.Handled = true;
                return;
            }

            _lastDropPosition = dropInfo.DropPosition;

            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);

            //System.Diagnostics.Debug.WriteLine($"==> DragDrop: OnDragOver, pos={dropInfo.DropPosition}");
            dropHandler.DragOver(dropInfo);

            CreateAdorner();
            UsedAdorner?.Move(e.GetPosition(UsedAdorner.AdornedElement));

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

            UsedAdorner = null;

            dropHandler.DragOver(dropInfo);
            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = !dropInfo.NotHandled;

            LastDroppedIndex = PreviewInsertIndex;
            LastDroppedSource = sender as UIElement;
        }
    }
}
