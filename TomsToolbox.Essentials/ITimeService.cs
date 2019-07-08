namespace TomsToolbox.Essentials
{
    using System;

    /// <summary>
    /// A service providing the current date or time. 
    /// Very useful to decouple code from the static <see cref="DateTime"/> methods, to make code that has dependencies to date or time testable.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to today's date, with the time component set to 00:00:00.
        /// </summary>
        DateTime Today { get; }

        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        DateTime UtcNow { get; }
    }
}
