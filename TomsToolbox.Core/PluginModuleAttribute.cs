namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Denotes an assembly as a loadable module.<para/>
    /// Categories may be assigned to implement dynamic loading of modules by category. 
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
#if !PORTABLE
    [Serializable]
#endif
    public sealed class PluginModuleAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginModuleAttribute"/> class.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public PluginModuleAttribute(params string[] categories)
        {
            Contract.Requires(categories != null);
            Categories = categories;
        }

        /// <summary>
        /// Gets the categories for this module.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification="Can't use anything else in attributes. Property must match constructor parameter.")]
        public string[] Categories
        {
            get;
            private set;
        }

#if !PORTABLE
        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Categories != null);
        }
#endif  
    }
}
