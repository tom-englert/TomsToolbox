namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Interface of the public properties implemented by <see cref="CommandSourceFactory{T}"/>.
    /// </summary>
    public interface ICommandSourceFactory
    {
        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        [CanBeNull]
        object? Header
        {
            get;
        }

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        [CanBeNull]
        object? Description
        {
            get;
        }

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        [CanBeNull]
        object? Icon
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
        [CanBeNull]
        string? SubRegionId
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
        [CanBeNull]
        object? GroupName
        {
            get;
        }

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        [CanBeNull]
        object? Tag
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


        [NotNull]
        private readonly Dictionary<object, T> _commandSourcePerContext = new Dictionary<object, T>();

        /// <summary>
        /// Gets the part for the specified context.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <returns>The part to be used in composition.</returns>
        [NotNull]
        public object GetPart([CanBeNull] object? compositionContext)
        {
            return GetCommandSource(compositionContext);
        }

        /// <summary>
        /// Gets the command source for the specified composition context.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <returns>The command source.</returns>
        [NotNull]
        private T GetCommandSource([CanBeNull] object? compositionContext)
        {
            return _commandSourcePerContext.ForceValue(compositionContext ?? typeof(NullKey), context => CreateCommandSource())!;
        }

        /// <summary>
        /// Creates a new <see cref="CommandSource"/> or derived object.
        /// </summary>
        /// <returns>The command source.</returns>
        [NotNull]
        protected abstract T CreateCommandSource();

        /// <summary>
        /// Gets the header to be shown in the UI. Usually this is a localized text naming the command.
        /// </summary>
        [CanBeNull]
        public virtual object? Header => GetType().TryGetDisplayName();

        /// <summary>
        /// Gets the tool tip to be shown in the UI. Usually this is a localized text describing the command.
        /// </summary>
        [CanBeNull]
        public virtual object? Description => GetType().TryGetDescription();

        /// <summary>
        /// Gets the icon to be shown in the UI, or null to show no icon.
        /// </summary>
        [CanBeNull]
        public virtual object? Icon
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
        public virtual bool ShowTextOnButtons => string.Equals(bool.TrueString, GetType().TryGetText(ShowTextOnButtonsKey), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the id of the region sub-items can register for.
        /// </summary>
        /// <remarks>
        /// This is used to build up menus with sub menu entries.
        /// </remarks>
        [CanBeNull]
        public virtual string? SubRegionId => GetType().TryGetText(SubRegionIdKey);

        /// <summary>
        /// Gets a value indicating whether the control associated with this instance should be checkable, 
        /// e.g. a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> or a <see cref="ToggleButton"/> in a tool bar.
        /// </summary>
        public virtual bool IsCheckable => string.Equals(bool.TrueString, GetType().TryGetText(IsCheckableKey), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the name of the group that this command belongs to.
        /// If different group names are specified for a target region, the commands can be grouped and the groups separated by a <see cref="Separator" />.
        /// </summary>
        [CanBeNull]
        public virtual object? GroupName => GetType().TryGetText(GroupNameKey);

        /// <summary>
        /// Gets a tag that can be bound to the target objects tag.
        /// </summary>
        [CanBeNull]
        public virtual object? Tag => null;

        /// <summary>
        /// Attaches the specified command. The last command attached will become the active command, while the previous command will be pushed on a stack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="command">The command.</param>
        /// <returns>The <see cref="CommandSource"/> associated with the <paramref name="context"/></returns>
        [NotNull]
        public T Attach([CanBeNull] object? context, [NotNull] ICommand command)
        {
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
        [NotNull]
        public T Detach([CanBeNull] object? context, [NotNull] ICommand command)
        {
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
        [NotNull]
        public T Replace([CanBeNull] object? context, [NotNull] ICommand oldCommand, [NotNull] ICommand newCommand)
        {
            var commandSource = GetCommandSource(context);

            commandSource.Replace(oldCommand, newCommand);

            return commandSource;
        }

        private class NullKey
        {
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
