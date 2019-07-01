namespace TomsToolbox.Wpf.Composition
{
    using System.ComponentModel;

    /// <summary>
    /// Base class for items that export a selectable item for a selector control. 
    /// </summary>
    public interface ISelectableComposablePart : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        bool IsSelected
        {
            get;
            set;
        }
    }
}
