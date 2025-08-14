﻿namespace TomsToolbox.Wpf.Interactivity;

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

/// <summary>
/// Obsolete: Use AdvancedScrollWheelBehavior instead.
/// </summary>
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
    private const double LineHeight = 16;   // Default physical height of one line, as defined in https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/ScrollViewer.cs,2916
    private static double VerticalMouseWheelDelta => SystemParameters.WheelScrollLines * LineHeight;
    // Actually there is no system parameter available for horizontal scrolling, so we use a fixed value here.
    private const double HorizontalMouseWheelDelta = 3 * LineHeight;
    private const double TouchPadMouseWheelDelta = 3 * LineHeight;

    private delegate IScrollInfo GetScrollInfoDelegate(ScrollViewer scrollViewer);

    private static readonly GetScrollInfoDelegate _propertyScrollInfoGetter = (GetScrollInfoDelegate)typeof(ScrollViewer).GetProperty("ScrollInfo", BindingFlags.Instance | BindingFlags.NonPublic)!
        .GetGetMethod(true)!
        .CreateDelegate(typeof(GetScrollInfoDelegate));

    private ScrollViewer? _scrollViewer;

    private double _horizontalOffsetTarget;
    private double _verticalOffsetTarget;

    private bool _lastScrollWasTouchPad;
    private int _lastScrollingTick;
    private bool _hasSubscribedToEvents;

    private uint _animationIdCounter;
    private DoubleAnimation? _currentAnimation;
    private bool IsAnimationRunning => _currentAnimation != null;

    /// <inheritdoc />
    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        SetupScrollViewer();
    }

    /// <inheritdoc />
    protected override void OnAssociatedObjectUnloaded()
    {
        base.OnAssociatedObjectUnloaded();

        UnloadScrollViewer();
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        
        // Also call SetupScrollViewer here, because the object may already be loaded when the behavior is attached.
        SetupScrollViewer();
    }
    
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        
        // Also call UnloadScrollViewer here, because the object may still be loaded when the behavior is detached.
        UnloadScrollViewer();
    }

    private void SetupScrollViewer()
    {
        if (_hasSubscribedToEvents)
            return;
        
        _scrollViewer = AssociatedObject.VisualDescendantsAndSelf().OfType<ScrollViewer>().FirstOrDefault();

        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

        _horizontalOffsetTarget = _scrollViewer.HorizontalOffset;
        _verticalOffsetTarget = _scrollViewer.VerticalOffset;

        _hasSubscribedToEvents = true;
    }
    
    private void UnloadScrollViewer()
    {
        if (_scrollViewer == null)
            return;

        _scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
        _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;

        _scrollViewer = null;
        _hasSubscribedToEvents = false;
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
        DependencyProperty.Register(nameof(TouchpadScrollDeltaFactor), typeof(double), typeof(AdvancedScrollWheelBehavior), new FrameworkPropertyMetadata(4.0));

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

        Debug.WriteLine($"Offset_Changed: {offset}");

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
        var scrollViewer = (ScrollViewer)sender;

        // Use the current offsets from the scroll viewer, event args are not reliable if there are nested scroll viewers.
        var horizontalOffsetTarget = scrollViewer.HorizontalOffset;
        var verticalOffsetTarget = scrollViewer.VerticalOffset;
        
        Debug.WriteLine($"ScrollChanged: {_verticalOffsetTarget} => {verticalOffsetTarget} ({IsAnimationRunning})");

        if (IsAnimationRunning)
            return;
        
        if (Math.Abs(horizontalOffsetTarget - _horizontalOffsetTarget) >= 1.0)
        {
            _horizontalOffsetTarget = horizontalOffsetTarget;
        }

        if (Math.Abs(verticalOffsetTarget - _verticalOffsetTarget) >= 1.0)
        {
            _verticalOffsetTarget = verticalOffsetTarget;
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
        if ((_scrollViewer == null) || (e.Handled))
            return;

        bool vertical = Keyboard.Modifiers != ModifierKeys.Shift;
        double scrollDelta = e.Delta;

        var tickCount = Environment.TickCount;
        var ticksSinceLastEvent = tickCount - _lastScrollingTick;
        _lastScrollingTick = tickCount;

        Debug.WriteLine($"Scroll: {scrollDelta}, {_verticalOffsetTarget}, ({ticksSinceLastEvent})");

        var isTouchpadScrolling = scrollDelta % Mouse.MouseWheelDeltaForOneLine != 0
            || (_lastScrollWasTouchPad && (ticksSinceLastEvent < MillisecondsBetweenTouchpadScrolling));

        _lastScrollWasTouchPad = isTouchpadScrolling;

        scrollDelta *= isTouchpadScrolling ? TouchpadScrollDeltaFactor : MouseScrollDeltaFactor;
        scrollDelta /= Mouse.MouseWheelDeltaForOneLine;

        if (vertical)
        {
            if (GetScrollInfo(_scrollViewer) is not { } scrollInfo)
                return;

            var useScrollingAnimation = UseScrollingAnimation;

            // for touchpad scrolling we ignore system settings and use a fixed value
            var verticalMouseWheelDelta = isTouchpadScrolling ? TouchPadMouseWheelDelta : VerticalMouseWheelDelta;

            if (verticalMouseWheelDelta < 0)
            {
                // scroll a page per wheel tick
                scrollDelta *= scrollInfo.ViewportHeight;
            }
            else
            {
                scrollDelta *= verticalMouseWheelDelta;

                if (scrollInfo is VirtualizingPanel virtualizingPanel && VirtualizingPanel.GetScrollUnit(virtualizingPanel) == ScrollUnit.Item)
                {
                    // animation is barely noticeable and more distracting than helpful when scrolling items
                    useScrollingAnimation = false;
                    scrollDelta /= 16;
                }
            }

            _verticalOffsetTarget = Clamp(_verticalOffsetTarget - scrollDelta, 0, _scrollViewer.ScrollableHeight);

            if (isTouchpadScrolling || !useScrollingAnimation)
            {
                _scrollViewer.BeginAnimation(AnimatedVerticalOffsetProperty, null);
                _scrollViewer.ScrollToVerticalOffset(_verticalOffsetTarget);
            }
            else
            {
                StartAnimation(_scrollViewer, AnimatedVerticalOffsetProperty, _verticalOffsetTarget, _scrollViewer.VerticalOffset);
            }
        }
        else
        {
            scrollDelta *= HorizontalMouseWheelDelta;

            _horizontalOffsetTarget = Clamp(_horizontalOffsetTarget - scrollDelta, 0, _scrollViewer.ScrollableWidth);

            if (isTouchpadScrolling || !UseScrollingAnimation)
            {
                _scrollViewer.BeginAnimation(AnimatedHorizontalOffsetProperty, null);
                _scrollViewer.ScrollToHorizontalOffset(_horizontalOffsetTarget);
            }
            else
            {
                StartAnimation(_scrollViewer, AnimatedHorizontalOffsetProperty, _horizontalOffsetTarget, _scrollViewer.HorizontalOffset);
            }
        }

        e.Handled = true;
    }

    private void StartAnimation(UIElement target, DependencyProperty targetProperty, double to, double from)
    {
        var diff = to - from;
        var absDiff = Math.Abs(diff);
        var duration = ScrollingAnimationDuration;
        if (absDiff < Mouse.MouseWheelDeltaForOneLine)
        {
            duration = new(TimeSpan.FromTicks((long)(duration.TimeSpan.Ticks * absDiff / Mouse.MouseWheelDeltaForOneLine)));
        }

        DoubleAnimation animation = new()
        {
            EasingFunction = EasingFunction,
            Duration = duration,
            From = from,
            To = to,
            Name = $"A{_animationIdCounter++}"
        };

        // Debug.WriteLine($"Animation: {from} => {to}, {duration.TimeSpan.TotalMilliseconds}");

        animation.Completed += Animation_Completed;

        target.BeginAnimation(targetProperty, animation, HandoffBehavior.SnapshotAndReplace);

        _currentAnimation = animation;
    }

    private void Animation_Completed(object? sender, EventArgs e)
    {
        if ((sender is not AnimationClock clock) || (clock.Timeline.Name != _currentAnimation?.Name))
            return;

        // Debug.WriteLine("Animation_Completed");

        _currentAnimation = null;
    }
}
