namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Interactivity;

    /// <summary>
    /// Handles routing of a command from the <see cref="CommandSource"/> to the view model.
    /// Using this behavior ensures that the command is only active if the view is loaded.
    /// </summary>
    public sealed class CommandRoutingBehavior : FrameworkElementBehavior<FrameworkElement>, ICheckableCommand, ICommandChangedNotificationSink
    {
        /// <summary>
        /// Gets or sets the type of the command source factory defining the command.
        /// </summary>
        [CanBeNull]
        public Type CommandSource
        {
            get => (Type)GetValue(CommandSourceProperty);
            set => SetValue(CommandSourceProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CommandSource"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CommandSourceProperty =
            DependencyProperty.Register("CommandSource", typeof(Type), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata((sender, e) => ((CommandRoutingBehavior)sender)?.CommandSource_Changed((Type)e.OldValue, (Type)e.NewValue)));


        /// <summary>
        /// Gets or sets the command target.
        /// </summary>
        /// <remarks>
        /// The command target is usually by a binding to the corresponding command property of the view model.
        /// </remarks>
        [NotNull]
        public ICommand CommandTarget
        {
            get
            {
                return (ICommand)GetValue(CommandTargetProperty) ?? NullCommand.Default;
            }
            set => SetValue(CommandTargetProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(ICommand), typeof(CommandRoutingBehavior));


        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        [CanBeNull]
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandRoutingBehavior));


        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        [CanBeNull]
        public object CompositionContext
        {
            get => GetValue(CompositionContextProperty);
            set => SetValue(CompositionContextProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CompositionContext"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CompositionContextProperty =
            DependencyProperty.Register("CompositionContext", typeof(object), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata((sender, e) => ((CommandRoutingBehavior)sender)?.CompositionContext_Changed(e.OldValue, e.NewValue)));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled, i.e. if the routing will be active. 
        /// This does not affect the enabled state of the visual bound to the command source.
        /// </summary>
        public bool IsEnabled
        {
            get => this.GetValue<bool>(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata(true, (sender, e) => ((CommandRoutingBehavior)sender)?.StateChanged()));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked, e.g. when bound to a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> set to <c>true</c>.
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
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata(true, (sender, e) => ((CommandRoutingBehavior)sender)?.StateChanged()));


        /// <summary>
        /// Gets a value indicating whether this instance is active. 
        /// If multiple command routings exist, the routing of the element that had the most recent keyboard focus is active.
        /// </summary>
        public bool IsActive
        {
            get => this.GetValue<bool>(IsActiveProperty);
            private set => SetValue(_isActivePropertyKey, value);
        }
        [NotNull]
        private static readonly DependencyPropertyKey _isActivePropertyKey =
            DependencyProperty.RegisterReadOnly("IsActive", typeof(bool), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="IsActive"/> read only dependency property
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static readonly DependencyProperty IsActiveProperty = _isActivePropertyKey.DependencyProperty;


        private void CommandSource_Changed([CanBeNull] Type oldValue, [CanBeNull] Type newValue)
        {
            if (!typeof(CommandSourceFactory).IsAssignableFrom(newValue))
            {
                throw new InvalidOperationException(@"Only objects deriving from CommandSourceFactory can be assigned");
            }

            if (AssociatedObject == null)
                return;

            if ((oldValue != null) && (newValue != null))
            {
                throw new InvalidOperationException(@"Cannot change the command source while associated with an object.");
            }

            var compositionContext = CompositionContext;

            if (oldValue != null)
            {
                var oldCommandSource = GetCommandSourceFactory(oldValue);
                oldCommandSource?.Detach(compositionContext, this);
            }

            if (!IsAlive)
                return;

            if (newValue != null)
            {
                var newCommandSource = GetCommandSourceFactory(newValue);
                newCommandSource?.Attach(compositionContext, this);
            }
        }

        private void CompositionContext_Changed([CanBeNull] object oldValue, [CanBeNull] object newValue)
        {
            if (AssociatedObject == null)
                return;

            var factory = GetCommandSourceFactory();

            if (factory == null)
                return;

            factory.Detach(oldValue, this);

            if (!IsAlive)
                return;

            factory.Attach(newValue, this);
        }

        private void StateChanged()
        {
            var compositionContext = CompositionContext;

            CompositionContext_Changed(compositionContext, compositionContext);
        }

        /// <inheritdoc />
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.IsKeyboardFocusWithinChanged += AssociatedObject_IsKeyboardFocusWithinChanged;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.IsKeyboardFocusWithinChanged -= AssociatedObject_IsKeyboardFocusWithinChanged;
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            // Sometimes objects are preloaded, so we get two loaded events! Ignore when object is not yet visible!
            if (!IsAlive)
                return;

            var commandSource = GetCommandSourceFactory();

            commandSource?.Attach(CompositionContext, this);
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            base.OnAssociatedObjectUnloaded();

            var commandSource = GetCommandSourceFactory();

            commandSource?.Detach(CompositionContext, this);
        }

        [CanBeNull]
        private CommandSourceFactory GetCommandSourceFactory()
        {
            return GetCommandSourceFactory(CommandSource);
        }

        [CanBeNull]
        private CommandSourceFactory GetCommandSourceFactory([CanBeNull] Type commandSourceType)
        {
            if (commandSourceType == null)
                return null;

            var element = AssociatedObject;
            if (element == null)
                return null;

            var exportProvider = IsAlive ? element.GetExportProvider() : element.TryGetExportProvider();

            return exportProvider?.GetExports(commandSourceType, typeof(object), string.Empty)
                .Select(export => export?.Value)
                .OfType<CommandSourceFactory>()
                .FirstOrDefault();
        }

        private bool IsAlive
        {
            get
            {

                var associatedObject = AssociatedObject;

                return (associatedObject != null) && IsEnabled && associatedObject.IsLoaded && associatedObject.IsVisible;
            }
        }

        private void AssociatedObject_IsKeyboardFocusWithinChanged([NotNull] object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject?.IsKeyboardFocusWithin == true)
            {
                StateChanged();
            }
        }

        bool ICommand.CanExecute([CanBeNull] object parameter)
        {
            return CommandTarget.CanExecute(CommandParameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add => CommandTarget.CanExecuteChanged += value;
            remove => CommandTarget.CanExecuteChanged -= value;
        }

        void ICommand.Execute([CanBeNull] object parameter)
        {
            CommandTarget.Execute(CommandParameter);
        }

        void ICommandChangedNotificationSink.ActiveCommandChanged(ICommand command)
        {
            var oldValue = IsActive;

            IsActive = ReferenceEquals(command, this);

            var newValue = IsActive;

            if (newValue != oldValue)
            {
                Debug.WriteLine("IsActive, " + CommandSource + ", " + AssociatedObject + ": " + newValue);
            }
        }
    }

    internal interface ICommandChangedNotificationSink
    {
        void ActiveCommandChanged([CanBeNull] ICommand command);
    }
}
