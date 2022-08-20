namespace TomsToolbox.Wpf.Converters;

using System.ComponentModel;

/// <summary>
/// Event arguments for the <see cref="ConfirmedCommandConverter.Executing"/> event.
/// </summary>
public class ConfirmedCommandEventArgs : CancelEventArgs
{
    /// <summary>
    /// Gets or sets the parameter that will be passed to the command when it's executed.
    /// </summary>
    public object? Parameter
    {
        get;
        set;
    }
}