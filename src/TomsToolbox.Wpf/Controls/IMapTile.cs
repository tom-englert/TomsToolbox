namespace TomsToolbox.Wpf.Controls
{
    /// <summary>
    /// Implemented by representations of a map tile.
    /// </summary>
    public interface IMapTile
    {
        /// <summary>
        /// Gets the horizontal index of this tile.
        /// </summary>
        int X
        {
            get;
        }

        /// <summary>
        /// Gets the vertical index of this tile.
        /// </summary>
        int Y
        {
            get;
        }

        /// <summary>
        /// Gets the zoom level of this tile.
        /// </summary>
        int ZoomLevel
        {
            get;
        }

        /// <summary>
        /// Gets the parent tile, or <c>null</c> if this is the root tile.
        /// </summary>
        IMapTile? Parent
        {
            get;
        }

        /// <summary>
        /// Unloads this instance when the tile is no longer visible.
        /// </summary>
        void Unload();
    }
}