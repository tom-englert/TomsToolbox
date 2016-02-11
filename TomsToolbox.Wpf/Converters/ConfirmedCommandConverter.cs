﻿namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// A converter to use in <see cref="ICommand"/> bindings to intercept or filter command executions in the view layer in MVVM applications.
    /// </summary>
    [ValueConversion(typeof(ICommand), typeof(CommandProxy))]
    public class ConfirmedCommandConverter : ValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// Null and UnSet are unchanged.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// </returns>
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new CommandProxy(this, (ICommand)value);
        }

        /// <summary>
        /// Occurs when the command is being executed. The view can connect to this event to e.g. show a message box or modify the command parameter.
        /// </summary>
        public event EventHandler<ConfirmedCommandEventArgs> Executing;

        /// <summary>
        /// Occurs when an exception is raised during command execution.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        private void QueryCancelExecution(ConfirmedCommandEventArgs e)
        {
            Contract.Requires(e != null);

            var handler = Executing;
            if (handler != null)
                handler.Invoke(this, e);
        }

        private void OnError(ErrorEventArgs e)
        {
            var handler = Error;
            if (handler != null)
                handler.Invoke(this, e);
        }

        private class CommandProxy : ICommand
        {
            private readonly ConfirmedCommandConverter _owner;
            private readonly ICommand _command;

            public CommandProxy(ConfirmedCommandConverter owner, ICommand command)
            {
                Contract.Requires(owner != null);
                Contract.Requires(command != null);
                _owner = owner;
                _command = command;
            }

            public void Execute(object parameter)
            {
                var args = new ConfirmedCommandEventArgs { Parameter = parameter };

                _owner.QueryCancelExecution(args);

                if (args.Cancel)
                    return;

                try
                {
                    _command.Execute(args.Parameter);
                }
                catch (Exception ex)
                {
                    _owner.OnError(new ErrorEventArgs(ex));
                }
            }

            public bool CanExecute(object parameter)
            {
                return _command.CanExecute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { _command.CanExecuteChanged += value; }
                remove { _command.CanExecuteChanged -= value; }
            }

            [ContractInvariantMethod]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_owner != null);
                Contract.Invariant(_command != null);
            }
        }
    }
}
