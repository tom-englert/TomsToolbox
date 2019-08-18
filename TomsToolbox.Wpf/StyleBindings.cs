namespace TomsToolbox.Wpf
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using Microsoft.Xaml.Behaviors;

    using TomsToolbox.Essentials;

    using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;

    /// <summary>
    /// A container to host the source <see cref="InputBindingCollection"/>. Must be a <see cref="FrameworkElement"/> to minimize binding errors.
    /// </summary>
    [ContentProperty("InputBindings")]
    public class InputBindingTemplate : FrameworkElement
    {
    }

    /// <summary>
    /// A collection of <see cref="Microsoft.Xaml.Behaviors.Behavior"/> objects.
    /// </summary>
    public class BehaviorCollection : Collection<Behavior>
    {
    }

    /// <summary>
    /// A collection of Trigger (<see cref="Microsoft.Xaml.Behaviors.TriggerBase"/>) objects.
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
    /// A collection of <see cref="ColumnDefinition"/> objects.
    /// </summary>
    public class ColumnDefinitionCollection : Collection<ColumnDefinition>
    {
    }

    /// <summary>
    /// A collection of <see cref="RowDefinition"/> objects.
    /// </summary>
    public class RowDefinitionCollection : Collection<RowDefinition>
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
            return (InputBindingTemplate)obj.GetValue(InputBindingsProperty);
        }
        /// <summary>
        /// Sets the inputBindings attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.InputBindings"/> attached property.
        /// </summary>
        /// <param name="obj">The object the inputBindings are attached to.</param>
        /// <param name="value">The inputBindings to attach.</param>
        public static void SetInputBindings([NotNull] DependencyObject obj, [CanBeNull] InputBindingTemplate value)
        {
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

            if (!(d is UIElement target))
                return;

            var existingInputBindings = target.InputBindings;
            var newInputBindings = newValue.InputBindings;

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
            return (GroupStyleCollection)obj.GetValue(GroupStylesProperty);
        }
        /// <summary>
        /// Sets the group style attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyles"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <param name="value">The group styles.</param>
        public static void SetGroupStyles([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] GroupStyleCollection value)
        {
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

            if (e.NewValue is GroupStyleCollection newGroupStyles)
            {
                groupStyle.AddRange(newGroupStyles);
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
            return (GroupStyle)obj.GetValue(GroupStyleProperty);
        }
        /// <summary>
        /// Sets the group style attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupStyle"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group style is attached to.</param>
        /// <param name="value">The group style.</param>
        public static void SetGroupStyle([NotNull] DependencyObject obj, [CanBeNull] GroupStyle value)
        {
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

            if (e.NewValue is GroupStyle newGroupStyle)
            {
                groupStyle.Add(newGroupStyle);
            }
        }

        /// <summary>
        /// Gets the group descriptions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupDescriptions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group descriptions are attached to.</param>
        /// <returns>The group descriptions.</returns>
        [CanBeNull, ItemNotNull]
        public static GroupDescriptionCollection GetGroupDescriptions([NotNull] DependencyObject obj)
        {
            return (GroupDescriptionCollection)obj.GetValue(GroupDescriptionsProperty);
        }
        /// <summary>
        /// Sets the group descriptions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.GroupDescriptions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the group descriptions are attached to.</param>
        /// <param name="value">The group descriptions.</param>
        public static void SetGroupDescriptions([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] ICollection<GroupDescription> value)
        {
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
            var groupDescriptions = (d as ItemsControl)?.Items.GroupDescriptions;

            if (groupDescriptions == null)
                return;

            groupDescriptions.Clear();

            if (!(e.NewValue is GroupDescriptionCollection newGroupDescriptions))
                return;

            groupDescriptions.AddRange(newGroupDescriptions);
        }


        /// <summary>
        /// Gets the behaviors attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Behaviors"/> attached property.
        /// </summary>
        /// <param name="obj">The object the behaviors are attached to.</param>
        /// <returns>The behaviors.</returns>
        [CanBeNull, ItemNotNull]
        public static BehaviorCollection GetBehaviors([NotNull] DependencyObject obj)
        {
            return (BehaviorCollection)obj.GetValue(BehaviorsProperty);
        }
        /// <summary>
        /// Sets the behaviors attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Behaviors"/> attached property.
        /// </summary>
        /// <param name="obj">The object the behaviors are attached to.</param>
        /// <param name="value">The behaviors to attach.</param>
        public static void SetBehaviors([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] BehaviorCollection value)
        {
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

        private static void Behaviors_Changed([CanBeNull] DependencyObject d, [CanBeNull, ItemNotNull] IEnumerable<Behavior> newValue)
        {
            if (newValue != null)
            {
                var existingBehaviors = Interaction.GetBehaviors(d);
                existingBehaviors.AddRange(newValue.Select(item => (Behavior)item.Clone()));
            }
        }


        /// <summary>
        /// Gets the triggers attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Triggers"/> attached property.
        /// </summary>
        /// <param name="obj">The object the triggers are attached to.</param>
        /// <returns>The triggers.</returns>
        [CanBeNull, ItemNotNull]
        public static TriggerCollection GetTriggers([NotNull] DependencyObject obj)
        {
            return (TriggerCollection)obj.GetValue(TriggersProperty);
        }
        /// <summary>
        /// Sets the triggers attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.Triggers"/> attached property.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetTriggers([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] TriggerCollection value)
        {
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

        private static void Triggers_Changed([CanBeNull] DependencyObject d, [CanBeNull, ItemNotNull] IEnumerable<TriggerBase> newValue)
        {
            if (newValue != null)
            {
                var existingTriggers = Interaction.GetTriggers(d);

                existingTriggers.AddRange(newValue.Select(item => (TriggerBase)item.Clone()));
            }
        }


        /// <summary>
        /// Gets the column definitions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.ColumnDefinitions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the column definitions are attached to.</param>
        /// <returns>The column definitions.</returns>
        [CanBeNull, ItemNotNull]
        public static ColumnDefinitionCollection GetColumnDefinitions([NotNull] DependencyObject obj)
        {
            return (ColumnDefinitionCollection)obj.GetValue(ColumnDefinitionsProperty);
        }
        /// <summary>
        /// Sets the columnDefinitions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.ColumnDefinitions"/> attached property.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetColumnDefinitions([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] ColumnDefinitionCollection value)
        {
            obj.SetValue(ColumnDefinitionsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.ColumnDefinitions"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set column definitions via a <see cref="Style"/> or from a resource.
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="Grid">
        ///   <Setter Property="core:StyleBindings.ColumnDefinitions">
        ///     <Setter.Value>
        ///       <core:ColumnDefinitionCollection>
        ///         <ColumnDefinition Width="Auto" SharedSizeGroup="Col1" />
        ///         <ColumnDefinition Width="20" />
        ///         <ColumnDefinition Width="*" />
        ///       </core:ColumnDefinitionCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitionCollection", typeof(ColumnDefinitionCollection), typeof(StyleBindings), new FrameworkPropertyMetadata(ColumnDefinitions_Changed));

        private static void ColumnDefinitions_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var columnDefinitions = (d as Grid)?.ColumnDefinitions;

            if (columnDefinitions == null)
                return;

            columnDefinitions.Clear();

            if (e.NewValue is ColumnDefinitionCollection newColumnDefinitions)
            {
                foreach (var columnDefinition in newColumnDefinitions)
                {
                    columnDefinitions.Add(new ColumnDefinition
                    {
                        MinWidth = columnDefinition.MinWidth,
                        MaxWidth = columnDefinition.MaxWidth,
                        Width =  columnDefinition.Width,
                        Name = columnDefinition.Name,
                        SharedSizeGroup = columnDefinition.SharedSizeGroup,
                    });
                }
            }
        }
        
        
        /// <summary>
        /// Gets the row definitions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.RowDefinitions"/> attached property.
        /// </summary>
        /// <param name="obj">The object the row definitions are attached to.</param>
        /// <returns>The row definitions.</returns>
        [CanBeNull, ItemNotNull]
        public static RowDefinitionCollection GetRowDefinitions([NotNull] DependencyObject obj)
        {
            return (RowDefinitionCollection)obj.GetValue(RowDefinitionsProperty);
        }
        /// <summary>
        /// Sets the row definitions attached via the <see cref="P:TomsToolbox.Wpf.StyleBindings.RowDefinitions"/> attached property.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetRowDefinitions([NotNull] DependencyObject obj, [CanBeNull, ItemNotNull] RowDefinitionCollection value)
        {
            obj.SetValue(RowDefinitionsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.StyleBindings.RowDefinitions"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This property is needed to set row definitions via a <see cref="Style"/> or from a resource.
        /// </summary>
        /// <example><code language="XAML"><![CDATA[
        /// <Style TargetType="Grid">
        ///   <Setter Property="core:StyleBindings.RowDefinitions">
        ///     <Setter.Value>
        ///       <core:RowDefinitionCollection>
        ///         <RowDefinition Height="Auto" SharedSizeGroup="Col1" />
        ///         <RowDefinition Height="20" />
        ///         <RowDefinition Height="*" />
        ///       </core:RowDefinitionCollection>
        ///     </Setter.Value>
        ///   </Setter>
        /// ]]>
        /// </code></example>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached("RowDefinitionCollection", typeof(RowDefinitionCollection), typeof(StyleBindings), new FrameworkPropertyMetadata(RowDefinitions_Changed));

        private static void RowDefinitions_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rowDefinitions = (d as Grid)?.RowDefinitions;

            if (rowDefinitions == null)
                return;

            rowDefinitions.Clear();

            if (e.NewValue is RowDefinitionCollection newRowDefinitions)
            {
                foreach (var rowDefinition in newRowDefinitions)
                {
                    rowDefinitions.Add(new RowDefinition
                    {
                        MinHeight = rowDefinition.MinHeight,
                        MaxHeight = rowDefinition.MaxHeight,
                        Height =  rowDefinition.Height,
                        Name = rowDefinition.Name,
                        SharedSizeGroup = rowDefinition.SharedSizeGroup,
                    });
                }
            }
        }

    }
}
