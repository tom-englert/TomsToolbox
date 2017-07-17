namespace TomsToolbox.Wpf
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    using TriggerBase = System.Windows.Interactivity.TriggerBase;

    /// <summary>
    /// A container to host the source <see cref="InputBindingCollection"/>. Must be a <see cref="FrameworkElement"/> to minimize binding errors.
    /// </summary>
    [ContentProperty("InputBindings")]
    public class InputBindingTemplate : FrameworkElement
    {
    }

    /// <summary>
    /// A collection of <see cref="System.Windows.Interactivity.Behavior"/> objects.
    /// </summary>
    public class BehaviorCollection : Collection<Behavior>
    {
    }

    /// <summary>
    /// A collection of Trigger (<see cref="System.Windows.Interactivity.TriggerBase"/>) objects.
    /// </summary>
    public class TriggerCollection : Collection<TriggerBase>
    {
    }

    /// <summary>
    /// A collection of <see cref="GroupDescription"/> objects.
    /// </summary>
    public class GroupDescriptionCollection : Collection<GroupDescription>
    {
    }

    /// <summary>
    /// A collection of <see cref="GroupStyle"/> objects.
    /// </summary>
    public class GroupStyleCollection : Collection<GroupStyle>
    {
    }

    /// <summary>
    /// Extensions to support style binding of some read only collection properties.
    /// </summary>
    public static class StyleBindings
    {
        /// <summary>
        /// Gets the inputBindings attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.InputBindings"/> attached property.
        /// </summary>
        /// <param name="obj">The object the inputBindings are attached to.</param>
        /// <returns>The inputBindings.</returns>
        [CanBeNull]
        public static InputBindingTemplate GetInputBindings([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);
            return (InputBindingTemplate)obj.GetValue(InputBindingsProperty);
        }
        /// <summary>
        /// Sets the inputBindings attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.InputBindings"/> attached property.
        /// </summary>
        /// <param name="obj">The object the inputBindings are attached to.</param>
        /// <param name="value">The inputBindings to attach.</param>
        public static void SetInputBindings([NotNull] DependencyObject obj, [CanBeNull] InputBindingTemplate value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(InputBindingsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.InputBindings"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set <see cref="UIElement.InputBindings"/> via a <see cref="Style"/>
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="ListBoxItem">
        ///   <Setter Property="wpf:StyleBindings.InputBindings">
        ///     <Setter.Value>
        ///       <wpf:InputBindingTemplate>
        ///         <KeyBinding .... />
        ///         <MouseBinding .... />
        ///       </core:InputBindingTemplate>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingTemplate), typeof(StyleBindings), new FrameworkPropertyMetadata((d, e) => InputBindings_Changed(d, (InputBindingTemplate)e.NewValue)));

        private static void InputBindings_Changed([CanBeNull] DependencyObject d, [CanBeNull] UIElement newValue)
        {
            if (newValue == null)
                return;

            var target = d as UIElement;
            if (target == null)
                return;

            var existingInputBindings = target.InputBindings;
            var newInputBindings = newValue.InputBindings;

            Contract.Assume(existingInputBindings.Count == 0);

            // ReSharper disable once PossibleNullReferenceException
            existingInputBindings.AddRange(newInputBindings.OfType<InputBinding>().Select(item => item.Clone()).ToArray());
        }


        /// <summary>
        /// Gets the group styles attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyles"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <returns>The group styles.</returns>
        [CanBeNull, ItemNotNull]
        public static GroupStyleCollection GetGroupStyles([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);

            return (GroupStyleCollection)obj.GetValue(GroupStylesProperty);
        }
        /// <summary>
        /// Sets the group style attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyles"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <param name="value">The group styles.</param>
        public static void SetGroupStyles([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] GroupStyleCollection value)
        {
            Contract.Requires(obj != null);

            obj.SetValue(GroupStylesProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyles"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set <see cref="ItemsControl.GroupStyle"/> via a <see cref="Style"/>
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="ListBox">
        ///   <Setter Property="wpf:StyleBindings.GroupStyles">
        ///     <Setter.Value>
        ///       <GroupStyleCollection>
        ///         <GroupStyle>
        ///           < .... />
        ///         <GroupStyle>
        ///       </GroupStyleCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty GroupStylesProperty =
            DependencyProperty.RegisterAttached("GroupStyles", typeof(GroupStyleCollection), typeof(StyleBindings), new FrameworkPropertyMetadata(GroupStyles_Changed));

        private static void GroupStyles_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupStyle = (d as ItemsControl)?.GroupStyle;

            if (groupStyle == null)
                return;

            groupStyle.Clear();

            var newGroupStyles = e.NewValue as GroupStyleCollection;
            if (newGroupStyles != null)
            {
                groupStyle.AddRange(newGroupStyles);
                return;
            }
        }


        /// <summary>
        /// Gets the group style attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyle"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <returns>The group style.</returns>
        [CanBeNull]
        public static GroupStyle GetGroupStyle([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);

            return (GroupStyle)obj.GetValue(GroupStyleProperty);
        }
        /// <summary>
        /// Sets the group style attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyle"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <param name="value">The group style.</param>
        public static void SetGroupStyle([NotNull] DependencyObject obj, [CanBeNull] GroupStyle value)
        {
            Contract.Requires(obj != null);

            obj.SetValue(GroupStyleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyle"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set a single <see cref="ItemsControl.GroupStyle"/> via a <see cref="Style"/>.
        /// This a shortcut to <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyles"/> to simplify usage when only one group style is needed.
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="ListBox">
        ///   <Setter Property="wpf:StyleBindings.GroupStyle">
        ///     <Setter.Value>
        ///       <GroupStyle>
        ///         < .... />
        ///       <GroupStyle>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty GroupStyleProperty =
            DependencyProperty.RegisterAttached("GroupStyle", typeof(GroupStyle), typeof(StyleBindings), new FrameworkPropertyMetadata(GroupStyle_Changed));

        private static void GroupStyle_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groupStyle = (d as ItemsControl)?.GroupStyle;

            if (groupStyle == null)
                return;

            groupStyle.Clear();

            var newGroupStyle = e.NewValue as GroupStyle;
            if (newGroupStyle != null)
            {
                groupStyle.Add(newGroupStyle);
            }
        }

        /// <summary>
        /// Gets the group descriptions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupDescriptions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group descriptions are attached to.</param>
        /// <returns>The group descriptions.</returns>
        [CanBeNull]
        public static GroupDescriptionCollection GetGroupDescriptions([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);

            return (GroupDescriptionCollection)obj.GetValue(GroupDescriptionsProperty);
        }
        /// <summary>
        /// Sets the group descriptions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupDescriptions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group descriptions are attached to.</param>
        /// <param name="value">The group descriptions.</param>
        public static void SetGroupDescriptions([NotNull] DependencyObject obj, [CanBeNull] ICollection<GroupDescription> value)
        {
            Contract.Requires(obj != null);

            obj.SetValue(GroupDescriptionsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupDescriptions"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set <see cref="ItemCollection.GroupDescriptions"/> for an <see cref="ItemsControl"/> via a <see cref="Style"/>
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="ListBox">
        ///   <Setter Property="wpf:StyleBindings.GroupDescriptions">
        ///     <Setter.Value>
        ///       <GroupDescriptionCollection>
        ///         < .... />
        ///       <GroupDescriptionCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty GroupDescriptionsProperty =
            DependencyProperty.RegisterAttached("GroupDescriptions", typeof(GroupDescriptionCollection), typeof(StyleBindings), new FrameworkPropertyMetadata(GroupDescriptions_Changed));

        private static void GroupDescriptions_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            var groupDescriptions = (d as ItemsControl)?.Items.GroupDescriptions;

            if (groupDescriptions == null)
                return;

            groupDescriptions.Clear();

            var newGroupDescriptions = e.NewValue as GroupDescriptionCollection;
            if (newGroupDescriptions == null)
                return;

            groupDescriptions.AddRange(newGroupDescriptions);
        }


        /// <summary>
        /// Gets the behaviors attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Behaviors"/> attached property.
        /// </summary>
        /// <param name="obj">The object the behaviors are attached to.</param>
        /// <returns>The behaviors.</returns>
        [CanBeNull]
        public static BehaviorCollection GetBehaviors([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);
            return (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        }
        /// <summary>
        /// Sets the behaviors attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Behaviors"/> attached property.
        /// </summary>
        /// <param name="obj">The object the behaviors are attached to.</param>
        /// <param name="value">The behaviors to attach.</param>
        public static void SetBehaviors([NotNull] DependencyObject obj, [CanBeNull] BehaviorCollection value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(BehaviorsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.Behaviors"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set behaviors via a <see cref="Style"/>
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="MenuItem">
        ///   <Setter Property="core:StyleBindings.Behaviors">
        ///     <Setter.Value>
        ///       <core:BehaviorCollection>
        ///         <core:ItemsControlCompositionBehavior RegionId="{Binding SubRegionId}"/>
        ///       </core:BehaviorCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(StyleBindings), new UIPropertyMetadata((d, e) => Behaviors_Changed(d, (BehaviorCollection)e.NewValue)));

        private static void Behaviors_Changed([CanBeNull] DependencyObject d, [CanBeNull] IEnumerable<Behavior> newValue)
        {
            if (newValue != null)
            {
                var existingBehaviors = Interaction.GetBehaviors(d);
                Contract.Assume(existingBehaviors != null);
                Contract.Assume(existingBehaviors.Count == 0);
                existingBehaviors.AddRange(newValue.Select(item => (Behavior)item?.Clone()).Where(item => item != null));
            }
        }


        /// <summary>
        /// Gets the triggers attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Triggers"/> attached property.
        /// </summary>
        /// <param name="obj">The object the triggers are attached to.</param>
        /// <returns>The triggers.</returns>
        [CanBeNull]
        public static TriggerCollection GetTriggers([NotNull] DependencyObject obj)
        {
            Contract.Requires(obj != null);
            return (TriggerCollection)obj.GetValue(TriggersProperty);
        }
        /// <summary>
        /// Sets the triggers attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Triggers"/> attached property.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetTriggers([NotNull] DependencyObject obj, [CanBeNull] TriggerCollection value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(TriggersProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.Triggers"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set triggers via a <see cref="Style"/>
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="MenuItem">
        ///   <Setter Property="core:StyleBindings.Triggers">
        ///     <Setter.Value>
        ///       <core:TriggerCollection>
        ///         <interactivity:EventTrigger EventName="Loaded">
        ///           <interactivity:EventTrigger.Actions>
        ///             ....
        ///           </interactivity:EventTrigger.Actions>
        ///         </interactivity:EventTrigger>
        ///       </core:TriggerCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(TriggerCollection), typeof(StyleBindings), new UIPropertyMetadata((d, e) => Triggers_Changed(d, (TriggerCollection)e.NewValue)));

        private static void Triggers_Changed([CanBeNull] DependencyObject d, [CanBeNull] IEnumerable<TriggerBase> newValue)
        {
            if (newValue != null)
            {
                var existingTriggers = Interaction.GetTriggers(d);
                Contract.Assume(existingTriggers != null);
                Contract.Assume(existingTriggers.Count == 0);

                existingTriggers.AddRange(newValue.Select(item => (TriggerBase)item?.Clone()).Where(item => item != null));
            }
        }
    }
}
