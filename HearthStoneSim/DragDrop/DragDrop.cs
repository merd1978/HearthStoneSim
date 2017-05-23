using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HearthStoneSim.DragDrop
{
  public static partial class DragDrop
  {
    private static DragInfo _dragInfo;
    private static Point _dragStartPosition;
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
    private static TargetPointerAdorner _dragAdorner;
    private static TargetPointerAdorner DragAdorner
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
      DragAdorner = new TargetPointerAdorner(_rootElement, _dragStartPosition);
    }

    private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      // Ignore the click if clickCount != 1 or source is ignored.
      var elementPosition = e.GetPosition((IInputElement)sender);
      if (e.ClickCount != 1)
      //|| (sender as UIElement).IsDragSourceIgnored()
      //|| (e.Source as UIElement).IsDragSourceIgnored()
      //|| (e.OriginalSource as UIElement).IsDragSourceIgnored()
      //|| HitTestUtilities.IsNotPartOfSender(sender, e))
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
        if (dragHandler.CanStartDrag(_dragInfo))
        {
          dragHandler.StartDrag(_dragInfo);
          if (_dragInfo.Effects != DragDropEffects.None && _dragInfo.Data != null)
          {
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
      var elementPosition = e.GetPosition((IInputElement)sender);

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
          //var offsetX = _adornerSize.Width * -GetDragMouseAnchorPoint(m_DragInfo.VisualSource).X;
          //var offsetY = _adornerSize.Height * -GetDragMouseAnchorPoint(m_DragInfo.VisualSource).Y;
          //_adornerPos.Offset(offsetX, offsetY);
          //var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
          //var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
          //if (adornerPosRightX > maxAdornerPosX)
          //{
          //   _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
          //}
          //if (_adornerPos.Y < 0)
          //{
          //   _adornerPos.Y = 0;
          //}
        }

        //DragAdorner.MousePosition = _adornerPos;
        //DragAdorner.InvalidateVisual();
        DragAdorner.EndPoint = tempAdornerPos;
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

      //e.UseDefaultCursors = true;
      //e.Handled = true;
      //if (Mouse.OverrideCursor != null)
      //{
      //  Mouse.OverrideCursor = null;
      //}
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
