namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    /// <summary>
    /// Extension methods for <see cref="System.Xml.Linq"/> objects.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Gets the attribute value of an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value of the attribute, or <c>null</c> if no such attribute exists</returns>
        public static string GetAttribute(this XElement element, string name)
        {
            Contract.Requires(element != null);
            Contract.Requires(!string.IsNullOrEmpty(name));

            return GetAttribute(element, name, null);
        }

        /// <summary>
        /// Gets the attribute value of an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value of the attribute, or the <paramref name="defaultValue" /> if no such attribute exists
        /// </returns>
        public static string GetAttribute(this XElement element, string name, string defaultValue)
        {
            Contract.Requires(element != null);
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures((defaultValue == null) || (Contract.Result<string>() != null));

            return GetAttribute(element, XName.Get(name), defaultValue);
        }

        /// <summary>
        /// Gets the attribute value of an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value of the attribute, or <c>null</c> if no such attribute exists</returns>
        public static string GetAttribute(this XElement element, XName name)
        {
            Contract.Requires(element != null);
            Contract.Requires(name != null);

            return GetAttribute(element, name, null);
        }

        /// <summary>
        /// Gets the attribute value of an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value of the attribute, or the <paramref name="defaultValue"/> if no such attribute exists
        /// </returns>
        public static string GetAttribute(this XElement element, XName name, string defaultValue)
        {
            Contract.Requires(element != null);
            Contract.Requires(name != null);
            Contract.Ensures((defaultValue == null) || (Contract.Result<string>() != null));

            var attribute = element.Attribute(name);

            return attribute != null ? attribute.Value : defaultValue;
        }
    }
}
