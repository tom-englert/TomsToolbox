namespace TomsToolbox.Wpf.Interactivity;

using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

using Microsoft.Xaml.Behaviors;

using TomsToolbox.Wpf;

/// <summary>
/// Provides smooth scrolling behavior for a control.
/// </summary>
public class SmoothScrollingBehavior : FrameworkElementBehavior<FrameworkElement>
{
    private const long MillisecondsBetweenTouchpadScrolling = 100;

    private delegate bool GetBoolDelegate(ScrollViewer scrollViewer);
    private delegate IScrollInfo GetScrollInfoDelegate(ScrollViewer scrollViewer);

    private static readonly GetScrollInfoDelegate _propertyScrollInfoGetter = (GetScrollInfoDelegate)typeof(ScrollViewer).GetProperty("ScrollInfo", BindingFlags.Instance | BindingFlags.NonPublic)!
        .GetGetMethod(true)!
        .CreateDelegate(typeof(GetScrollInfoDelegate));

    private static readonly GetBoolDelegate _propertyHandlesMouseWheelScrollingGetter = (GetBoolDelegate)typeof(ScrollViewer)
        .GetProperty("HandlesMouseWheelScrolling", BindingFlags.Instance | BindingFlags.NonPublic)!
        .GetGetMethod(true)!
        .CreateDelegate(typeof(GetBoolDelegate));

    private static readonly IEasingFunction _scrollingAnimationEase = new CubicEase { EasingMode = EasingMode.EaseOut };

    private ScrollViewer? _scrollViewer;
    private ScrollContentPresenter? _scrollContentPresenter;

    private double _horizontalOffsetTarget;
    private double _verticalOffsetTarget;

    private bool _isAnimationRunning;
    private int _lastScrollDelta;
    private int _lastVerticalScrollingDelta;
    private int _lastHorizontalScrollingDelta;
    private long _lastScrollingTick;

