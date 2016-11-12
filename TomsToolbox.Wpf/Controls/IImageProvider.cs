namespace TomsToolbox.Wpf.Controls
{
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

    /// <summary>
    /// Implemented by image providers for the <see cref="Map"/> control.
    /// </summary>
    [ContractClass(typeof (ImageProviderContract))]
    public interface IImageProvider
    {
        /// <summary>
        /// Gets the minimum zoom factor supported by this provider.
        /// </summary>
        int MinZoom
        {
            get;
        }

        /// <summary>
        /// Gets the maximum zoom factor supported by this provider.
        /// </summary>
        int MaxZoom
        {
            get;
        }

        /// <summary>
        /// Gets the image for a map tile.
        /// </summary>
        /// <param name="tile">The tile for which to provide the image.</param>
        /// <returns>The image.</returns>
        IImage GetImage(IMapTile tile);
    }

    [ContractClassFor(typeof(IImageProvider))]
    abstract class ImageProviderContract : IImageProvider
    {
        int IImageProvider.MinZoom
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        int IImageProvider.MaxZoom
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        [NotNull]
        IImage IImageProvider.GetImage([NotNull] IMapTile tile)
        {
            Contract.Requires(tile != null);
            Contract.Ensures(Contract.Result<IImage>() != null);

            throw new System.NotImplementedException();
        }
    }
}