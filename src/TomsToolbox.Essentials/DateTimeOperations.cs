namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Additional operations for <see cref="DateTime"/> and <see cref="TimeSpan"/>.
    /// </summary>
    public static class DateTimeOperations
    {
        /// <summary>
        /// Returns the smaller of two dates.
        /// </summary>
        /// <param name="value1">The first of two dates.</param>
        /// <param name="value2">The second of two dates.</param>
        /// <returns>Parameter value1 or value2, whichever is smaller.</returns>
        public static DateTime Min(DateTime value1, DateTime value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the larger of two dates.
        /// </summary>
        /// <param name="value1">The first of two dates.</param>
        /// <param name="value2">The second of two dates.</param>
        /// <returns>Parameter value1 or value2, whichever is larger.</returns>
        public static DateTime Max(DateTime value1, DateTime value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the smaller of two time spans.
        /// </summary>
        /// <param name="value1">The first of two time spans.</param>
        /// <param name="value2">The second of two time spans.</param>
        /// <returns>Parameter value1 or value2, whichever is smaller.</returns>
        public static TimeSpan Min(TimeSpan value1, TimeSpan value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the larger of two time spans.
        /// </summary>
        /// <param name="value1">The first of two time spans.</param>
        /// <param name="value2">The second of two time spans.</param>
        /// <returns>Parameter value1 or value2, whichever is larger.</returns>
        public static TimeSpan Max(TimeSpan value1, TimeSpan value2)
        {
            return value1 > value2 ? value1 : value2;
        }


        /// <summary>
        /// Gets the days of a week starting with the cultures first day of week.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The days of a week starting with the cultures first day of week.</returns>
        public static IList<DayOfWeek> GetDaysOfWeek(this CultureInfo cultureInfo)
        {
            var values = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            return values.OrderBy(d => d < firstDayOfWeek ? (int)d + 7 : (int)d).ToArray();
        }


        /// <summary>
        /// Rounds the time span so it does not contain any fractional seconds.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The time span with no fractional seconds.</returns>
        public static TimeSpan RoundToSeconds(this TimeSpan timeSpan)
        {
            return RoundToSeconds(timeSpan, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional seconds.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the seconds.</param>
        /// <returns> The time span with no fractional seconds.</returns>
        public static TimeSpan RoundToSeconds(this TimeSpan timeSpan, Func<double, double> roundingOperation)
        {
            return TimeSpan.FromSeconds(roundingOperation(timeSpan.TotalSeconds));
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional seconds.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The time with no fractional seconds.</returns>
        public static DateTime RoundToSeconds(this DateTime time)
        {
            return RoundToSeconds(time, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional seconds.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the seconds.</param>
        /// <returns>The time with no fractional seconds.</returns>
        public static DateTime RoundToSeconds(this DateTime time, Func<double, double> roundingOperation)
        {
            return time.Date + TimeSpan.FromSeconds(roundingOperation(time.TimeOfDay.TotalSeconds));
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional minutes.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The time span with no fractional minutes.</returns>
        public static TimeSpan RoundToMinutes(this TimeSpan timeSpan)
        {
            return RoundToMinutes(timeSpan, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional minutes.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the minutes.</param>
        /// <returns> The time span with no fractional minutes.</returns>
        public static TimeSpan RoundToMinutes(this TimeSpan timeSpan, Func<double, double> roundingOperation)
        {
            return TimeSpan.FromMinutes(roundingOperation(timeSpan.TotalMinutes));
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional minutes.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The time with no fractional minutes.</returns>
        public static DateTime RoundToMinutes(this DateTime time)
        {
            return RoundToMinutes(time, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional minutes.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the minutes.</param>
        /// <returns>The time with no fractional minutes.</returns>
        public static DateTime RoundToMinutes(this DateTime time, Func<double, double> roundingOperation)
        {
            return time.Date + TimeSpan.FromMinutes(roundingOperation(time.TimeOfDay.TotalMinutes));
        }


        /// <summary>
        /// Rounds the time span so it does not contain any fractional hours.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The time span with no fractional hours.</returns>
        public static TimeSpan RoundToHours(this TimeSpan timeSpan)
        {
            return RoundToHours(timeSpan, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional hours.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the hours.</param>
        /// <returns> The time span with no fractional hours.</returns>
        public static TimeSpan RoundToHours(this TimeSpan timeSpan, Func<double, double> roundingOperation)
        {
            return TimeSpan.FromHours(roundingOperation(timeSpan.TotalHours));
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional hours.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The time with no fractional hours.</returns>
        public static DateTime RoundToHours(this DateTime time)
        {
            return RoundToHours(time, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional hours.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the hours.</param>
        /// <returns>The time with no fractional hours.</returns>
        public static DateTime RoundToHours(this DateTime time, Func<double, double> roundingOperation)
        {
            return time.Date + TimeSpan.FromHours(roundingOperation(time.TimeOfDay.TotalHours));
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional days.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The time with no fractional days.</returns>
        public static DateTime RoundToDays(this DateTime time)
        {
            return RoundToDays(time, Math.Round);
        }

        /// <summary>
        /// Rounds the time span so it does not contain any fractional days.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="roundingOperation">The rounding operation that rounds the days.</param>
        /// <returns>The time with no fractional days.</returns>
        public static DateTime RoundToDays(this DateTime time, Func<double, double> roundingOperation)
        {
            return time.Date + TimeSpan.FromDays(roundingOperation(time.TimeOfDay.TotalDays));
        }

        /// <summary>
        /// Adds the specified number of seconds to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="value">A number of whole and fractional seconds. The <paramref name="value"/> parameter can be negative or positive.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> that represents the value of this instance plus the value of <paramref name="value"/>.
        /// </returns>
        /// <exception cref="T:System.OverflowException">The resulting <see cref="T:System.TimeSpan"/> is less than <see cref="F:System.TimeSpan.MinValue"/> or greater than <see cref="F:System.TimeSpan.MaxValue"/>.</exception>
        public static TimeSpan AddSeconds(this TimeSpan timeSpan, double value)
        {
            return timeSpan.Add(TimeSpan.FromSeconds(value));
        }

        /// <summary>
        /// Adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="value">A number of whole and fractional minutes. The <paramref name="value"/> parameter can be negative or positive.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> that represents the value of this instance plus the value of <paramref name="value"/>.
        /// </returns>
        /// <exception cref="T:System.OverflowException">The resulting <see cref="T:System.TimeSpan"/> is less than <see cref="F:System.TimeSpan.MinValue"/> or greater than <see cref="F:System.TimeSpan.MaxValue"/>.</exception>
        public static TimeSpan AddMinutes(this TimeSpan timeSpan, double value)
        {
            return timeSpan.Add(TimeSpan.FromMinutes(value));
        }

        /// <summary>
        /// Adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="value">A number of whole and fractional hours. The <paramref name="value"/> parameter can be negative or positive.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> that represents the value of this instance plus the value of <paramref name="value"/>.
        /// </returns>
        /// <exception cref="T:System.OverflowException">The resulting <see cref="T:System.TimeSpan"/> is less than <see cref="F:System.TimeSpan.MinValue"/> or greater than <see cref="F:System.TimeSpan.MaxValue"/>.</exception>
        public static TimeSpan AddHours(this TimeSpan timeSpan, double value)
        {
            return timeSpan.Add(TimeSpan.FromHours(value));
        }

        /// <summary>
        /// Adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="value">A number of whole and fractional days. The <paramref name="value"/> parameter can be negative or positive.</param>
        /// <returns>
        /// A <see cref="T:System.TimeSpan"/> that represents the value of this instance plus the value of <paramref name="value"/>.
        /// </returns>
        /// <exception cref="T:System.OverflowException">The resulting <see cref="T:System.TimeSpan"/> is less than <see cref="F:System.TimeSpan.MinValue"/> or greater than <see cref="F:System.TimeSpan.MaxValue"/>.</exception>
        public static TimeSpan AddDays(this TimeSpan timeSpan, double value)
        {
            return timeSpan.Add(TimeSpan.FromDays(value));
        }

        /// <summary>
        /// Multiplies the time span with the specified factor.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The time span multiplied with the factor</returns>
        public static TimeSpan MultipliedWith(this TimeSpan timeSpan, double factor)
        {
            return TimeSpan.FromSeconds(timeSpan.TotalSeconds * factor);
        }
    }
}
