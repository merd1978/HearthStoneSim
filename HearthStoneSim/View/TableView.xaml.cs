using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using HearthStoneSim.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HearthStoneSim.View
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : UserControl
    {
        //private TargetPointerAdorner _targetPointer;
        private static TargetPointerAdorner _dragAdorner;
        private static TargetPointerAdorner DragAdorner
        {
            get { return _dragAdorner; }
            set
            {
                _dragAdorner?.Detatch();
                _dragAdorner = value;
            }
        }
        private static DragInfo m_DragInfo;
        private static bool m_DragInProgress;
        private static Point _adornerPos;
        private static Size _adornerSize;
        public static IDropTarget DefaultDropHandler { get; } = new DefaultDropHandler();
        public static IDragSource DefaultDragHandler { get; } = new DefaultDragHandler();

        public TableView()
        {
            InitializeComponent();
        }

        private void TableViewLoaded(object sender, EventArgs e)
        {
        }
                
        private void CreateDragAdorner(DropInfo dropInfo)
        {
            var rootElement = RootElementFinder.FindRoot(dropInfo.VisualTarget ?? m_DragInfo.VisualSource);
            DragAdorner = new TargetPointerAdorner(rootElement);
        }

        private void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if clickCount != 1 or source is ignored.
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1)
                //|| (sender as UIElement).IsDragSourceIgnored()
                //|| (e.Source as UIElement).IsDragSourceIgnored()
                //|| (e.OriginalSource as UIElement).IsDragSourceIgnored()
                //|| HitTestUtilities.IsNotPartOfSender(sender, e))
            {
                m_DragInfo = null;
                return;
            }

            m_DragInfo = new DragInfo(sender, e);

            if (m_DragInfo.VisualSourceItem == null)
            {
                m_DragInfo = null;
                return;
            }

            var dragHandler = DefaultDragHandler;
            if (!dragHandler.CanStartDrag(m_DragInfo))
            {
                m_DragInfo = null;
                return;
            }
        }

        private void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_DragInfo == null) return;
            if (!m_DragInProgress)
            {

                // the start from the source
                var dragStart = m_DragInfo.DragStartPosition;

                // do nothing if mouse left button is released or the pointer is captured
                if (m_DragInfo.MouseButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
                {
                    m_DragInfo = null;
                    return;
                }

                // current mouse position
                var position = e.GetPosition((IInputElement)sender);

                // prevent selection changing while drag operation
                m_DragInfo.VisualSource?.ReleaseMouseCapture();

                // only if the sender is the source control and the mouse point differs from an offset
                if (m_DragInfo.VisualSource == sender
                    && (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    var dragHandler = DefaultDragHandler;
                    if (dragHandler.CanStartDrag(m_DragInfo))
                    {
                        dragHandler.StartDrag(m_DragInfo);
                    }
                }

                m_DragInProgress = true;
            }
            else
            {
                //var dropInfo = new DropInfo(sender, e, m_DragInfo);
                if (DragAdorner == null && m_DragInfo != null)
                {
                    CreateDragAdorner(dropInfo);
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

                    if (m_DragInfo != null)
                    {
                        // move the adorner
                        var offsetX = _adornerSize.Width;
                        var offsetY = _adornerSize.Height;
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

                    DragAdorner.DrawSelection(sender, e, _anchorPoint);

                    DragAdorner.MousePosition = _adornerPos;
                    DragAdorner.InvalidateVisual();
                }

                e.Effects = dropInfo.Effects;
                e.Handled = !dropInfo.NotHandled;

                if (!dropInfo.IsSameDragDropContextAsSource)
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void DoMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_DragInfo = null;
        }

        private void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);

            var dropInfo = new DropInfo(sender, e, m_DragInfo);
            var dropHandler = DefaultDropHandler;
            var itemsControl = dropInfo.VisualTarget;

            dropHandler.DragOver(dropInfo);

            if (DragAdorner == null && m_DragInfo != null)
            {
                CreateDragAdorner(dropInfo);
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

                if (m_DragInfo != null)
                {
                    // move the adorner
                    var offsetX = _adornerSize.Width;
                    var offsetY = _adornerSize.Height;
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

                DragAdorner.DrawSelection(sender, e, _anchorPoint);

                DragAdorner.MousePosition = _adornerPos;
                DragAdorner.InvalidateVisual();
            }

            e.Effects = dropInfo.Effects;
            e.Handled = !dropInfo.NotHandled;

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }
}


