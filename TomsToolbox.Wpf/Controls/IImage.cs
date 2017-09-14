namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// Implemented by image providers to provide a dynamically loadable image.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        /// Gets the source of the image.
        /// </summary>
        [NotNull]
        ImageSource Source
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the image of this instance is loaded.
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// Occurs when the image is loaded.
        /// </summary>
        event EventHandler Loaded;
    }
}
