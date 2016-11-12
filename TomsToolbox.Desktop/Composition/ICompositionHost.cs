namespace TomsToolbox.Desktop.Composition
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

    /// <summary>
    /// Class hosting a <see cref="CompositionContainer"/> with an <see cref="AggregateCatalog"/>.
    /// </summary>
    [ContractClass(typeof(CompositionHostContracts))]
    public interface ICompositionHost : IDisposable
    {
        /// <summary>
        /// Gets the composition container.
        /// </summary>
        CompositionContainer Container
        {
            get;
        }

        /// <summary>
        /// Gets the catalog of the container.
        /// </summary>
        AggregateCatalog Catalog
        {
            get;
        }
    }

    [ContractClassFor(typeof(ICompositionHost))]
    abstract class CompositionHostContracts : ICompositionHost
    {
        [NotNull]
        public CompositionContainer Container
        {
            get
            {

                Contract.Ensures(Contract.Result<CompositionContainer>() != null);
                throw new NotImplementedException();
            }
        }

        [NotNull]
        public AggregateCatalog Catalog
        {
            get
            {
                Contract.Ensures(Contract.Result<AggregateCatalog>() != null);
                Contract.Ensures(Contract.Result<AggregateCatalog>().Catalogs != null);

                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
