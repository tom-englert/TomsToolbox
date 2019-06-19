namespace TomsToolbox.Desktop.Composition
{
    using System;
    using System.ComponentModel.Composition.Hosting;

    using JetBrains.Annotations;

    /// <summary>
    /// Class hosting a <see cref="CompositionContainer"/> with an <see cref="AggregateCatalog"/>.
    /// </summary>
    public interface ICompositionHost : IDisposable
    {
        /// <summary>
        /// Gets the composition container.
        /// </summary>
        [NotNull]
        CompositionContainer Container
        {
            get;
        }

        /// <summary>
        /// Gets the catalog of the container.
        /// </summary>
        [NotNull]
        AggregateCatalog Catalog
        {
            get;
        }
    }
}
