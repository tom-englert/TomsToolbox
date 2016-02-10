namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
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
    public abstract class ObservableObject : INotifyPropertyChanged, IDataErrorInfo
#if NETFRAMEWORK_4_5
        , INotifyDataErrorInfo
#endif
    {
        private static readonly AutoWeakIndexer<Type, IDictionary<string, IEnumerable<string>>> DependencyMappingCache = new AutoWeakIndexer<Type, IDictionary<string, IEnumerable<string>>>(type => PropertyDependencyAttribute.CreateDependencyMapping(type.GetProperties()));
        private IDictionary<string, IEnumerable<string>> _dependencyMapping;

        private static readonly AutoWeakIndexer<Type, IDictionary<Type, IDictionary<string, string>>> RelayMappingCache = new AutoWeakIndexer<Type, IDictionary<Type, IDictionary<string, string>>>(type => RelayedEventAttribute.CreateRelayMapping(type.GetProperties()));
        private IDictionary<Type, IDictionary<string, string>> _relayMapping;
        private Dictionary<Type, INotifyPropertyChanged> _eventSources;

        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets the dispatcher of the thread where this object was created.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get
            {
                Contract.Ensures(Contract.Result<Dispatcher>() != null);
                return _dispatcher;
            }
        }

        /// <summary>
        /// Relays the property changed events of the source object (if not null) and detaches the old source (if not null).
        /// </summary>
        /// <param name="oldSource"></param>
        /// <param name="newSource"></param>
        protected void RelayEventsOf(INotifyPropertyChanged oldSource, INotifyPropertyChanged newSource)
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
        protected void RelayEventsOf(INotifyPropertyChanged source)
        {
            Contract.Requires(source != null);

            var sourceType = source.GetType();
            if (!RelayMapping.Keys.Any(key => key.IsAssignableFrom(sourceType)))
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
        protected void DetachEventSource(INotifyPropertyChanged item)
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
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> propertyExpression)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> propertyExpression, Action<T, T> changeCallback)
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
        protected bool SetProperty<T>(ref T backingField, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
#else
        protected bool SetProperty<T>(ref T backingField, T value, string propertyName)
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
        protected bool SetProperty<T>(ref T backingField, T value, string propertyName, Action<T, T> changeCallback)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));
            Contract.Requires(changeCallback != null);

            var oldValue = backingField;

            if (!SetProperty(ref backingField, value, propertyName))
                return false;

            changeCallback(oldValue, value);
            return true;
        }

#if NETFRAMEWORK_4_5
        /// <summary>
        /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field for the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property; omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
        /// <param name="changeCallback">The callback that is invoked if the value has changed. Parameters are (oldValue, newValue).</param>
        /// <returns> True if value has changed and the PropertyChange event was raised. </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This pattern is required by the CallerMemberName attribute.")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>(ref T backingField, T value, Action<T, T> changeCallback, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));
            Contract.Requires(changeCallback != null);

            return SetProperty<T>(ref backingField, value, propertyName, changeCallback);
        }
#endif

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property; <c>.Net 4.5 only:</c> omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This pattern is required by the CallerMemberName attribute.")]
        [NotifyPropertyChangedInvocator]
#if NETFRAMEWORK_4_5
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
#else
        protected void OnPropertyChanged(string propertyName)
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

        private Dictionary<Type, INotifyPropertyChanged> EventSources
        {
            get
            {
                return _eventSources ?? (_eventSources = new Dictionary<Type, INotifyPropertyChanged>());
            }
        }

        private IDictionary<Type, IDictionary<string, string>> RelayMapping
        {
            get
            {
                return _relayMapping ?? (_relayMapping = RelayMappingCache[GetType()]);
            }
        }

        private IDictionary<string, IEnumerable<string>> DependencyMapping
        {
            get
            {
                return _dependencyMapping ?? (_dependencyMapping = DependencyMappingCache[GetType()]);
            }
        }

        private void RelaySource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Contract.Requires(sender != null);

            if (e.PropertyName == null)
                return;

            var sourceType = sender.GetType();
            foreach (var mapping in RelayMapping.Where(item => item.Key.IsAssignableFrom(sourceType)).Select(item => item.Value))
            {
                Contract.Assume(mapping != null);

                string targetPropertyName;
                if (mapping.TryGetValue(e.PropertyName, out targetPropertyName) && !string.IsNullOrEmpty(targetPropertyName))
                {
                    OnPropertyChanged(targetPropertyName);
                }
            }
        }

        private void InternalOnPropertyChanged(string propertyName)
        {
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            var eventHander = PropertyChanged;
            if (eventHander != null)
            {
                eventHander(this, new PropertyChangedEventArgs(propertyName));
            }
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
        protected virtual IEnumerable<string> GetDataErrors(string propertyName)
        {
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            if (string.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            var property = GetType().GetProperty(propertyName);
            if (property == null)
                return Enumerable.Empty<string>();

            var errorInfos = property.GetCustomAttributes<ValidationAttribute>(true)
                .Where(va => !va.IsValid(property.GetValue(this, null)))
                .Select(va => va.FormatErrorMessage(propertyName));

            return errorInfos;
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return GetDataErrors(null).FirstOrDefault();
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return GetDataErrors(columnName).FirstOrDefault();
            }
        }

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
            return GetDataErrors(propertyName);
        }

        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return GetDataErrors(null).Any();
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

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_dispatcher != null);
        }
    }
}

