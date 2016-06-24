namespace TomsToolbox.Core
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Reflection;

    ///<summary>
    /// Provides support for extracting property information based on a property expression.
    ///</summary>
    public static class PropertySupport
    {
        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. () => p.PropertyName) to extract the property name from.</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Works only with exactly this kind of expression, so we don't want to allow to pass something else!")]
        public static string ExtractPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return ExtractPropertyName(propertyExpression, true);
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. () => p.PropertyName) to extract the property name from.</param>
        /// <returns>The name of the property, or null if the extraction fails.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Works only with exactly this kind of expression, so we don't want to allow to pass something else!")]
        public static string TryExtractPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);

            return ExtractPropertyName(propertyExpression, false);
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p =&gt; p.PropertyName) to extract the property name from.</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br />
        /// Not a <see cref="MemberExpression" /><br />
        /// The <see cref="MemberExpression" /> does not represent a property.<br /></exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Works only with exactly this kind of expression, so we don't want to allow to pass something else!")]
        public static string ExtractPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return ExtractPropertyName(propertyExpression, true);
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName) to extract the property name from.</param>
        /// <returns>The name of the property, or null if the extraction fails.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Works only with exactly this kind of expression, so we don't want to allow to pass something else!")]
        public static string TryExtractPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);

            return ExtractPropertyName(propertyExpression, false);
        }

        private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression, bool failOnErrors)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures((failOnErrors == false) || !string.IsNullOrEmpty(Contract.Result<string>()));

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                return HandleError(failOnErrors, @"Expression is not a member access expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                return HandleError(failOnErrors, @"Expression is not a property expression");

            var memberName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(memberName))
                return HandleError(failOnErrors, @"Expression is not a valid property expression");

            return memberName;
        }

        private static string ExtractPropertyName<T, TR>(Expression<Func<T, TR>> propertyExpression, bool failOnErrors)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures((failOnErrors == false) || !string.IsNullOrEmpty(Contract.Result<string>()));

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                return HandleError(failOnErrors, @"Expression is not a member access expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                return HandleError(failOnErrors, @"Expression is not a property expression");

            var memberName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(memberName))
                return HandleError(failOnErrors, @"Expression is not a valid property expression");

            return memberName;
        }

        private static string HandleError(bool failOnErrors, string errorMessage)
        {
            Contract.Requires(errorMessage != null);
            Contract.Ensures((failOnErrors == false) || !string.IsNullOrEmpty(Contract.Result<string>()));

            if (failOnErrors)
                throw new ArgumentException(errorMessage);

            return null;
        }

        /// <summary>
        /// Gets the <see cref="PropertyChangedEventArgs"/> for the specified property.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName) to extract the property name from.</param>
        /// <returns>The event arguments to pass to <see cref="INotifyPropertyChanged.PropertyChanged"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        /// </exception>
        [ContractVerification(false)]
        public static PropertyChangedEventArgs GetEventArgs<T>(Expression<Func<T>> propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Contract.Ensures(Contract.Result<PropertyChangedEventArgs>() != null);
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<PropertyChangedEventArgs>().PropertyName));

            return new PropertyChangedEventArgs(ExtractPropertyName(propertyExpression));
        }
    }
}
