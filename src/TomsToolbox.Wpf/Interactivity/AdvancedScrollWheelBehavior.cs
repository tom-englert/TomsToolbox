namespace TomsToolbox.Wpf.Interactivity;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

using Microsoft.Xaml.Behaviors;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf;

[Obsolete("Use AdvancedScrollWheelBehavior instead.")]
// For backward compatibility, the SmoothScrollingBehavior is still available, but it is recommended to use the AdvancedScrollWheelBehavior instead.
public class SmoothScrollingBehavior : AdvancedScrollWheelBehavior
{
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
        SetAttach(d, (e.NewValue is true) ? AdvancedScrollWheelMode.WithAnimation : AdvancedScrollWheelMode.None);
    }
}

/// <summary>
/// Provides advanced mouse wheel scrolling behavior for a control.
/// Makes the scrolling distance smoother, especially for touchpad scrolling, and optionally animated for mouse wheel scrolling.
/// </summary>
public class AdvancedScrollWheelBehavior : FrameworkElementBehavior<FrameworkElement>
{
    private const long MillisecondsBetweenTouchpadScrolling = 100;

    private delegate IScrollInfo GetScrollInfoDelegate(ScrollViewer scrollViewer);

    private static readonly GetScrollInfoDelegate _propertyScrollInfoGetter = (GetScrollInfoDelegate)typeof(ScrollViewer).GetProperty("ScrollInfo", BindingFlags.Instance | BindingFlags.NonPublic)!
        .GetGetMethod(true)!
        .CreateDelegate(typeof(GetScrollInfoDelegate));

    private ScrollViewer? _scrollViewer;
    private ScrollContentPresenter? _scrollContentPresenter;

    private double _horizontalOffsetTarget;
    private double _verticalOffsetTarget;

    private bool _lastScrollWasTouchPad;
    private long _lastScrollingTick;

    private int _animationIdCounter;
    private DoubleAnimation? _currentAnimation;

    private bool IsAnimationRunning => _currentAnimation != null;

    /// <inheritdoc />
    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        _scrollViewer = AssociatedObject.VisualDescendantsAndSelf().OfType<ScrollViewer>().FirstOrDefault();

        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

        _horizontalOffsetTarget = _scrollViewer.HorizontalOffset;
        _verticalOffsetTarget = _scrollViewer.VerticalOffset;

