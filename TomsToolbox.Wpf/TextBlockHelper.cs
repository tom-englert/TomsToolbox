namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// Attached property provider which adds the read-only attached property <see cref="IsTextTrimmedProperty"/> to
    /// the framework's <see cref="TextBlock"/> control.
    /// Can be used to e.g. show a dynamic tool tip with the full text, that shows up only if the text is really trimmed.
    /// </summary>
    /// <remarks>
    /// Remember to use NotifyOnTargetUpdated=True for bindings, if you need the <see cref="TextBlockHelper"/> to react to changes to the TextBlock.Text property!
    /// </remarks>
    public static class TextBlockHelper
    {
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static TextBlockHelper()
        {
            EventManager.RegisterClassHandler(typeof(TextBlock), FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(TextBlock_SizeChanged), true);
            EventManager.RegisterClassHandler(typeof(TextBlock), Binding.TargetUpdatedEvent, new EventHandler<DataTransferEventArgs>(Binding_TargetUpdated), true);
        }

        /// <summary>
        /// Gets the value of the <see cref="P:TomsToolbox.Wpf.TextBlockHelper.IsTextTrimmed"/> attached property.
        /// </summary>
        /// <param name="obj">The text block.</param>
        /// <returns><c>true</c> if text trimmed inside the text block; otherwise <c>false</c></returns>
        public static bool GetIsTextTrimmed([NotNull] DependencyObject obj)
        {
            return obj.GetValue<bool>(IsTextTrimmedProperty);
        }
        private static void SetIsTextTrimmed([NotNull] DependencyObject obj, bool value)
        {
            obj.SetValue(IsTextTrimmedPropertyKey, value);
        }
        [NotNull] 
        private static readonly DependencyPropertyKey IsTextTrimmedPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBlockHelper), new FrameworkPropertyMetadata());
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.TextBlockHelper.IsTextTrimmed"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// If the <see cref="P:TomsToolbox.Wpf.TextBlockHelper.IsAutomaticToolTipEnabled"/> attached property is set to <c>true</c> on a text block, this property
        /// reflects if the text inside the text block is trimmed, i.e. not fully visible.
        /// </summary>
        /// </AttachedPropertyComments>
        [NotNull] 
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating if the automatic tool tip is enabled on this text block or not.
        /// </summary>
        /// <param name="obj">The <see cref="TextBlock"/> to evaluate.</param>
        /// <returns><c>true</c> if the automatic tool tip is enabled; otherwise <c>false</c></returns>
        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static bool GetIsAutomaticToolTipEnabled([NotNull] DependencyObject obj)
        {
            return obj.GetValue<bool>(IsAutomaticToolTipEnabledProperty);
        }
        /// <summary>
        /// Sets a value indicating if the automatic tool tip is enabled on this text block or not.
        /// </summary>
        /// <param name="obj">The <see cref="TextBlock"/> to evaluate.</param>
        /// <param name="value"><c>true</c> to enable the automatic tool tip; otherwise <c>false</c></param>
        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static void SetIsAutomaticToolTipEnabled([NotNull] DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutomaticToolTipEnabledProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.TextBlockHelper.IsAutomaticToolTipEnabled"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Set this property to <c>true</c> to enable automatic evaluation of the <see cref="P:TomsToolbox.Wpf.TextBlockHelper.IsTextTrimmed"/> attached property.
        /// This property is used by the style identified with the <see cref="ResourceKeys.AutoToolTipTextBoxStyle"/> to display of a tool tip only if the text of the text block is trimmed.
        /// </summary>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty IsAutomaticToolTipEnabledProperty
            = DependencyProperty.RegisterAttached("IsAutomaticToolTipEnabled", typeof(bool), typeof(TextBlockHelper), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Event handler for TextBlock's SizeChanged routed event. Triggers evaluation of the IsTextTrimmed attached property.
        /// </summary>
        /// <param name="sender">Object where the event handler is attached</param>
        /// <param name="e">Event data</param>
        private static void TextBlock_SizeChanged([NotNull] object sender, [NotNull] SizeChangedEventArgs e)
        {
            if (!(sender is TextBlock textBlock))
                return;

            if (!e.WidthChanged)
                return;

            if (!textBlock.IsArrangeValid)
                return;

            UpdateIsTextTrimmed(textBlock);
        }

        /// <summary>
        /// Event handler for TextBlock's TargetUpdated routed event. Triggers evaluation of the
        /// IsTextTrimmed attached property.
        /// </summary>
        /// <param name="sender">Object where the event handler is attached</param>
        /// <param name="e">Event data</param>
        private static void Binding_TargetUpdated([NotNull] object sender, [NotNull] DataTransferEventArgs e)
        {
            if (!(sender is TextBlock textBlock))
                return;

            if (e.Property != TextBlock.TextProperty)
                return;

            UpdateIsTextTrimmed(textBlock);
        }

        /// <summary>
        /// Update the value of IsTextTrimmed.
        /// </summary>
        /// <param name="textBlock">The text block</param>
        private static void UpdateIsTextTrimmed([NotNull] TextBlock textBlock)
        {
            SetIsTextTrimmed(textBlock, (textBlock.TextTrimming != TextTrimming.None) && EvaluateIsTextTrimmed(textBlock));
        }

        /// <summary>
        /// Determines whether or not the text in <paramref name="textBlock"/> is currently being trimmed due to width or height constraints.
        /// </summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to evaluate.</param>
        /// <returns><c>true</c> if the text is currently being trimmed; otherwise <c>false</c></returns>
        private static bool EvaluateIsTextTrimmed([NotNull] TextBlock textBlock)
        {
            var fontFamily = textBlock.FontFamily;
            var text = textBlock.Text;

            if ((fontFamily == null) || (text == null))
                return false;

            var typeface = new Typeface(fontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);

            var numberSubstitution = new NumberSubstitution(NumberSubstitution.GetCultureSource(textBlock), NumberSubstitution.GetCultureOverride(textBlock), NumberSubstitution.GetSubstitution(textBlock));
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground, numberSubstitution, TextOptions.GetTextFormattingMode(textBlock));

            var padding = textBlock.Padding;
            var actualWidth = textBlock.ActualWidth - padding.Left - padding.Right;
            var actualHeight = textBlock.ActualHeight - padding.Top - padding.Bottom;

            if (textBlock.TextWrapping != TextWrapping.NoWrap)
                formattedText.MaxTextWidth = actualWidth;

            var isTextTrimmed = ((Math.Floor(formattedText.Height) - actualHeight) > 0.0001) || (Math.Floor(formattedText.Width) - actualWidth > 0.0001);

            return isTextTrimmed;
        }
    }
}
