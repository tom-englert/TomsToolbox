namespace TomsToolbox.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;
#if NETSTANDARD1_0
    using System.Reflection;
#endif

    /// <summary>
    /// Extension methods to ease dealing with <see cref="Enum"/> types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Determines whether any of the specified flags is set on the specified value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>True if any of the specified flags is set.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag", Justification="Dealing with flags")]
        public static bool IsAnyFlagSet<T>(this T value, T flag) where T : struct
        {
            VerifyIsFlagsEnum<T>();

            var lValue = ToInt64(value);
            var lFlags = ToInt64(flag);

            return (lValue & lFlags) != 0;
        }

        /// <summary>
        /// Determines whether all of the specified flags are set on the specified value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>True if all of the specified flags are set.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flag", Justification="Dealing with flags")]
        public static bool AreAllFlagsSet<T>(this T value, T flag) where T : struct
        {
            VerifyIsFlagsEnum<T>();

            var lValue = ToInt64(value);
            var lFlags = ToInt64(flag);

            return (lValue & lFlags) == lFlags;
        }

        /// <summary>
        /// Gets the individual flags set on the specified value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The individual flags set on the specified value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [NotNull]
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {

            VerifyIsFlagsEnum<T>();

            return Enum.GetValues(typeof(T)).Cast<T>().Where(flag => value.IsAnyFlagSet(flag));
        }

        /// <summary>
        /// Sets the specified flags on the specified value on or off.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="on">If set to <c>true</c>, the flags are set, otherwise the flags are cleared.</param>
        /// <returns>The value with the specified flags set or cleared.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification="Dealing with flags")]
        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
        {
            VerifyIsFlagsEnum<T>();

            var lValue = ToInt64(value);
            var lFlag = ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }

            // ReSharper disable once PossibleNullReferenceException
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        /// <summary>
        /// Sets the specified flags on the specified value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The value with the specified flags set.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification="Dealing with flags")]
        public static T SetFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, true);
        }

        /// <summary>
        /// Clears the specified flags on the specified value.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The value with the specified flags cleared.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification="Dealing with flags")]
        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }

        /// <summary>
        /// Combines the flags into a single <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
        /// <param name="flags">The flags.</param>
        /// <returns>The combined flags.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification="Dealing with flags")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification="Dealing with flags")]
        public static T CombineFlags<T>([NotNull] this IEnumerable<T> flags) where T : struct
        {

            VerifyIsFlagsEnum<T>();

            var lValue = flags
                .Select(flag => ToInt64(flag))
                .Aggregate<long, long>(0, (current, lFlag) => current | lFlag);

            // ReSharper disable once PossibleNullReferenceException
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        /// <summary>
        /// Converts an integer value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        public static T ToEnum<T>(this int value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts an unsigned integer value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        [CLSCompliant(false)]
        public static T ToEnum<T>(this uint value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts a short value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        public static T ToEnum<T>(this short value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts an unsigned short value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        [CLSCompliant(false)]
        public static T ToEnum<T>(this ushort value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts a long value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        public static T ToEnum<T>(this long value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts an unsigned long value into an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Enum"/> type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Enum"/> corresponding to the value. If the value is not valid for the given <see cref="Enum"/>, the default value for the <see cref="Enum"/> is returned.</returns>
        [CLSCompliant(false)]
        public static T ToEnum<T>(this ulong value)
            where T : struct
        {
            return InternalToEnum<T>(value);
        }

        /// <summary>
        /// Converts an integer value into an enum.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum corresponding to the value. If the value is not valid for the given enum, the default value for the enum is returned.</returns>
        public static T? ToEnum<T>(this int? value)
            where T : struct
        {
            return InternalToNullableEnum<T>(value);
        }

        /// <summary>
        /// Converts an unsigned integer value into an enum.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum corresponding to the value. If the value is not valid for the given enum, the default value for the enum is returned.</returns>
        [CLSCompliant(false)]
        public static T? ToEnum<T>(this uint? value)
            where T : struct
        {
            return InternalToNullableEnum<T>(value);
        }

        /// <summary>
        /// Converts a short value into an enum.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum corresponding to the value. If the value is not valid for the given enum, the default value for the enum is returned.</returns>
        public static T? ToEnum<T>(this short? value)
            where T : struct
        {
            return InternalToNullableEnum<T>(value);
        }

        /// <summary>
        /// Converts an unsigned short value into an enum.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The enum corresponding to the value. If the value is not valid for the given enum, the default value for the enum is returned.</returns>
        [CLSCompliant(false)]
        public static T? ToEnum<T>(this ushort? value)
            where T : struct
        {
            return InternalToNullableEnum<T>(value);
        }

        private static T? InternalToNullableEnum<T>([CanBeNull] object value)
            where T : struct
        {
            if (value == null)
                return null;

            return InternalToEnum<T>(value);
        }

        private static T InternalToEnum<T>([NotNull] object value)
            where T : struct
        {
            var enumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (!enumType.GetTypeInfo().IsEnum)
                throw new InvalidOperationException(enumType.Name + " is not an System.Enum.");

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return (T)Enum.ToObject(enumType, value);
            }
            catch (ArgumentException)
            {
                return default(T);
            }
        }

        private static long ToInt64([CanBeNull] object value)
        {
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }

        private static void VerifyIsEnum<T>()
        {
            // ReSharper disable once PossibleNullReferenceException
            if (!typeof(T).GetTypeInfo().IsEnum)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' is not an System.Enum", typeof(T).FullName));
        }

        private static void VerifyIsFlagsEnum<T>()
        {
            VerifyIsEnum<T>();

            // ReSharper disable once AssignNullToNotNullAttribute
            if (!typeof(T).GetTypeInfo().GetCustomAttributes(true).OfType<FlagsAttribute>().Any())
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }
    }
}

