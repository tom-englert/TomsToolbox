namespace TomsToolbox.Wpf.Styles
{
    using System.Diagnostics.Contracts;
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// The interface to be implemented and exported by application specific theme resource providers. 
    /// Theme resource providers can load additional theme specific resources into the resource dictionary.
    /// </summary>
    [ContractClass(typeof(ThemeResourceProviderContract))]
    public interface IThemeResourceProvider
    {
        /// <summary>
        /// Called by the styles to loads the theme specific resources.
        /// </summary>
        /// <param name="resource">The resource where the provider can add the theme specific resource.</param>
        void LoadThemeResources([NotNull] ResourceDictionary resource);
    }

    [ContractClassFor(typeof(IThemeResourceProvider))]
    internal abstract class ThemeResourceProviderContract : IThemeResourceProvider
    {
        public void LoadThemeResources(ResourceDictionary resource)
        {
            Contract.Requires(resource != null);
            throw new System.NotImplementedException();
        }
    }
}
