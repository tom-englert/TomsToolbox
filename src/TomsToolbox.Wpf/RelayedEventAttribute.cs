namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Attribute to mark one property to relay the property changed events of another property from the governing class.
    /// If you call <see cref="ObservableObjectBase.OnPropertyChanged"/> for a property of the governing class,
    /// the property change event will also be raised for the relayed property.
    /// </summary>
    /// <example>
    /// <code language="C#"><![CDATA[
    /// class X : ObservableObject
    /// {
    ///     Y _governingObject;
    ///
    ///     public X(Y governingObject)
    ///     {
    ///         _governingObject = governingObject;
    ///         RelayEventsOf(_governingObject);
    ///     }
    ///
    ///     [RelayedEvent(typeof(Y))]
    ///     string Value { get { return _governingObject.Value } }
    ///
    ///     void ChageSomething()
    ///     {
    ///         _governingObject.Value = "new Value";
    ///     }
    /// }
    /// ]]></code>
    /// Changing 'Y.Value' will also raise the PropertyChanged event for the "X.Value" property.
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RelayedEventAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayedEventAttribute"/> class.
        /// </summary>
        /// <param name="sourceType">Type of the source for the events.</param>
        public RelayedEventAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayedEventAttribute"/> class.
        /// </summary>
        /// <param name="sourceType">Type of the source for the events.</param>
        /// <param name="sourceName">Name of the source property. You need to specify this only if the source property name is different.</param>
        public RelayedEventAttribute(Type sourceType, string? sourceName)
            : this(sourceType)
        {
            SourceName = sourceName;
        }

        /// <summary>
        /// Gets the type of the source for the events.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// Gets the name of the source property, or null if the name is the same as the target property.
        /// </summary>
        public string? SourceName { get; }

        internal static IDictionary<Type, IDictionary<string, string>>? CreateRelayMapping(Type? type)
        {
            if (type == null)
                return null;

            var properties = type.GetProperties();

            var relayMapping = properties
                .Select(prop => new { TargetName = prop.Name, RelayFrom = prop.GetCustomAttributes<RelayedEventAttribute>(true).FirstOrDefault() })
                .Where(item => item.RelayFrom != null)
                .Select(item => new { item.TargetName, item.RelayFrom!.SourceType, SourceName = item.RelayFrom.SourceName ?? item.TargetName })
                .GroupBy(item => item.SourceType)
                .Where(group => AreAllPropertiesValid(group.Key, group.Select(item => item.SourceName)))
                .ToDictionary(group => group.Key, group => (IDictionary<string, string>)group.ToDictionary(item => item.SourceName, item => item.TargetName));

            return relayMapping;
        }

        private static bool AreAllPropertiesValid(Type sourceType, IEnumerable<string> propertyNames)
        {
            var existingPropertyNames = sourceType.GetProperties()
                .Select(p => p.Name)
                .ToArray();

            var invalidPropertyNames = propertyNames
                .Where(name => !existingPropertyNames.Contains(name))
                .ToArray();

            if (invalidPropertyNames.Length == 0)
                return true;

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, @"Invalid RelayedEventAttribute definitions for properties '{0}' on type {1}", string.Join(", ", invalidPropertyNames), sourceType));
        }
    }
}
