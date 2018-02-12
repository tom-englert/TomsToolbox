namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

    /// <summary>
    /// Command declaration to be used with visual composition.
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    /// <inheritdoc />
    public class CommandSource : DependencyObject
    {
        [NotNull]
        private readonly ICommandSourceFactory _owner;
        [NotNull, ItemNotNull]
        private readonly List<ICommand> _attachedCommands = new List<ICommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Composition.CommandSource" /> class.
        /// </summary>
        /// <param name="owner">The command source factory.</param>
        /// <inheritdoc />
        public CommandSource([NotNull] ICommandSourceFactory owner)
        {
            Contract.Requires(owner != null);

            _owner = owner;
        }

        /// <summary>
        /// Gets the command represented by this <see cref="CommandSourceFactory" />. This can be bound to a menu's or button's Command property.
        /// </summary>
        [CanBeNull]
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            private set => SetValue(_commandPropertyKey, value);
        }
        [NotNull]
        private static readonly DependencyPropertyKey _commandPropertyKey =
            DependencyProperty.RegisterReadOnly("Command", typeof(ICommand), typeof(CommandSource), new FrameworkPropertyMetadata(NullCommand.Default, null, Command_CoerceValue));
        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property
        /// </summary>
        // ReSharper disable once AssignNullToNotNullAttribute
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
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
        [NotNull]
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CommandSource));


        [CanBeNull]
        private static object Command_CoerceValue([CanBeNull] DependencyObject d, [CanBeNull] object basevalue)
        {
            return basevalue ?? NullCommand.Default;
        }

        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        [CanBeNull]
        public object Header => _owner.Header;

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        [CanBeNull]
        public object Description => _owner.Description;

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        [CanBeNull]
        public object Icon => _owner.Icon;

        /// <summary>
        /// Gets a value indicating whether to show the header text when this command is bound to a button. 
        /// If false, only the icon should be displayed.
        /// </summary>
        public bool ShowTextOnButtons => _owner.ShowTextOnButtons;

        /// <summary>
        /// Gets the id of the region sub-items can register for.
        /// </summary>
        [CanBeNull]
        public string SubRegionId => _owner.SubRegionId;

        /// <summary>
        /// Gets a value indicating whether the control associated with this instance should be checkable, 
        /// e.g. a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> or a <see cref="ToggleButton"/> in a tool bar.
        /// </summary>
        public bool IsCheckable => _owner.IsCheckable;

        /// <summary>
        /// Gets the name of the group that this command belongs to.
        /// If different group names are specified for a target region, the commands can be grouped and the groups separated by a <see cref="Separator" />.
        /// </summary>
        [CanBeNull]
        public virtual object GroupName => _owner.GroupName;

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        [CanBeNull]
        public object Tag => _owner.Tag;

        /// <summary>
        /// Gets a value indicating whether any target is attached to this source.
        /// Some controls like tool bars may want to hide commands that have no target attached rather than only disabling them.
        /// </summary>
        public bool IsAnyTargetAttached
        {
            get => this.GetValue<bool>(IsAnyTargetAttachedProperty);
            private set => SetValue(_isAnyTargetAttachedPropertyKey, value);
        }
        [NotNull]
        private static readonly DependencyPropertyKey _isAnyTargetAttachedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsAnyTargetAttached", typeof(bool), typeof(CommandSource), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="IsAnyTargetAttached"/> dependency property
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static readonly DependencyProperty IsAnyTargetAttachedProperty = _isAnyTargetAttachedPropertyKey.DependencyProperty;

        /// <summary>
        /// Attaches the specified command. The last command attached will become the active command, while the previous command will be pushed on a stack.
        /// </summary>
        /// <param name="command">The command.</param>
        internal void Attach([NotNull] ICommand command)
        {
            Contract.Requires(command != null);

            _attachedCommands.Insert(0, command);

            SetCommand(_attachedCommands.FirstOrDefault());
        }

        /// <summary>
        /// Detaches the specified command. If the detached command was the active command, the previous command from the stack will become the active command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.ArgumentException">Can't detach a command that has not been attached before;command</exception>
        internal void Detach([NotNull] ICommand command)
        {
            Contract.Requires(command != null);

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
        internal void Replace([NotNull] ICommand oldCommand, [NotNull] ICommand newCommand)
        {
            Contract.Requires(oldCommand != null);
            Contract.Requires(newCommand != null);

            var index = _attachedCommands.IndexOf(oldCommand);

            if (index < 0)
                throw new ArgumentException(@"Can't replace a command that has not been attached before", nameof(oldCommand));

            _attachedCommands[index] = newCommand;

            SetCommand(_attachedCommands.FirstOrDefault());
        }

        private void SetCommand([CanBeNull] ICommand command)
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

        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_attachedCommands != null);
            Contract.Invariant(_owner != null);
        }
    }
}
