namespace TomsToolbox.Wpf.Interactivity;

using System;
using System.Windows;
using System.Windows.Threading;

/// <summary>
/// A trigger that continuously fires while the associated object is loaded.
/// </summary>
public sealed class TimerTrigger : Microsoft.Xaml.Behaviors.TriggerBase<FrameworkElement>
{
    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the interval of the timer.
    /// </summary>
    public TimeSpan Interval
    {
        get => this.GetValue<TimeSpan>(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="Interval"/> dependency property
    /// </summary>
    public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register(
        "Interval", typeof(TimeSpan), typeof(TimerTrigger), new FrameworkPropertyMetadata(default(TimeSpan)));

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        var target = AssociatedObject;
        if (target == null)
            return;

        target.Loaded += AssociatedObject_Loaded;
        target.Unloaded += AssociatedObject_Unloaded;
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        StopTimer();
        StartTimer();
    }

    private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
    {
        StopTimer();
    }

    private void StartTimer()
    {
        _timer = new DispatcherTimer { Interval = Interval };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer == null)
            return;
        _timer.Stop();
        _timer = null;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        InvokeActions(EventArgs.Empty);
    }
}