namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Attribute to associate a resource with a module; when a module is loaded dynamically, the resource can be linked into the application resource scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple=true)]
    [Serializable]
    public sealed class ModuleResourceUriAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleResourceUriAttribute"/> class.
        /// </summary>
        /// <param name="uri">The URI of the resource.</param>
        public ModuleResourceUriAttribute(string uri)
        {
            Contract.Requires(uri != null);
            Uri = uri;
        }

        /// <summary>
        /// Gets the URI of the resource.
        /// </summary>
        public string Uri
        {
            get;
            private set;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Uri != null);
        }
    }
}