    /// <inheritdoc />
    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        _scrollViewer = AssociatedObject.VisualDescendantsAndSelf().OfType<ScrollViewer>().FirstOrDefault();

        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        _scrollContentPresenter = _scrollViewer.VisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();
    }

    /// <inheritdoc />
    protected override void OnAssociatedObjectUnloaded()
    {
        base.OnAssociatedObjectUnloaded();

        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the smooth scrolling behavior is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets or sets a value indicating whether to use scrolling animation when scolling using the mouse wheel.
    /// </summary>
    public bool UseScrollingAnimation
    {
        get { return (bool)GetValue(UseScrollingAnimationProperty); }
        set { SetValue(UseScrollingAnimationProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="UseScrollingAnimation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UseScrollingAnimationProperty =
        DependencyProperty.Register(nameof(UseScrollingAnimation), typeof(bool), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets or sets the duration of the scrolling animation.
    /// </summary>
    public Duration ScrollingAnimationDuration
    {
        get { return (Duration)GetValue(ScrollingAnimationDurationProperty); }
        set { SetValue(ScrollingAnimationDurationProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="ScrollingAnimationDuration"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollingAnimationDurationProperty =
        DependencyProperty.Register(nameof(ScrollingAnimationDuration), typeof(Duration), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(250))), ValidateScrollingAnimationDuration);

    private static bool ValidateScrollingAnimationDuration(object value)
        => value is Duration { HasTimeSpan: true };

    /// <summary>
    /// Gets or sets the delta value factor while mouse scrolling.
    /// </summary>
    public double MouseScrollDeltaFactor
    {
        get { return (double)GetValue(MouseScrollDeltaFactorProperty); }
        set { SetValue(MouseScrollDeltaFactorProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="MouseScrollDeltaFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MouseScrollDeltaFactorProperty =
        DependencyProperty.Register(nameof(MouseScrollDeltaFactor), typeof(double), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(1.0));

    /// <summary>
    /// Gets or sets the delta value factor while touchpad scrolling.
    /// </summary>
    public double TouchpadScrollDeltaFactor
    {
        get { return (double)GetValue(TouchpadScrollDeltaFactorProperty); }
        set { SetValue(TouchpadScrollDeltaFactorProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="TouchpadScrollDeltaFactor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TouchpadScrollDeltaFactorProperty =
        DependencyProperty.Register(nameof(TouchpadScrollDeltaFactor), typeof(double), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(2.0));

    /// <summary>
    /// Gets or sets a value indicating whether to always handle mouse wheel scrolling, even if the control has opted out (e.g., "TextBox").
    /// </summary>
    public bool AlwaysHandleMouseWheelScrolling
    {
        get { return (bool)GetValue(AlwaysHandleMouseWheelScrollingProperty); }
        set { SetValue(AlwaysHandleMouseWheelScrollingProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="AlwaysHandleMouseWheelScrolling"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlwaysHandleMouseWheelScrollingProperty =
        DependencyProperty.Register(nameof(AlwaysHandleMouseWheelScrolling), typeof(bool), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Sets the Register property for the specified element.
    /// </summary>
    /// <param name="element">The element to set the property for.</param>
    /// <param name="value">The value to set.</param>
    public static void SetRegister(DependencyObject element, bool value)
    {
        element.SetValue(RegisterProperty, value);
    }
    /// <summary>
    /// Gets the Register property for the specified element.
    /// </summary>
    /// <param name="element">The element to get the property for.</param>
    /// <returns>The value of the Register property.</returns>
    public static bool GetRegister(DependencyObject element)
    {
        return (bool)element.GetValue(RegisterProperty);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wfp.Interactivity.SmoothScrollingBehavior.Register"/> attached property.
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// If set to true, the behavior is attached to the target element. This is a shortcut to avoid attaching the behavior in XAML when all defaults are used.
    /// </summary>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty RegisterProperty = DependencyProperty.RegisterAttached(
        "Register", typeof(bool), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(default(bool), Register_Changed));

    private static void Register_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue as bool? != true)
            return;

        var behaviors = Interaction.GetBehaviors(d);
        if (behaviors.OfType<SmoothScrollingBehavior>().Any())
            return;
        
        behaviors.Add(new SmoothScrollingBehavior());
    }

    private static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(0.0, VerticalOffset_Changed));

    private static void VerticalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((e.NewValue is not double offset) || (d is not ScrollViewer scrollViewer))
            return;

        scrollViewer.ScrollToVerticalOffset(offset);
    }

    private static readonly DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(SmoothScrollingBehavior), new FrameworkPropertyMetadata(0.0, HorizontalOffset_Changed));

    private static void HorizontalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((e.NewValue is not double offset) || (d is not ScrollViewer scrollViewer))
            return;

        scrollViewer.ScrollToHorizontalOffset(offset);
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    private static IScrollInfo GetScrollInfo(ScrollViewer scrollViewer)
    {
        return _propertyScrollInfoGetter(scrollViewer);
    }

    private static bool HandlesMouseWheelScrolling(ScrollViewer scrollViewer)
    {
        return _propertyHandlesMouseWheelScrollingGetter(scrollViewer);
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!IsEnabled)
            return;

        Scroll(e);
    }

    private void Scroll(MouseWheelEventArgs e)
    {
        if (_scrollViewer == null || _scrollContentPresenter == null)
            return;

        if (e.Handled)
        {
            return;
        }

        if (!AlwaysHandleMouseWheelScrolling && !HandlesMouseWheelScrolling(_scrollViewer))
        {
            return;
        }

        bool vertical = Keyboard.Modifiers != ModifierKeys.Shift;

        var tickCount = Environment.TickCount;
        double scrollDelta = e.Delta;

        var isTouchpadScrolling = scrollDelta % Mouse.MouseWheelDeltaForOneLine != 0 || (tickCount - _lastScrollingTick < MillisecondsBetweenTouchpadScrolling && _lastScrollDelta % Mouse.MouseWheelDeltaForOneLine != 0);

        if (isTouchpadScrolling)
        {
            scrollDelta *= TouchpadScrollDeltaFactor;
        }
        else
        {
            scrollDelta *= MouseScrollDeltaFactor;
        }

        if (vertical)
        {
            if (GetScrollInfo(_scrollViewer) is { } scrollInfo)
            {
                // Considering that VirtualizingPanel may have a virtual size, so the Delta needs to be corrected here
                scrollDelta *= scrollInfo.ViewportHeight / (_scrollContentPresenter?.ActualHeight ?? _scrollViewer.ActualHeight);
            }

            var sameDirectionAsLast = Math.Sign(e.Delta) == Math.Sign(_lastVerticalScrollingDelta);
            var nowOffset = sameDirectionAsLast && _isAnimationRunning ? _verticalOffsetTarget : _scrollViewer.VerticalOffset;
            var newOffset = Clamp(nowOffset - scrollDelta, 0, _scrollViewer.ScrollableHeight);

            _verticalOffsetTarget = newOffset;

            if (!UseScrollingAnimation || isTouchpadScrolling)
            {
                _scrollViewer.BeginAnimation(VerticalOffsetProperty, null);
                _scrollViewer.ScrollToVerticalOffset(newOffset);
            }
            else
            {
                var diff = newOffset - _scrollViewer.VerticalOffset;
                var absDiff = Math.Abs(diff);
                var duration = ScrollingAnimationDuration;
                if (absDiff < Mouse.MouseWheelDeltaForOneLine)
                {
                    duration = new(TimeSpan.FromTicks((long)(duration.TimeSpan.Ticks * absDiff / Mouse.MouseWheelDeltaForOneLine)));
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    EasingFunction = _scrollingAnimationEase,
                    Duration = duration,
                    From = _scrollViewer.VerticalOffset,
                    To = newOffset,
                };

                doubleAnimation.Completed += Animation_Completed;

                _isAnimationRunning = true;
                _scrollViewer.BeginAnimation(VerticalOffsetProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            _lastVerticalScrollingDelta = e.Delta;
        }
        else
        {
            if (GetScrollInfo(_scrollViewer) is { } scrollInfo)
            {
                // Considering that VirtualizingPanel may have a virtual size, so the Delta needs to be corrected here
                scrollDelta *= scrollInfo.ViewportWidth / (_scrollContentPresenter?.ActualWidth ?? _scrollViewer.ActualWidth);
            }

            var sameDirectionAsLast = Math.Sign(e.Delta) == Math.Sign(_lastHorizontalScrollingDelta);
            var nowOffset = sameDirectionAsLast && _isAnimationRunning ? _horizontalOffsetTarget : _scrollViewer.HorizontalOffset;
            var newOffset = Clamp(nowOffset - scrollDelta, 0, _scrollViewer.ScrollableWidth);

            _horizontalOffsetTarget = newOffset;

            _scrollViewer.BeginAnimation(HorizontalOffsetProperty, null);

            if (!UseScrollingAnimation || isTouchpadScrolling)
            {
                _scrollViewer.ScrollToHorizontalOffset(newOffset);
            }
            else
            {
                var diff = newOffset - _scrollViewer.HorizontalOffset;
                var absDiff = Math.Abs(diff);
                var duration = ScrollingAnimationDuration;
                if (absDiff < Mouse.MouseWheelDeltaForOneLine)
                {
                    duration = new(TimeSpan.FromTicks((long)(duration.TimeSpan.Ticks * absDiff / Mouse.MouseWheelDeltaForOneLine)));
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    EasingFunction = _scrollingAnimationEase,
                    Duration = duration,
                    From = _scrollViewer.HorizontalOffset,
                    To = newOffset,
                };

                doubleAnimation.Completed += Animation_Completed;

                _isAnimationRunning = true;
                _scrollViewer.BeginAnimation(HorizontalOffsetProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            _lastHorizontalScrollingDelta = e.Delta;
        }

        _lastScrollingTick = tickCount;
        _lastScrollDelta = e.Delta;

        e.Handled = true;
    }

    private void Animation_Completed(object? sender, EventArgs e)
    {
        _isAnimationRunning = false;
    }
}
