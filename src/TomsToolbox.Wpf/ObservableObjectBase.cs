namespace TomsToolbox.Wpf;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;

using TomsToolbox.Essentials;

/// <summary>
/// Base class implementing <see cref="INotifyPropertyChanged"/>.<para/>
/// Supports declarative dependencies specified by the <see cref="PropertyDependencyAttribute"/>.
/// </summary>
/// <remarks>
/// Also implements <see cref="IDataErrorInfo"/> and <see cref="INotifyDataErrorInfo"/> to support validation.
/// The default implementation examines <see cref="ValidationAttribute"/> on the affected properties to retrieve error information.
/// </remarks>
[Serializable]
[DataContract(IsReference = true)]
public abstract partial class ObservableObjectBase : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
{
    private static readonly AutoWeakIndexer<Type, IDictionary<string, IEnumerable<string>>> _dependencyMappingCache = new(PropertyDependencyAttribute.CreateDependencyMapping);
    [NonSerialized]
    private IDictionary<string, IEnumerable<string>>? _dependencyMapping;

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="propertyExpression">The expression identifying the property.</param>
    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
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
    [NotifyPropertyChangedInvocator]
    protected bool SetProperty<T>(ref T? backingField, T? value, Expression<Func<T>> propertyExpression)
    {
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
    [NotifyPropertyChangedInvocator]
    protected bool SetProperty<T>(ref T? backingField, T? value, Expression<Func<T>> propertyExpression, Action<T, T> changeCallback)
    {
        return SetProperty(ref backingField, value, PropertySupport.ExtractPropertyName(propertyExpression), changeCallback);
    }

    /// <summary>
    /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="backingField">The backing field for the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="propertyName">Name of the property; omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
    /// <returns>True if value has changed and the PropertyChange event was raised. </returns>
    [NotifyPropertyChangedInvocator]
    protected bool SetProperty<T>(ref T? backingField, T? value, [CallerMemberName] string propertyName = null!)
    {
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
    [NotifyPropertyChangedInvocator]
    protected bool SetProperty<T>(ref T? backingField, T? value, string propertyName, Action<T, T> changeCallback)
    {
        var oldValue = backingField;

        if (!SetProperty(ref backingField, value, propertyName))
            return false;

        changeCallback(oldValue!, value!);
        return true;
    }

    /// <summary>
    /// Sets the property and raises the <see cref="PropertyChanged" /> event for the property identified by the specified property expression.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="backingField">The backing field for the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="changeCallback">The callback that is invoked if the value has changed. Parameters are (oldValue, newValue).</param>
    /// <param name="propertyName">Name of the property; omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
    /// <returns> True if value has changed and the PropertyChange event was raised. </returns>
    [NotifyPropertyChangedInvocator]
    protected bool SetProperty<T>(ref T? backingField, T? value, Action<T, T> changeCallback, string propertyName)
    {
        return SetProperty(ref backingField, value, propertyName, changeCallback);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the property with the specified name.
    /// </summary>
    /// <param name="propertyName">Name of the property; omit this parameter to use the callers name provided by the CallerMemberNameAttribute</param>
    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        InternalOnPropertyChanged(propertyName);

        if (!DependencyMapping.TryGetValue(propertyName, out var dependentProperties))
            return;

        foreach (var dependentProperty in dependentProperties)
        {
            InternalOnPropertyChanged(dependentProperty);
        }
    }

    private IDictionary<string, IEnumerable<string>> DependencyMapping => _dependencyMapping ??= _dependencyMappingCache[GetType()];

    private void InternalOnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

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
    protected virtual IEnumerable<string> GetDataErrors(string? propertyName)
    {
        if (propertyName.IsNullOrEmpty())
            return [];

        var property = GetType().GetProperty(propertyName);
        if (property == null)
            return [];

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
    // ReSharper disable UnusedParameter.Global
    protected virtual void OnDataErrorsEvaluated(string? propertyName, IEnumerable<string>? dataErrors)
        // ReSharper restore UnusedParameter.Global
    {
    }

    private IEnumerable<string> InternalGetDataErrors(string? propertyName)
    {
        var dataErrors = GetDataErrors(propertyName).ToArray();

        OnDataErrorsEvaluated(propertyName, dataErrors);

        return dataErrors;
    }

    string IDataErrorInfo.Error => InternalGetDataErrors(null).FirstOrDefault() ?? string.Empty;

    string IDataErrorInfo.this[string? columnName] => InternalGetDataErrors(columnName).FirstOrDefault() ?? string.Empty;

    private event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Raises the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property where validation errors have changed; or null or <see cref="F:System.String.Empty"/>, when entity-level errors have changed.</param>
    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    System.Collections.IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
    {
        return InternalGetDataErrors(propertyName);
    }

    bool INotifyDataErrorInfo.HasErrors => InternalGetDataErrors(null).Any();

    event EventHandler<DataErrorsChangedEventArgs>? INotifyDataErrorInfo.ErrorsChanged
    {
        add => ErrorsChanged += value;
        remove => ErrorsChanged -= value;
    }
}
