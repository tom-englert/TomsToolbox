namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;

    /// <summary>
    /// Denotes an assembly as a loadable module.<para/>
    /// Categories may be assigned to implement dynamic loading of modules by category. 
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
#if !PORTABLE && !NETSTANDARD1_0
    [Serializable]
#endif
    public sealed class PluginModuleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginModuleAttribute"/> class.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public PluginModuleAttribute([NotNull, ItemNotNull] params string[] categories)
        {
            Categories = categories;
        }

        /// <summary>
        /// Gets the categories for this module.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Can't use anything else in attributes. Property must match constructor parameter.")]
        [NotNull, ItemNotNull]
        public string[] Categories
        {
            get;
        }
    }
}
