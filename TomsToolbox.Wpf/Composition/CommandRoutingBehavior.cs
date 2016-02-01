namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Interactivity;

    /// <summary>
    /// Handles routing of a command from the <see cref="CommandSource"/> to the view model.
    /// Using this behavior ensures that the command is only active if the view is loaded.
    /// </summary>
    public sealed class CommandRoutingBehavior : FrameworkElementBehavior<FrameworkElement>, ICheckableCommand
    {
        /// <summary>
        /// Gets or sets the type of the command source factory defining the command.
        /// </summary>
        public Type CommandSource
        {
            get { return (Type)GetValue(CommandSourceProperty); }
            set { SetValue(CommandSourceProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CommandSource"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandSourceProperty =
            DependencyProperty.Register("CommandSource", typeof(Type), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata((sender, e) => ((CommandRoutingBehavior)sender).CommandSource_Changed((Type)e.OldValue, (Type)e.NewValue)));


        /// <summary>
        /// Gets or sets the command target.
        /// </summary>
        /// <remarks>
        /// The command target is usually by a binding to the corresponding command property of the view model.
        /// </remarks>
        public ICommand CommandTarget
        {
            get
            {
                Contract.Ensures(Contract.Result<ICommand>() != null);
                return (ICommand)GetValue(CommandTargetProperty) ?? NullCommand.Default;
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(ICommand), typeof(CommandRoutingBehavior));


        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandRoutingBehavior));
        

        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        public object CompositionContext
        {
            get { return GetValue(CompositionContextProperty); }
            set { SetValue(CompositionContextProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CompositionContext"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CompositionContextProperty =
            DependencyProperty.Register("CompositionContext", typeof(object), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata((sender, e) => ((CommandRoutingBehavior)sender).CompositionContext_Changed(e.OldValue, e.NewValue)));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled, i.e. if the routing will be active. 
        /// This does not affect the enabled state of the visual bound to the command source.
        /// </summary>
        public bool IsEnabled
        {
            get { return this.GetValue<bool>(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata(true, (sender, e) => ((CommandRoutingBehavior)sender).State_Changed()));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked, e.g. when bound to a <see cref="MenuItem"/> with <see cref="MenuItem.IsCheckable"/> set to <c>true</c>.
        /// </summary>
        public bool IsChecked
        {
            get { return this.GetValue<bool>(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsChecked"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CommandRoutingBehavior), new FrameworkPropertyMetadata(true, (sender, e) => ((CommandRoutingBehavior)sender).State_Changed()));


        private void CommandSource_Changed(Type oldValue, Type newValue)
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
                if (oldCommandSource != null)
                {
                    oldCommandSource.Detach(compositionContext, this);
                }
            }

            if (!IsActive)
                return;

            if (newValue != null)
            {
                var newCommandSource = GetCommandSourceFactory(newValue);
                if (newCommandSource != null)
                {
                    newCommandSource.Attach(compositionContext, this);
                }
            }
        }

        private void CompositionContext_Changed(object oldValue, object newValue)
        {
            if (AssociatedObject == null)
                return;

            var factory = GetCommandSourceFactory();

            if (factory == null)
                return;

            factory.Detach(oldValue, this);

            if (!IsActive)
                return;

            factory.Attach(newValue, this);
        }

        private void State_Changed()
        {
            var compositionContext = CompositionContext;

            CompositionContext_Changed(compositionContext, compositionContext);
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            // Sometimes objects are preloaded, so we get two loaded events! Ignore when object is not yet visible!
            if (!IsActive)
                return;

            var commandSource = GetCommandSourceFactory();

            if (commandSource == null)
                return;

            commandSource.Attach(CompositionContext, this);
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            base.OnAssociatedObjectUnloaded();

            var commandSource = GetCommandSourceFactory();

            if (commandSource == null)
                return;

            commandSource.Detach(CompositionContext, this);
        }

        private CommandSourceFactory GetCommandSourceFactory()
        {
            return GetCommandSourceFactory(CommandSource);
        }

        private CommandSourceFactory GetCommandSourceFactory(Type commandSourceType)
        {
            if ((commandSourceType == null) || !IsActive)
                return null;

            var exportProvider = AssociatedObject.GetExportProvider();

            return exportProvider.GetExports(commandSourceType, typeof(object), null)
                .Select(export => export.Value)
                .OfType<CommandSourceFactory>()
                .FirstOrDefault();
        }

        private bool IsActive
        {
            get
            {
                Contract.Ensures((Contract.Result<bool>() == false) || (AssociatedObject != null));

                var associatedObject = AssociatedObject;

                return (associatedObject != null) && IsEnabled && associatedObject.IsLoaded && associatedObject.IsVisible;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CommandTarget.CanExecute(CommandParameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandTarget.CanExecuteChanged += value;
            }
            remove
            {
                CommandTarget.CanExecuteChanged -= value;
            }
        }

        void ICommand.Execute(object parameter)
        {
            CommandTarget.Execute(CommandParameter);
        }
    }
}
