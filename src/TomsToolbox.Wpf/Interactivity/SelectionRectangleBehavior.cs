namespace TomsToolbox.Wpf.Interactivity;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

using TomsToolbox.Wpf.Controls;

/// <summary>
/// Adding selection rectangle behavior to a layer canvas. Selection is done with the right mouse button.
/// </summary>
public class SelectionRectangleBehavior : Behavior<ViewportCanvas>
{
    private Point? _startPosition;

    /// <summary>
    /// Gets or sets the target element that displays the selection.
    /// </summary>
    public FrameworkElement? TargetElement
    {
        get => (FrameworkElement?)GetValue(TargetElementProperty);
        set => SetValue(TargetElementProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="TargetElement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetElementProperty =
        DependencyProperty.Register("TargetElement", typeof(FrameworkElement), typeof(SelectionRectangleBehavior));


    /// <summary>
    /// Gets or sets the view port in which the target element is displayed.
    /// </summary>
    public FrameworkElement? Viewport
    {
        get => (FrameworkElement?)GetValue(ViewportProperty);
        set => SetValue(ViewportProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="Viewport"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewportProperty =
        DependencyProperty.Register("Viewport", typeof(FrameworkElement), typeof(SelectionRectangleBehavior), new FrameworkPropertyMetadata((sender, e) => ((SelectionRectangleBehavior)sender)?.Viewport_Changed((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue)));


    /// <summary>
    /// Gets or sets the selection rectangle in logical coordinates.
    /// </summary>
    public Rect Selection
    {
        get => this.GetValue<Rect>(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="Selection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectionProperty =
        DependencyProperty.Register("Selection", typeof(Rect), typeof(SelectionRectangleBehavior), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => ((SelectionRectangleBehavior)sender)?.Selection_Changed((Rect)e.NewValue)));


    private void Selection_Changed(Rect value)
    {
        var visual = TargetElement;

        if (visual == null)
            return;

        if (value.IsEmpty)
        {
            visual.Visibility = Visibility.Collapsed;
            return;
        }

        visual.Visibility = Visibility.Visible;

        Canvas.SetLeft(visual, value.Left);
        Canvas.SetTop(visual, value.Top);
        Canvas.SetRight(visual, value.Right);
        Canvas.SetBottom(visual,value.Bottom);

        AssociatedObject?.InvalidateMeasure();
    }


    private void Viewport_Changed(FrameworkElement? oldValue, FrameworkElement? newValue)
    {
        if (oldValue != null)
        {
            oldValue.MouseMove -= Viewport_MouseMove;
            oldValue.MouseRightButtonDown -= Viewport_MouseRightButtonDown;
            oldValue.MouseRightButtonUp -= Viewport_MouseRightButtonUp;
        }

        if (newValue != null)
        {
            newValue.MouseMove += Viewport_MouseMove;
            newValue.MouseRightButtonDown += Viewport_MouseRightButtonDown;
            newValue.MouseRightButtonUp += Viewport_MouseRightButtonUp;
        }
    }


    private void Viewport_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_startPosition == null)
            return;

        var worldLayer = AssociatedObject?.World;
        if (worldLayer == null)
            return;

        var p0 = _startPosition.GetValueOrDefault();
        var p1 = e.GetPosition(worldLayer);

        Selection = new Rect(p0, p1);
    }


    private void Viewport_MouseRightButtonDown(object? sender, MouseButtonEventArgs e)
    {
        var viewport = Viewport;
        if (viewport == null)
            return;

        var worldLayer = AssociatedObject?.World;
        if (worldLayer == null)
            return;

        _startPosition = e.GetPosition(worldLayer);

        viewport.Cursor = Cursors.ScrollAll;
        viewport.CaptureMouse();
    }

    private void Viewport_MouseRightButtonUp(object? sender, MouseButtonEventArgs? e)
    {
        _startPosition = null;

        var viewport = Viewport;
        if (viewport == null)
            return;

        viewport.Cursor = null;
        viewport.ReleaseMouseCapture();
    }
}