namespace TomsToolbox.Essentials;

using System;

/// <summary>
/// Time service returning the actual system values from <see cref="DateTime"/>.
/// </summary>
/// <seealso cref="TomsToolbox.Essentials.ITimeService" />
public class RealTimeService : ITimeService
{
    /// <summary>
    /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer, expressed as the local time.
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets a <see cref="DateTime"/> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public DateTime Today => DateTime.Today;

    /// <summary>
    /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}