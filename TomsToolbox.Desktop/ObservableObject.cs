namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Base class implementing <see cref="INotifyPropertyChanged"/>.<para/>
    /// Supports declarative dependencies specified by the <see cref="PropertyDependencyAttribute"/> and
    /// relaying events of other objects using the <see cref="RelayedEventAttribute"/>.
    /// </summary>
    /// <remarks>
    /// Also implements <see cref="IDataErrorInfo"/> (and INotifyDataErrorInfo in .Net4.5++) to support validation. The default implementation examines <see cref="ValidationAttribute"/> on the affected properties to retrieve error information.
    /// </remarks>
    [Serializable]
    public abstract class ObservableObjectBase : INotifyPropertyChanged, IDataErrorInfo
#if NETFRAMEWORK_4_5
        , INotifyDataErrorInfo
#endif
    {
        [NotNull]
        private static readonly AutoWeakIndexer<Type, IDictionary<string, IEnumerable<string>>> _dependencyMappingCache = new AutoWeakIndexer<Type, IDictionary<string, IEnumerable<string>>>(PropertyDependencyAttribute.CreateDependencyMapping);
        [NonSerialized, CanBeNull]
        private IDictionary<string, IEnumerable<string>> _dependencyMapping;

        [NotNull]
        private static readonly AutoWeakIndexer<Type, IDictionary<Type, IDictionary<string, string>>> _relayMappingCache = new AutoWeakIndexer<Type, IDictionary<Type, IDictionary<string, string>>>(RelayedEventAttribute.CreateRelayMapping);
        [NonSerialized, CanBeNull]
        private IDictionary<Type, IDictionary<string, string>> _relayMapping;

        [NonSerialized, CanBeNull]
        private Dictionary<Type, INotifyPropertyChanged> _eventSources;

        /// <summary>
        /// Relays the property changed events of the source object (if not null) and detaches the old source (if not null).
        /// </summary>
        /// <param name="oldSource"></param>
        /// <param name="newSource"></param>
        protected void RelayEventsOf([CanBeNull] INotifyPropertyChanged oldSource, [CanBeNull] INotifyPropertyChanged newSource)
        {
            if (ReferenceEquals(oldSource, newSource))
                return;

            if (newSource != null)
            {
                RelayEventsOf(newSource);
            }
            else
            {
                DetachEventSource(oldSource);
            }
        }

        /// <summary>
        /// Relays the property changed events of the source object.
        /// The properties to relay must be declared with the <see cref="RelayedEventAttribute"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        protected void RelayEventsOf([NotNull] INotifyPropertyChanged source)
        {
            Contract.Requires(source != null);

            var sourceType = source.GetType();
            if (RelayMapping.Keys.All(key => key?.IsAssignableFrom(sourceType) != true))
                throw new InvalidOperationException(@"This class has no property with a RelayedEventAttribute for the type " + sourceType);

            INotifyPropertyChanged oldSource;
            if (EventSources.TryGetValue(sourceType, out oldSource) && (oldSource != null))
                oldSource.PropertyChanged -= RelaySource_PropertyChanged;

            source.PropertyChanged += RelaySource_PropertyChanged;
            EventSources[sourceType] = source;
        }

        /// <summary>
        /// Detaches all event sources.
        /// </summary>
        protected void DetachEventSources()
        {
            foreach (var item in EventSources.Values.ToArray())
            {
                Contract.Assume(item != null);
                DetachEventSource(item);
            }
        }

        /// <summary>
        /// Detaches the event source.
        /// </summary>
        /// <param name="item">The item to detach.</param>
        protected void DetachEventSource([NotNull] INotifyPropertyChanged item)
        {
            Contract.Requires(item != null);
            item.PropertyChanged -= RelaySource_PropertyChanged;
            EventSources.Remove(item.GetType());
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression identifying the property.</param>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged<T>([NotNull] Expression<Func<T>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);

            OnPropertyChanged(PropertySupport.ExtractPropertyName(propertyExpression));
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyExpression">The expression identifying the property.</param>
        /// <returns>True if value has changed and the PropertyChange event was raised.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [NotNull] Expression<Func<T>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);

            return SetProperty(ref backingField, value, PropertySupport.ExtractPropertyName(propertyExpression));
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyExpression">The expression identifying the property.</param>
        /// <param name="changeCallback">The callback that is invoked if the value has changed. Parameters are (oldValue, newValue).</param>
        /// <returns>True if value has changed and the PropertyChange event was raised.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [NotNull] Expression<Func<T>> propertyExpression, [NotNull] Action<T, T> changeCallback)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Requires(changeCallback != null);

            return SetProperty(ref backingField, value, PropertySupport.ExtractPropertyName(propertyExpression), changeCallback);
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property; <c>.Net 4.5 only:</c> omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
        /// <returns>True if value has changed and the PropertyChange event was raised. </returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
#if NETFRAMEWORK_4_5
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [System.Runtime.CompilerServices.CallerMemberName][NotNull] string propertyName = null)
#else
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [NotNull] string propertyName)
#endif
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            if (Equals(backingField, value))
                return false;

            backingField = value;

            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="changeCallback">The callback that is invoked if the value has changed. Parameters are (oldValue, newValue).</param>
        /// <returns> True if value has changed and the PropertyChange event was raised. </returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [NotNull] string propertyName, [NotNull] Action<T, T> changeCallback)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));
            Contract.Requires(changeCallback != null);

            var oldValue = backingField;

            if (!SetProperty(ref backingField, value, propertyName))
                return false;

            changeCallback(oldValue, value);
            return true;
        }

        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="changeCallback">The callback that is invoked if the value has changed. Parameters are (oldValue, newValue).</param>
        /// <param name="propertyName">Name of the property; omit this parameter to use the callers name provided by the CallerMemberNameAttribute (.Net4.5 only)</param>
        /// <returns> True if value has changed and the PropertyChange event was raised. </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This pattern is required by the CallerMemberName attribute.")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>([CanBeNull] ref T backingField, [CanBeNull] T value, [NotNull] Action<T, T> changeCallback,
