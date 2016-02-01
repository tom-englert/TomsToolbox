namespace TomsToolbox.Wpf.Composition
{
    using System.Windows.Input;

    /// <summary>
    /// A command proxy to handle checkable menu items or toggle buttons.
    /// </summary>
    interface ICheckableCommand : ICommand
    {
        bool IsChecked
        {
            get;
            set;
        }
    }
}
