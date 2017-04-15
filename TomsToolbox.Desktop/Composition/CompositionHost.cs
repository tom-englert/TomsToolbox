namespace TomsToolbox.Desktop.Composition
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Implementation of <see cref="ICompositionHost"/>
    /// </summary>
    public sealed class CompositionHost : ICompositionHost, IServiceProvider
    {
        [NotNull]
        private readonly CompositionContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionHost" /> class with a container that is thread safe.
        /// </summary>
        public CompositionHost()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionHost" /> class.
        /// </summary>
        /// <param name="isThreadSafe">if set to <c>true</c> if the container is thread safe.</param>
        public CompositionHost(bool isThreadSafe)
        {
            var catalog = new AggregateCatalog();
            Contract.Assume(catalog.Catalogs != null);
            _container = new CompositionContainer(catalog, isThreadSafe);
            _container.ComposeExportedValue((ICompositionHost)this);
            _container.ComposeExportedValue((IServiceProvider)this);
            _container.ComposeExportedValue((ExportProvider)Container);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public CompositionContainer Container => _container;

        /// <summary>
        /// Gets the catalog of the container.
        /// </summary>
        public AggregateCatalog Catalog
        {
            get
            {
                var result = (AggregateCatalog)Container.Catalog;

                Contract.Assume(result != null);
                Contract.Assume(result.Catalogs != null);

                return result;
            }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        [CanBeNull]
        public object GetService([NotNull] Type serviceType)
        {
            return Container.GetExports(serviceType, null, string.Empty).Select(item => item?.Value).FirstOrDefault();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CompositionHost"/> class.
        /// </summary>
        ~CompositionHost()
        {
            this.ReportNotDisposedObject();
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_container != null);
        }
    }
}