#if !NETFRAMEWORK_4_5
            [NotNull] string propertyName)
#else
            [System.Runtime.CompilerServices.CallerMemberName][NotNull] string propertyName = null)
#endif
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));
            Contract.Requires(changeCallback != null);

            return SetProperty(ref backingField, value, propertyName, changeCallback);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property; <c>.Net 4.5 only:</c> omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This pattern is required by the CallerMemberName attribute.")]
        [NotifyPropertyChangedInvocator]
#if NETFRAMEWORK_4_5
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName][NotNull] string propertyName = null)
#else
        protected void OnPropertyChanged([NotNull] string propertyName)
#endif
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            InternalOnPropertyChanged(propertyName);

            IEnumerable<string> dependentProperties;

            if (!DependencyMapping.TryGetValue(propertyName, out dependentProperties))
                return;

            Contract.Assume(dependentProperties != null);

            foreach (var dependentProperty in dependentProperties)
            {
                Contract.Assume(!string.IsNullOrEmpty(dependentProperty));
                InternalOnPropertyChanged(dependentProperty);
            }
        }

        [NotNull]
        private Dictionary<Type, INotifyPropertyChanged> EventSources => _eventSources ?? (_eventSources = new Dictionary<Type, INotifyPropertyChanged>());

        [NotNull]
        private IDictionary<Type, IDictionary<string, string>> RelayMapping => _relayMapping ?? (_relayMapping = _relayMappingCache[GetType()]);

        [NotNull]
        private IDictionary<string, IEnumerable<string>> DependencyMapping => _dependencyMapping ?? (_dependencyMapping = _dependencyMappingCache[GetType()]);

        // ReSharper disable once AnnotateNotNullParameter
        private void RelaySource_PropertyChanged([NotNull] object sender, PropertyChangedEventArgs e)
        {
            Contract.Requires(sender != null);

            // ReSharper disable once PossibleNullReferenceException
            if (e.PropertyName == null)
                return;

            var sourceType = sender.GetType();
            // ReSharper disable once PossibleNullReferenceException
            foreach (var mapping in RelayMapping.Where(item => item.Key.IsAssignableFrom(sourceType)).Select(item => item.Value))
            {
                Contract.Assume(mapping != null);

                if (mapping.TryGetValue(e.PropertyName, out var targetPropertyName) && !string.IsNullOrEmpty(targetPropertyName))
                {
                    OnPropertyChanged(targetPropertyName);
                }
            }
        }

        private void InternalOnPropertyChanged([NotNull] string propertyName)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <remarks>
        /// The default implementation returns the <see cref="ValidationAttribute"/> errors of the property.
        /// </remarks>
        [NotNull, ItemNotNull]
        protected virtual IEnumerable<string> GetDataErrors([CanBeNull] string propertyName)
        {
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            if (string.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            var property = GetType().GetProperty(propertyName);
            if (property == null)
                return Enumerable.Empty<string>();

            var errorInfos = property.GetCustomAttributes<ValidationAttribute>(true)
                .Where(va => va.GetValidationResult(property.GetValue(this, null), new ValidationContext(this, null, null)) != ValidationResult.Success)
                .Select(va => va.FormatErrorMessage(propertyName));

            return errorInfos;
        }

        /// <summary>
        /// Called when data errors have been evaluated. Used e.g. to track data errors for each property.
        /// </summary>
        /// <param name="propertyName">Name of the property, or <c>null</c> if the errors .</param>
        /// <param name="dataErrors">The data errors for the property.</param>
        protected virtual void OnDataErrorsEvaluated([CanBeNull] string propertyName, [CanBeNull, ItemNotNull] IEnumerable<string> dataErrors)
        {
        }

        [NotNull, ItemNotNull]
        private IEnumerable<string> InternalGetDataErrors([CanBeNull] string propertyName)
        {
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            var dataErrors = GetDataErrors(propertyName).ToArray();

            OnDataErrorsEvaluated(propertyName, dataErrors);

            return dataErrors;
        }

        [CanBeNull]
        string IDataErrorInfo.Error => InternalGetDataErrors(null).FirstOrDefault();

        [CanBeNull]
        string IDataErrorInfo.this[[CanBeNull] string columnName] => InternalGetDataErrors(columnName).FirstOrDefault();

#if NETFRAMEWORK_4_5
        private event EventHandler<DataErrorsChangedEventArgs> _errorsChanged;

        /// <summary>
        /// Raises the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property where validation errors have changed; or null or <see cref="F:System.String.Empty"/>, when entity-level errors have changed.</param>
        protected void OnErrorsChanged(string propertyName)
        {
            var eventHandler = _errorsChanged;
            if (eventHandler != null)
                eventHandler(this, new DataErrorsChangedEventArgs(propertyName));
        }

        System.Collections.IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return InternalGetDataErrors(propertyName);
        }

        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return InternalGetDataErrors(null).Any();
            }
        }

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
                _errorsChanged += value;
            }
            remove
            {
                _errorsChanged -= value;
            }
        }
#endif
    }

    /// <summary>
    /// Like <see cref="TomsToolbox.Desktop.ObservableObjectBase" />, with an additional dispatcher field to track the owning thread.
    /// This version is not serializable, since <see cref="Dispatcher"/> is not.
    /// </summary>
    /// <seealso cref="TomsToolbox.Desktop.ObservableObjectBase" />
    public abstract class ObservableObject : ObservableObjectBase
    {
        [NotNull]
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets the dispatcher of the thread where this object was created.
        /// </summary>
        [NotNull]
        public Dispatcher Dispatcher
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);
                return _dispatcher;
            }
        }


        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_dispatcher != null);
        }
    }
}

