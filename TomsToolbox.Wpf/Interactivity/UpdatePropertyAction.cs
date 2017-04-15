namespace TomsToolbox.Wpf.Interactivity
{
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Updates the associated objects binding of the specified property; e.g. display computed properties like current time without the need to
    /// write a special services that provide individual property change events.
    /// </summary>
    /// <example><code language="XAML"><![CDATA[
    /// <TextBlock x:Name="Text" Text="{Binding SomeComputedPropertyWithoutChangeNotification}" />
    /// <i:Interaction.Triggers>
    ///   <ei:TimerTrigger MillisecondsPerTick="1000">
    ///     <toms:UpdatePropertyAction TargetName="Text" Property="{x:Static TextBlock.TextProperty}"/>
    ///   </ei:TimerTrigger>
    /// </i:Interaction.Triggers>
    /// ]]>
    /// </code></example>
    public class UpdatePropertyAction : TargetedTriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Gets the property that should be refreshed.
        /// </summary>
        [CanBeNull]
        public DependencyProperty Property
        {
            get { return (DependencyProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="Property"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PropertyProperty =
            DependencyProperty.Register("Property", typeof (DependencyProperty), typeof (UpdatePropertyAction));

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to the action. If the action does not require a parameter, the parameter may be
        /// set to a null reference.
        /// </param>
        protected override void Invoke([CanBeNull] object parameter)
        {
            var target = Target;
            if ((target == null) || !target.IsLoaded)
                return;

            var property = Property;
            if (property == null)
                return;

            var bindingExpression = BindingOperations.GetBindingExpression(target, property);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
                return;
            }

            var multiBindingExpression = BindingOperations.GetMultiBindingExpression(target, property);

            multiBindingExpression?.BindingExpressions.ForEach(expr => expr?.UpdateTarget());
        }
    }
}