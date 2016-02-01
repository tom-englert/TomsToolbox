namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

    /// <summary>
    /// Interface of the public properties implemented by <see cref="CommandSourceFactory{T}"/>.
    /// </summary>
    public interface ICommandSourceFactory
    {
        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        object Header
        {
            get;
        }

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        object Description
        {
            get;
        }

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        object Icon
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether to show the header text when this command is bound to a button.
        /// If false, only the icon should be displayed.
        /// </summary>
        bool ShowTextOnButtons
        {
            get;
        }

        /// <summary>
        /// Gets the id of the region sub-items can register for.
        /// </summary>
        /// <remarks>
        /// This is used to build up menus with sub menu entries.
        /// </remarks>
        string SubRegionId
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the control associated with this instance should be checkable, 
        /// e.g. a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> or a <see cref="ToggleButton"/> in a tool bar.
        /// </summary>
        bool IsCheckable
        {
            get;
        }

        /// <summary>
        /// Gets the name of the group that this command belongs to. 
        /// If different group names are specified for a target region, the commands can be grouped and the groups separated by a <see cref="Separator"/>.
        /// </summary>
        object GroupName
        {
            get;
        }

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        object Tag
        {
            get;
        }
    }

    /// <summary>
    /// Base class for command declaration to be used with visual composition.
    /// </summary>
    /// <typeparam name="T">The derived type of the hosted <see cref="CommandSource"/>.</typeparam>
    public abstract class CommandSourceFactory<T> : DependencyObject, IComposablePartFactory, ICommandSourceFactory where T : CommandSource
    {
        /// <summary>
        /// The key for the <see cref="TextAttribute"/> that defines the default <see cref="SubRegionId"/>
        /// </summary>
        public const string SubRegionIdKey = "SubRegionId";
        /// <summary>
        /// The key for the <see cref="TextAttribute"/> that defines the default Uri for the <see cref="Icon"/>
        /// </summary>
        public const string IconUriKey = "IconUri";
        /// <summary>
        /// The key for the <see cref="TextAttribute"/> that defines the default value for the <see cref="ShowTextOnButtons"/>
        /// </summary>
        public const string ShowTextOnButtonsKey = "ShowTextOnButtons";
        /// <summary>
        /// The key for the <see cref="TextAttribute"/> that defines the default value for the <see cref="IsCheckable"/>
        /// </summary>
        public const string IsCheckableKey = "IsCheckable";
        /// <summary>
        /// The key for the <see cref="TextAttribute"/> that defines the default value for the <see cref="GroupName"/>
        /// </summary>
        public const string GroupNameKey = "GroupName";


        private readonly Dictionary<object, T> _commandSourcePerContext = new Dictionary<object, T>();

        /// <summary>
        /// Gets the part for the specified context.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <returns>The part to be used in composition.</returns>
        public object GetPart(object compositionContext)
        {
            return GetCommandSource(compositionContext);
        }

        /// <summary>
        /// Gets the command source for the specified composition context.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <returns>The command source.</returns>
        private T GetCommandSource(object compositionContext)
        {
            Contract.Ensures(Contract.Result<T>() != null);

            var commandSource = _commandSourcePerContext.ForceValue(compositionContext ?? typeof(NullKey), context => CreateCommandSource());
            Contract.Assume(commandSource != null);
            return commandSource;
        }

        /// <summary>
        /// Creates a new <see cref="CommandSource"/> or derived object.
        /// </summary>
        /// <returns>The command source.</returns>
        protected abstract T CreateCommandSource();

        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        public virtual object Header
        {
            get
            {
                return GetType().TryGetDisplayName();
            }
        }

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        public virtual object Description
        {
            get
            {
                return GetType().TryGetDescription();
            }
        }

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        public virtual object Icon
        {
            get
            {
                var iconUri = GetType().TryGetText(IconUriKey);

                return iconUri == null ? null : new BitmapImage(new Uri(iconUri, UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// Gets a value indicating whether to show the header text when this command is bound to a button.
        /// If false, only the icon should be displayed.
        /// </summary>
        public virtual bool ShowTextOnButtons
        {
            get
            {
                return bool.TrueString.Equals(GetType().TryGetText(ShowTextOnButtonsKey), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the id of the region sub-items can register for.
        /// </summary>
        /// <remarks>
        /// This is used to build up menus with sub menu entries.
        /// </remarks>
        public virtual string SubRegionId
        {
            get
            {
                return GetType().TryGetText(SubRegionIdKey);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control associated with this instance should be checkable, 
        /// e.g. a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> or a <see cref="ToggleButton"/> in a tool bar.
        /// </summary>
        public virtual bool IsCheckable
        {
            get
            {
                return bool.TrueString.Equals(GetType().TryGetText(IsCheckableKey), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the name of the group that this command belongs to.
        /// If different group names are specified for a target region, the commands can be grouped and the groups separated by a <see cref="Separator" />.
        /// </summary>
        public virtual object GroupName
        {
            get
            {
                return GetType().TryGetText(GroupNameKey);
            }
        }

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        public virtual object Tag
        {
            get
            {
                Contract.Ensures(Contract.Result<object>() == null);
                return null;
            }
        }

        /// <summary>
        /// Attaches the specified command. The last command attached will become the active command, while the previous command will be pushed on a stack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="command">The command.</param>
        /// <returns>The <see cref="CommandSource"/> associated with the <paramref name="context"/></returns>
        public T Attach(object context, ICommand command)
        {
            Contract.Requires(command != null);
            Contract.Ensures(Contract.Result<T>() != null);

            var commandSource = GetCommandSource(context);

            commandSource.Attach(command);

            return commandSource;
        }

        /// <summary>
        /// Detaches the specified command. If the detached command was the active command, the previous command from the stack will become the active command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="command">The command.</param>
        /// <returns>The <see cref="CommandSource"/> associated with the <paramref name="context"/></returns>
        /// <exception cref="System.ArgumentException">Can't detach a command that has not been attached before;command</exception>
        public T Detach(object context, ICommand command)
        {
            Contract.Requires(command != null);
            Contract.Ensures(Contract.Result<T>() != null);

            var commandSource = GetCommandSource(context);

            commandSource.Detach(command);

            return commandSource;
        }

        /// <summary>
        /// Replaces the specified old command with the new command, preserving it's position in the command stack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        /// <returns>The <see cref="CommandSource"/> associated with the <paramref name="context"/></returns>
        /// <exception cref="System.ArgumentException">Can't replace a command that has not been attached before;oldCommand</exception>
        public T Replace(object context, ICommand oldCommand, ICommand newCommand)
        {
            Contract.Requires(oldCommand != null);
            Contract.Requires(newCommand != null);
            Contract.Ensures(Contract.Result<T>() != null);

            var commandSource = GetCommandSource(context);

            commandSource.Replace(oldCommand, newCommand);

            return commandSource;
        }

        private class NullKey
        {
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_commandSourcePerContext != null);
        }
    }

    /// <summary>
    /// Base class for command declaration to be used with visual composition.
    /// </summary>
    public abstract class CommandSourceFactory : CommandSourceFactory<CommandSource>
    {
        /// <summary>
        /// Creates a new <see cref="CommandSource" /> or derived object.
        /// </summary>
        /// <returns>
        /// The command source.
        /// </returns>
        protected override CommandSource CreateCommandSource()
        {
            return new CommandSource(this);
        }
    }
}
