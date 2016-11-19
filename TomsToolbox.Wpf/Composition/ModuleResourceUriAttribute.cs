namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

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
        public ModuleResourceUriAttribute([NotNull] string uri)
        {
            Contract.Requires(uri != null);
            Uri = uri;
        }

        /// <summary>
        /// Gets the URI of the resource.
        /// </summary>
        [NotNull]
        public string Uri
        {
            get;
        }
    }
}