        _scrollContentPresenter = _scrollViewer.VisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();
    }

    /// <inheritdoc />
    protected override void OnAssociatedObjectUnloaded()
    {
        base.OnAssociatedObjectUnloaded();

        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
        _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
    }

    #region DependencyProperties

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
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets or sets a value indicating whether to use scrolling animation when scrolling using the mouse wheel.
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
        DependencyProperty.Register(nameof(UseScrollingAnimation), typeof(bool), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(true));

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
        DependencyProperty.Register(nameof(ScrollingAnimationDuration), typeof(Duration), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(250))), ValidateScrollingAnimationDuration);

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
        DependencyProperty.Register(nameof(MouseScrollDeltaFactor), typeof(double), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(1.0));

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
        DependencyProperty.Register(nameof(TouchpadScrollDeltaFactor), typeof(double), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(2.0));

    /// <summary>
    /// Gets or sets the easing function used for the scrolling animation.
    /// </summary>
    public IEasingFunction? EasingFunction
    {
        get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
        set { SetValue(EasingFunctionProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="EasingFunction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register(
        nameof(EasingFunction), typeof(IEasingFunction), typeof(AdvancedScrollWheelBehavior));

    /// <summary>
    /// Sets the Register property for the specified element.
    /// </summary>
    /// <param name="element">The element to set the property for.</param>
    /// <param name="value">The value to set.</param>
    public static void SetAttach(DependencyObject element, AdvancedScrollWheelMode value)
    {
        element.SetValue(AttachProperty, value);
    }
    /// <summary>
    /// Gets the Register property for the specified element.
    /// </summary>
    /// <param name="element">The element to get the property for.</param>
    /// <returns>The value of the Register property.</returns>
    public static AdvancedScrollWheelMode GetAttach(DependencyObject element)
    {
        return (AdvancedScrollWheelMode)element.GetValue(AttachProperty);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wfp.Interactivity.SmoothScrollingBehavior.Attach"/> attached property.
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// If set to <see cref="AdvancedScrollWheelMode.WithAnimation"/> or <see cref="AdvancedScrollWheelMode.WithoutAnimation"/>, the behavior is attached to the target element with default settings.
    /// This is a shortcut to omit the full behavior notation in XAML when only defaults are used.
    /// </summary>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
        "Attach", typeof(AdvancedScrollWheelMode), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(default(AdvancedScrollWheelMode), Attach_Changed));

    private static void Attach_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behaviors = Interaction.GetBehaviors(d);

        behaviors.RemoveWhere(item => item is AdvancedScrollWheelBehavior);

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.NewValue as AdvancedScrollWheelMode?)
        {
            case AdvancedScrollWheelMode.WithAnimation:
                behaviors.Add(new AdvancedScrollWheelBehavior { UseScrollingAnimation = true });
                break;

            case AdvancedScrollWheelMode.WithoutAnimation:
                behaviors.Add(new AdvancedScrollWheelBehavior { UseScrollingAnimation = false });
                break;
        }
    }

    #endregion

    private static readonly DependencyProperty AnimatedVerticalOffsetProperty =
        DependencyProperty.RegisterAttached("AnimatedVerticalOffset", typeof(double), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(0.0, AnimatedVerticalOffset_Changed));

    private static void AnimatedVerticalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((e.NewValue is not double offset) || (d is not ScrollViewer scrollViewer))
            return;

        Debug.WriteLine($"Offset: {offset}");

        scrollViewer.ScrollToVerticalOffset(offset);
    }

    private static readonly DependencyProperty AnimatedHorizontalOffsetProperty =
        DependencyProperty.RegisterAttached("AnimatedHorizontalOffset", typeof(double), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(0.0, AnimatedHorizontalOffset_Changed));

    private static void AnimatedHorizontalOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        Debug.WriteLine($"ScrollChanged: {e.VerticalOffset} => {IsAnimationRunning}");

        if (IsAnimationRunning)
            return;

        if (Math.Abs(e.HorizontalOffset - _horizontalOffsetTarget) >= 1.0)
        {
            _horizontalOffsetTarget = e.HorizontalOffset;
        }

        if (Math.Abs(e.VerticalOffset - _verticalOffsetTarget) >= 1.0)
        {
            _verticalOffsetTarget = e.VerticalOffset;
        }
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

        bool vertical = Keyboard.Modifiers != ModifierKeys.Shift;

        var tickCount = Environment.TickCount;
        double scrollDelta = e.Delta;

        var isTouchpadScrolling = scrollDelta % Mouse.MouseWheelDeltaForOneLine != 0
            || (_lastScrollWasTouchPad && (tickCount - _lastScrollingTick < MillisecondsBetweenTouchpadScrolling));

        scrollDelta *= isTouchpadScrolling ? TouchpadScrollDeltaFactor : MouseScrollDeltaFactor;

        if (vertical)
        {
            if (GetScrollInfo(_scrollViewer) is { } scrollInfo)
            {
                // Considering that VirtualizingPanel may have a virtual size, so the Delta needs to be corrected here
                scrollDelta *= scrollInfo.ViewportHeight / (_scrollContentPresenter?.ActualHeight ?? _scrollViewer.ActualHeight);
            }

            Debug.WriteLine($"Delta: {scrollDelta}");

            var nowOffset = _verticalOffsetTarget;
            var newOffset = Clamp(nowOffset - scrollDelta, 0, _scrollViewer.ScrollableHeight);

            _verticalOffsetTarget = newOffset;

            if (isTouchpadScrolling || !UseScrollingAnimation)
            {
                _scrollViewer.BeginAnimation(AnimatedVerticalOffsetProperty, null);
                _scrollViewer.ScrollToVerticalOffset(newOffset);
            }
            else
            {
                var from = _scrollViewer.VerticalOffset;
                var diff = newOffset - from;
                var absDiff = Math.Abs(diff);
                var duration = ScrollingAnimationDuration;
                if (absDiff < Mouse.MouseWheelDeltaForOneLine)
                {
                    duration = new(TimeSpan.FromTicks((long)(duration.TimeSpan.Ticks * absDiff / Mouse.MouseWheelDeltaForOneLine)));
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    EasingFunction = EasingFunction,
                    Duration = duration,
                    From = from,
                    To = newOffset,
                    Name = $"Ani{_animationIdCounter++}"
                };

                Debug.WriteLine($"Animate: {from} => {newOffset}, {duration.TimeSpan.TotalMilliseconds}");

                doubleAnimation.Completed += Animation_Completed;

                _currentAnimation = doubleAnimation;
                _scrollViewer.BeginAnimation(AnimatedVerticalOffsetProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }
        }
        else
        {
            if (GetScrollInfo(_scrollViewer) is { } scrollInfo)
            {
                // Considering that VirtualizingPanel may have a virtual size, so the Delta needs to be corrected here
                scrollDelta *= scrollInfo.ViewportWidth / (_scrollContentPresenter?.ActualWidth ?? _scrollViewer.ActualWidth);
            }

            var nowOffset = _horizontalOffsetTarget;
            var newOffset = Clamp(nowOffset - scrollDelta, 0, _scrollViewer.ScrollableWidth);

            _horizontalOffsetTarget = newOffset;

            if (isTouchpadScrolling || !UseScrollingAnimation)
            {
                _scrollViewer.BeginAnimation(AnimatedHorizontalOffsetProperty, null);
                _scrollViewer.ScrollToHorizontalOffset(newOffset);
            }
            else
            {
                var diff = newOffset - nowOffset;
                var absDiff = Math.Abs(diff);
                var duration = ScrollingAnimationDuration;
                if (absDiff < Mouse.MouseWheelDeltaForOneLine)
                {
                    duration = new(TimeSpan.FromTicks((long)(duration.TimeSpan.Ticks * absDiff / Mouse.MouseWheelDeltaForOneLine)));
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    EasingFunction = EasingFunction,
                    Duration = duration,
                    From = nowOffset,
                    To = newOffset,
                };

                doubleAnimation.Completed += Animation_Completed;

                _currentAnimation = doubleAnimation;
                _scrollViewer.BeginAnimation(AnimatedHorizontalOffsetProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }
        }

        _lastScrollingTick = tickCount;
        _lastScrollWasTouchPad = isTouchpadScrolling;

        e.Handled = true;
    }

    private void Animation_Completed(object? sender, EventArgs e)
    {
        if ((sender is not AnimationClock clock) || (clock.Timeline.Name != _currentAnimation?.Name))
            return;

        Debug.WriteLine("Animation finished");
        _currentAnimation = null;
    }
}
