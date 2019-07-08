namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Classifies white spaces in plain text.
    /// </summary>
    [Flags]
    public enum WhiteSpaces
    {
        /// <summary>
        /// No white space
        /// </summary>
        None = 0,
        /// <summary>
        /// Paragraphs (i.e. line breaks)
        /// </summary>
        Paragraph = 1,
        /// <summary>
        /// Spaces
        /// </summary>
        Space = 2,
        /// <summary>
        /// Tab characters
        /// </summary>
        Tab = 4,
        /// <summary>
        /// All white space
        /// </summary>
        All = Paragraph | Space | Tab

    }

    /// <summary>
    /// Makes white space in text boxes visible.
    /// </summary>
    /// <example>
    /// Surround a <see cref="TextBox"/> with the decorator to make white space visible:<para/>
    /// <code language="XAML"><![CDATA[
    /// <local:TextBoxVisibleWhiteSpaceDecorator WhiteSpaces="Paragraph,Space,Tab">
    ///   <TextBox TextWrapping="Wrap" AcceptsReturn="True" AcceptsTab="True"/>
    /// </local:TextBoxVisibleWhiteSpaceDecorator>
    /// ]]></code></example>
    [ContentProperty("Child")]
    public class TextBoxVisibleWhiteSpaceDecorator : FrameworkElement
    {
        [NotNull]
        private readonly AdornerDecorator _adornerDecorator = new AdornerDecorator { ClipToBounds = true };
        [CanBeNull]
        private WhiteSpaceDecoratorAdorner _adorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxVisibleWhiteSpaceDecorator"/> class.
        /// </summary>
        public TextBoxVisibleWhiteSpaceDecorator()
        {
            Loaded += Self_Loaded;

            AddLogicalChild(_adornerDecorator);
            AddVisualChild(_adornerDecorator);
        }

        [NotNull]
        private AdornerLayer AdornerLayer => _adornerDecorator.AdornerLayer;

        /// <summary>
        /// Gets or sets the white spaces to show.
        /// </summary>
        public WhiteSpaces WhiteSpaces
        {
            get => this.GetValue<WhiteSpaces>(WhiteSpacesProperty);
            set => SetValue(WhiteSpacesProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaces"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty WhiteSpacesProperty =
            DependencyProperty.Register("WhiteSpaces", typeof(WhiteSpaces), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(WhiteSpaces.Paragraph, (sender, e) => ((TextBoxVisibleWhiteSpaceDecorator)sender)?._adorner?.UpdateWhiteSpaces()));


        /// <summary>
        /// Gets or sets the color of the white space visualization.
        /// </summary>
        [CanBeNull]
        public Brush WhiteSpaceColor
        {
            get => (Brush)GetValue(WhiteSpaceColorProperty);
            set => SetValue(WhiteSpaceColorProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaceColor"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty WhiteSpaceColorProperty =
            DependencyProperty.Register("WhiteSpaceColor", typeof(Brush), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(Brushes.Gray, (sender, e) => ((TextBoxVisibleWhiteSpaceDecorator)sender)?._adorner?.InvalidateVisual()));


        /// <summary>
        /// Gets or sets the opacity of the white space visualization.
        /// </summary>
        public double WhiteSpaceOpacity
        {
            get => this.GetValue<double>(WhiteSpaceOpacityProperty);
            set => SetValue(WhiteSpaceOpacityProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaceOpacity"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty WhiteSpaceOpacityProperty =
            DependencyProperty.Register("WhiteSpaceOpacity", typeof(double), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(1.0, (sender, e) => ((TextBoxVisibleWhiteSpaceDecorator)sender)?._adorner?.InvalidateVisual()));

        /// <summary>
        /// Gets or sets the inner text box.
        /// </summary>
        [CanBeNull]
        public TextBox Child
        {
            get => _adornerDecorator.Child as TextBox;
            set => _adornerDecorator.Child = value;
        }

        /// <inheritdoc />
        [CanBeNull]
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return _adornerDecorator;
            }
        }

        /// <inheritdoc />
        [CanBeNull]
        protected override Visual GetVisualChild(int index)
        {
            return (index == 0) ? _adornerDecorator : null;
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => 1;

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            _adornerDecorator.Measure(availableSize);
            return _adornerDecorator.DesiredSize;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerDecorator.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private void Self_Loaded([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            var textBox = Child;
            if (textBox == null)
                return;

            AdornerLayer.Add(_adorner = new WhiteSpaceDecoratorAdorner(this, textBox));
        }

        private class WhiteSpaceDecoratorAdorner : Adorner
        {
            [NotNull]
            private readonly TextBoxVisibleWhiteSpaceDecorator _owner;
            [NotNull]
            private readonly TextBox _textBox;
            [NotNull, ItemNotNull]
            private IList<WhiteSpace> _whiteSpaces = new WhiteSpace[0];

            public WhiteSpaceDecoratorAdorner([NotNull] TextBoxVisibleWhiteSpaceDecorator owner, [NotNull] TextBox textBox)
                : base(textBox)
            {
                _owner = owner;
                _textBox = textBox;

                textBox.TextChanged += TextBox_TextChanged;

                if (textBox.Template?.FindName("PART_ContentHost", textBox) is ScrollViewer scrollViewer)
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

                UpdateWhiteSpaces();
            }

            private void ScrollViewer_ScrollChanged([CanBeNull] object sender, [CanBeNull] ScrollChangedEventArgs e)
            {
                InvalidateVisual();
            }

            private void TextBox_TextChanged([CanBeNull] object sender, [CanBeNull] TextChangedEventArgs e)
            {
                UpdateWhiteSpaces();
            }

            internal void UpdateWhiteSpaces()
            {
                var textBox = _textBox;
                var whiteSpaces = _owner.WhiteSpaces;

                _whiteSpaces = textBox.Text
                    .Select((character, index) => GetWhiteSpace(character, index, whiteSpaces))
                    .Where(item => item != null)
                    .ToArray();

                this.BeginInvoke(DispatcherPriority.Background, InvalidateVisual);
            }

            [CanBeNull]
            private static WhiteSpace GetWhiteSpace(char character, int index, WhiteSpaces whiteSpaces)
            {
                switch (character)
                {
                    case '\n':
                        return (whiteSpaces & WhiteSpaces.Paragraph) != 0 ? new WhiteSpace(index, "¶") : null;
                    case ' ':
                        return (whiteSpaces & WhiteSpaces.Space) != 0 ? new WhiteSpace(index, "∙") : null;
                    case '\xA0':
                        return (whiteSpaces & WhiteSpaces.Space) != 0 ? new WhiteSpace(index, "°") : null;
                    case '\t':
                        return (whiteSpaces & WhiteSpaces.Tab) != 0 ? new WhiteSpace(index, "→") : null;

                    default:
                        return null;
                }
            }

            private static void DrawAdorners([NotNull] DrawingContext drawingContext, [NotNull] TextBox textBox, [NotNull] [ItemNotNull] IList<WhiteSpace> adorners, int firstIndex, int lastIndex, Size desiredSize, [NotNull] Typeface typeface, double fontSize, [NotNull] Brush brush)
            {
                while (true)
                {
                    if (lastIndex <= firstIndex)
                        return;

                    var index = (lastIndex + firstIndex) / 2;

                    var adorner = adorners[index];
                    var rect = adorner.GetDesiredRect(textBox);
                    if (rect.IsEmpty)
                        return;

                    if (rect.Top < 0)
                    {
                        firstIndex = index + 1;
                        continue;
                    }

                    if (rect.Bottom > desiredSize.Height)
                    {
                        lastIndex = index;
                        continue;
                    }

                    if ((rect.Right >= 0) && (rect.Left <= desiredSize.Width))
                    {
                        drawingContext.DrawText(new FormattedText(adorner.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, brush), rect.TopLeft);
                    }

                    DrawAdorners(drawingContext, textBox, adorners, index + 1, lastIndex, desiredSize, typeface, fontSize, brush);

                    lastIndex = index;
                }
            }

            protected override int VisualChildrenCount => 0;

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                drawingContext.PushOpacity(_owner.WhiteSpaceOpacity);

                var typefaceName = _textBox.FontFamily?.Source ?? "Segoe UI";
                var brush = _owner.WhiteSpaceColor ?? Brushes.Gray; 
                DrawAdorners(drawingContext, _textBox, _whiteSpaces, 0, _whiteSpaces.Count, RenderSize, new Typeface(typefaceName), _textBox.FontSize, brush);

                drawingContext.Pop();
            }

            private class WhiteSpace
            {
                private readonly int _charIndex;

                public WhiteSpace(int charIndex, [NotNull] string text)
                {
                    _charIndex = charIndex;
                    Text = text;
                }

                [NotNull]
                public string Text { get; }

                public Rect GetDesiredRect([NotNull] TextBox textBox)
                {
                    return textBox.Text.Length <= _charIndex ? Rect.Empty : textBox.GetRectFromCharacterIndex(_charIndex);
                }
            }
        }
    }
}
