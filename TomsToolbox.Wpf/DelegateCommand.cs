namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    using TomsToolbox.Core;

    /// <summary>
    /// A simple, straight forward delegate command implementation. For usage see MVVM concepts.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// <para/>
        /// No callback is initially set, so they must be set via the property setters. This usage generates easy readable code even if the delegates are inlined.
        /// </summary>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand DeleteCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand<Item>
        ///         {
        ///             CanExecuteCallback = item =>
        ///             {
        ///                 return IsSomethingSelected(item);
        ///             },
        ///             ExecuteCallback = item =>
        ///             {
        ///                 if (IsSomehingSelected(item))
        ///                 {
        ///                     DelteTheSelection();
        ///                 }
        ///             }
        ///         };
        ///     }
        /// }
        /// ]]></code></example>
        public DelegateCommand()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class with the execute callback.
        /// <para/>
        /// This version generates more compact code; not recommended for in-line delegates.
        /// </summary>
        /// <param name="executeCallback">The default execute callback.</param>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand AboutCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand<Item>(item => ShowAboutBox(item));
        ///     }
        /// }
        /// ]]></code></example>
        public DelegateCommand(Action<T> executeCallback)
            : this(null, executeCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
        /// </summary>
        /// <param name="canExecuteCallback">The default can execute callback.</param>
        /// <param name="executeCallback">The default execute callback.</param>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand EditCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand<Item>(CanEdit, Edit);
        ///     }
        /// }
        ///
        /// public bool CanEdit(Item param)
        /// {
        ///     .....
        /// ]]></code></example>
        public DelegateCommand(Predicate<T> canExecuteCallback, Action<T> executeCallback)
        {
            ExecuteCallback = executeCallback;
            CanExecuteCallback = canExecuteCallback;
        }

        /// <summary>
        /// Gets or sets the predicate to handle the ICommand.CanExecute method.
        /// If unset, ICommand.CanExecute will always return true if ExecuteCallback is set.
        /// </summary>
        public Predicate<T> CanExecuteCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action to handle the ICommand.Execute method.
        /// If unset, ICommand.CanExecute will always return false.
        /// </summary>
        public Action<T> ExecuteCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <remarks>
        /// The event is forwarded to the <see cref="CommandManager"/>, so visuals bound to the delegate command will be updated
        /// in sync with the system. To explicitly refresh all visuals call CommandManager.InvalidateRequerySuggested();
        /// </remarks>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (ExecuteCallback == null)
            {
                return false;
            }

            if (CanExecuteCallback == null)
            {
                return true;
            }

            return CanExecuteCallback(parameter.SafeCast<T>());
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (ExecuteCallback != null)
            {
                ExecuteCallback(parameter.SafeCast<T>());
            }
        }
    }

    /// <summary>
    /// A simple, straight forward delegate command implementation that does not make use of the command parameter.
    /// For usage see MVVM concepts.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// <para/>
        /// No callback is initially set, so they must be set via the property setters. This usage generates easy readable code even if the delegates are inlined.
        /// </summary>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand DeleteCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand
        ///         {
        ///             CanExecuteCallback = delegate
        ///             {
        ///                 return IsSomethingSelected();
        ///             },
        ///             ExecuteCallback = delegate
        ///             {
        ///                 if (IsSomehingSelected())
        ///                 {
        ///                     DelteTheSelection();
        ///                 }
        ///             }
        ///         };
        ///     }
        /// }
        /// ]]></code></example>
        public DelegateCommand()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class with the execute callback.
        /// <para/>
        /// This version generates more compact code; not recommended for in-line delegates.
        /// </summary>
        /// <param name="executeCallback">The default execute callback.</param>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand AboutCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand(ShowAboutBox);
        ///     }
        /// }
        /// ]]></code></example>
        public DelegateCommand(Action executeCallback)
            : this(null, executeCallback)
        {
            Contract.Requires(executeCallback != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="canExecuteCallback">The default can execute callback.</param>
        /// <param name="executeCallback">The default execute callback.</param>
        /// <example><code language="C#"><![CDATA[
        /// public ICommand EditCommand
        /// {
        ///     get
        ///     {
        ///         return new DelegateCommand(CanEdit, Edit);
        ///     }
        /// }
        ///
        /// public bool CanEdit()
        /// {
        ///     .....
        /// ]]></code></example>
        public DelegateCommand(Func<bool> canExecuteCallback, Action executeCallback)
        {
            CanExecuteCallback = canExecuteCallback;
            ExecuteCallback = executeCallback;
        }

        /// <summary>
        /// Gets or sets the predicate to handle the ICommand.CanExecute method.
        /// If unset, ICommand.CanExecute will always return true if ExecuteCallback is set.
        /// </summary>
        public Func<bool> CanExecuteCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action to handle the ICommand.Execute method.
        /// If unset, ICommand.CanExecute will always return false.
        /// </summary>
        public Action ExecuteCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <remarks>
        /// The event is forwarded to the <see cref="CommandManager"/>, so visuals bound to the delegate command will be updated
        /// in sync with the system. To explicitly refresh all visuals call CommandManager.InvalidateRequerySuggested();
        /// </remarks>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (ExecuteCallback == null)
            {
                return false;
            }

            if (CanExecuteCallback == null)
            {
                return true;
            }

            return CanExecuteCallback();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (ExecuteCallback != null)
            {
                ExecuteCallback();
            }
        }
    }
}