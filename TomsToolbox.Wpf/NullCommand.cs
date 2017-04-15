namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    using JetBrains.Annotations;

    /// <summary>
    /// A <see cref="ICommand"/> implementation that does nothing and can't be executed. 
    /// </summary>
    /// <remarks>
    /// Useful as fallback for command bindings, since a binding to <c>null</c> will leave the bound control enabled.
    /// </remarks>
    public class NullCommand : ICommand
    {
        /// <summary>
        /// The singleton instance of the command.
        /// </summary>
        public static readonly ICommand Default = new NullCommand();

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute([CanBeNull] object parameter)
        {
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute([CanBeNull] object parameter)
        {
            Contract.Ensures(Contract.Result<bool>() == false);
            return false;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
