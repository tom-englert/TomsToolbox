namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Various extension methods.
    /// </summary>
    public static class CustomAttributeProviderExtensions
    {
        /// <summary>
        /// Returns a list of custom attributes identified by the type. <see cref="MemberInfo.GetCustomAttributes(Type, bool)"/>
        /// </summary>
        /// <typeparam name="T">The type of attributes to return.</typeparam>
        /// <param name="self">The member info of the object to evaluate.</param>
        /// <param name="inherit">Specifies whether to search this member's inheritance chain to find the attributes.</param>
        /// <returns>An array of custom attributes applied to this member, or an array with zero (0) elements if no attributes have been applied.</returns>
        /// <exception cref="System.TypeLoadException">A custom attribute type cannot be loaded</exception>
        /// <exception cref="System.InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.</exception>
        [ItemNotNull]
        [NotNull]
        public static IEnumerable<T> GetCustomAttributes<T>([NotNull] this ICustomAttributeProvider self, bool inherit)
        {
            Contract.Requires(self != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return self.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }


        /// <summary>
        /// Get the value of the DisplayNameAttribute associated with the given item.
        /// </summary>
        /// <param name="item">The item to lookup. This can be a MemberInfo like FieldInfo, PropertyInfo...</param>
        /// <returns>The associated display name, or null if the item does not have a DisplayName attribute.</returns>
        public static string TryGetDisplayName([NotNull] this ICustomAttributeProvider item)
        {
            Contract.Requires(item != null);

            return item.GetCustomAttributes<DisplayNameAttribute>(false)
                .Select(attr => attr.DisplayName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the value of the DescriptionAttribute associated with the given item.
        /// </summary>
        /// <param name="item">The item to lookup. This can be a MemberInfo like FieldInfo, PropertyInfo...</param>
        /// <returns>The associated description, or null if the item does not have a Description attribute.</returns>
        public static string TryGetDescription([NotNull] this ICustomAttributeProvider item)
        {
            Contract.Requires(item != null);

            return item.GetCustomAttributes<DescriptionAttribute>(false)
                .Select(attr => attr.Description)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the value of the TextAttribute with the specified key that is associated with the given item.
        /// </summary>
        /// <param name="item">The item to lookup. This can be a MemberInfo like FieldInfo, PropertyInfo...</param>
        /// <param name="key">The key.</param>
        /// <returns>The associated text, or null if the item does not have a text attribute with this key.</returns>
        public static string TryGetText([NotNull] this ICustomAttributeProvider item, [NotNull] object key)
        {
            Contract.Requires(item != null);
            Contract.Requires(key != null);

            return item.GetCustomAttributes<TextAttribute>(false)
                .Where(attr => Equals(attr.Key, key))
                .Select(attr => attr.Text)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the custom <see cref="TypeConverter" /> declared with the <see cref="TypeConverterAttribute"/> on the specified item.
        /// </summary>
        /// <param name="item">The item to look up.</param>
        /// <returns>
        /// The custom type converter, or null if the item has no custom type converter attribute.
        /// </returns>
        public static TypeConverter GetCustomTypeConverter([NotNull] this ICustomAttributeProvider item)
        {
            Contract.Requires(item != null);

            return item
                .GetCustomAttributes<TypeConverterAttribute>(false)
                .Select(attr => Type.GetType(attr.ConverterTypeName))
                .Where(type => (type != null) && typeof(TypeConverter).IsAssignableFrom(type))
                .Select(type => (TypeConverter)Activator.CreateInstance(type))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the sequence associated with the specified target.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="defaultValue">The default value, if the object has no sequence attribute.</param>
        /// <returns>The associated sequence.</returns>
        public static double GetSequence([NotNull] this ICustomAttributeProvider target, double defaultValue)
        {
            Contract.Requires(target != null);

            return target.GetCustomAttributes<SequenceAttribute>(false)
                .Select(attr => attr.Value)
                .DefaultIfEmpty(defaultValue)
                .FirstOrDefault();
        }
    }
}
