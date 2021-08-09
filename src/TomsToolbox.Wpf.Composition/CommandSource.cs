namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Command declaration to be used with visual composition.
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    /// <inheritdoc />
    public class CommandSource : DependencyObject
    {
        private readonly ICommandSourceFactory _owner;
        private readonly List<ICommand> _attachedCommands = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Composition.CommandSource" /> class.
        /// </summary>
        /// <param name="owner">The command source factory.</param>
        public CommandSource(ICommandSourceFactory owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the command represented by this <see cref="CommandSourceFactory" />. This can be bound to a menu's or button's Command property.
        /// </summary>
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            private set => SetValue(_commandPropertyKey, value);
        }
        private static readonly DependencyPropertyKey _commandPropertyKey =
            DependencyProperty.RegisterReadOnly("Command", typeof(ICommand), typeof(CommandSource), new FrameworkPropertyMetadata(NullCommand.Default, null, Command_CoerceValue));
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = _commandPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked. This can be bound to a checkable menu item or toggle button.
        /// </summary>
        public bool IsChecked
        {
            get => this.GetValue<bool>(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="IsChecked"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CommandSource));


        private static object Command_CoerceValue(DependencyObject? d, object? basevalue)
        {
            return basevalue ?? NullCommand.Default;
        }

        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        public object? Header => _owner.Header;

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        public object? Description => _owner.Description;

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        public object? Icon => _owner.Icon;

        /// <summary>
        /// Gets a value indicating whether to show the header text when this command is bound to a button. 
        /// If false, only the icon should be displayed.
        /// </summary>
        public bool ShowTextOnButtons => _owner.ShowTextOnButtons;

        /// <summary>
        /// Gets the id of the region sub-items can register for.
        /// </summary>
        public string? SubRegionId => _owner.SubRegionId;

        /// <summary>
        /// Gets a value indicating whether the control associated with this instance should be checkable, 
        /// e.g. a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> or a <see cref="ToggleButton"/> in a tool bar.
        /// </summary>
        public bool IsCheckable => _owner.IsCheckable;

        /// <summary>
        /// Gets the name of the group that this command belongs to.
        /// If different group names are specified for a target region, the commands can be grouped and the groups separated by a <see cref="Separator" />.
        /// </summary>
        public virtual object? GroupName => _owner.GroupName;

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        public object? Tag => _owner.Tag;

        /// <summary>
        /// Gets a value indicating whether any target is attached to this source.
        /// Some controls like tool bars may want to hide commands that have no target attached rather than only disabling them.
        /// </summary>
        public bool IsAnyTargetAttached
        {
            get => this.GetValue<bool>(IsAnyTargetAttachedProperty);
            private set => SetValue(_isAnyTargetAttachedPropertyKey, value);
        }
        private static readonly DependencyPropertyKey _isAnyTargetAttachedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsAnyTargetAttached", typeof(bool), typeof(CommandSource), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="IsAnyTargetAttached"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsAnyTargetAttachedProperty = _isAnyTargetAttachedPropertyKey.DependencyProperty;

        /// <summary>
        /// Attaches the specified command. The last command attached will become the active command, while the previous command will be pushed on a stack.
        /// </summary>
        /// <param name="command">The command.</param>
        internal void Attach(ICommand command)
        {
            _attachedCommands.Insert(0, command);

            SetCommand(_attachedCommands.FirstOrDefault());
        }

        /// <summary>
        /// Detaches the specified command. If the detached command was the active command, the previous command from the stack will become the active command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.ArgumentException">Can't detach a command that has not been attached before;command</exception>
        internal void Detach(ICommand command)
        {
            if (!_attachedCommands.Remove(command))
                return;

            SetCommand(_attachedCommands.FirstOrDefault());
        }

        /// <summary>
        /// Replaces the specified old command with the new command, preserving it's position in the command stack.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        /// <exception cref="System.ArgumentException">Can't replace a command that has not been attached before;oldCommand</exception>
        internal void Replace(ICommand oldCommand, ICommand newCommand)
        {
            var index = _attachedCommands.IndexOf(oldCommand);

            if (index < 0)
                throw new ArgumentException(@"Can't replace a command that has not been attached before", nameof(oldCommand));

            _attachedCommands[index] = newCommand;

            SetCommand(_attachedCommands.FirstOrDefault());
        }

        private void SetCommand(ICommand? command)
        {
            Command = command;

            BindingOperations.ClearBinding(this, IsCheckedProperty);

            if (command is ICheckableCommand)
            {
                BindingOperations.SetBinding(this, IsCheckedProperty, new Binding("IsChecked") { Source = command });
            }

            IsAnyTargetAttached = command != null;

            _attachedCommands.OfType<ICommandChangedNotificationSink>().ForEach(item => item?.ActiveCommandChanged(command));
        }
    }
}
