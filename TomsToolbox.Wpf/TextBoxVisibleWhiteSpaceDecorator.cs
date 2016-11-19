namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;

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
        [NotNull]
        private readonly AdornerLayer _adornerLayer;
        private ScrollViewer _scrollViewer;
        [NotNull]
        private IList<TextAdorner> _adorners = new TextAdorner[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxVisibleWhiteSpaceDecorator"/> class.
        /// </summary>
        public TextBoxVisibleWhiteSpaceDecorator()
        {
            Loaded += Self_Loaded;

            AddLogicalChild(_adornerDecorator);
            AddVisualChild(_adornerDecorator);

            // ReSharper disable once AssignNullToNotNullAttribute
            _adornerLayer = _adornerDecorator.AdornerLayer;
        }

        /// <summary>
        /// Gets or sets the white spaces to show.
        /// </summary>
        public WhiteSpaces WhiteSpaces
        {
            get { return this.GetValue<WhiteSpaces>(WhiteSpacesProperty); }
            set { SetValue(WhiteSpacesProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaces"/> dependency property
        /// </summary>
        public static readonly DependencyProperty WhiteSpacesProperty =
            DependencyProperty.Register("WhiteSpaces", typeof(WhiteSpaces), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(WhiteSpaces.Paragraph, (sender, e) => ((TextBoxVisibleWhiteSpaceDecorator)sender)?.UpdateAdorners()));


        /// <summary>
        /// Gets or sets the color of the white space visualization.
        /// </summary>
        public Brush WhiteSpaceColor
        {
            get { return (Brush)GetValue(WhiteSpaceColorProperty); }
            set { SetValue(WhiteSpaceColorProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaceColor"/> dependency property
        /// </summary>
        public static readonly DependencyProperty WhiteSpaceColorProperty =
            DependencyProperty.Register("WhiteSpaceColor", typeof(Brush), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(Brushes.Gray));


        /// <summary>
        /// Gets or sets the opacity of the white space visualization.
        /// </summary>
        public double WhiteSpaceOpacity
        {
            get { return this.GetValue<double>(WhiteSpaceOpacityProperty); }
            set { SetValue(WhiteSpaceOpacityProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="WhiteSpaceOpacity"/> dependency property
        /// </summary>
        public static readonly DependencyProperty WhiteSpaceOpacityProperty =
            DependencyProperty.Register("WhiteSpaceOpacity", typeof(double), typeof(TextBoxVisibleWhiteSpaceDecorator), new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the inner text box.
        /// </summary>
        public TextBox Child
        {
            get
            {
                return _adornerDecorator.Child as TextBox;
            }
            set
            {
                _adornerDecorator.Child = value;
            }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return _adornerDecorator;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return (index == 0) ? _adornerDecorator : null;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount => 1;

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _adornerDecorator.Measure(availableSize);
            return _adornerDecorator.DesiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerDecorator.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private void Self_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = Child;
            if (textBox == null)
                return;

            textBox.TextChanged += TextBox_TextChanged;

            var template = textBox.Template;
            if (template == null)
                return;

            _scrollViewer = template.FindName("PART_ContentHost", textBox) as ScrollViewer;
            if (_scrollViewer == null)
                return;

            _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

            UpdateAdorners();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.BeginInvoke(DispatcherPriority.Background, _adornerLayer.Update);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.BeginInvoke( DispatcherPriority.Background, _adornerLayer.Update);

            UpdateAdorners((TextBox)sender);
        }

        private void UpdateAdorners()
        {
            UpdateAdorners(Child);
        }

        private void UpdateAdorners(TextBox textBox)
        {
            if (textBox == null)
                return;

            using (var adornersSource = _adorners.GetEnumerator())
            {
                var oldAdorners = _adorners;

                // ReSharper disable AssignNullToNotNullAttribute
                // ReSharper disable PossibleNullReferenceException
                _adorners = textBox.Text
                    .Select((character, index) => new { text = GetAdornerText(character), index })
                    .Where(item => item.text != null)
                    .Select(item => GetNextAdorner(adornersSource, item.index, item.text))
                    .ToArray();

                foreach (var item in oldAdorners.Skip(_adorners.Count))
                {
                    _adornerLayer.Remove(item);
                }
                // ReSharper restore PossibleNullReferenceException
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        private string GetAdornerText(char character)
        {
            switch (character)
            {
                case '\n':
                    return (WhiteSpaces & WhiteSpaces.Paragraph) != 0 ? "¶" : null;
                case ' ':
                    return (WhiteSpaces & WhiteSpaces.Space) != 0 ? "∙" : null;
                case '\xA0':
                    return (WhiteSpaces & WhiteSpaces.Space) != 0 ? "°" : null;
                case '\t':
                    return (WhiteSpaces & WhiteSpaces.Tab) != 0 ? "→" : null;

                default:
                    return null;
            }
        }

        [NotNull]
        private TextAdorner GetNextAdorner([NotNull] IEnumerator<TextAdorner> existingAdorners, int charIndex, string text)
        {
            Contract.Requires(existingAdorners != null);

            var textAdorner = existingAdorners.MoveNext() ? existingAdorners.Current : CreateNewAdorner();
            Contract.Assume(textAdorner != null);

            return textAdorner.Apply(charIndex, text);
        }

        [NotNull]
        private TextAdorner CreateNewAdorner()
        {
            var textBox = Child;
            Contract.Assume(textBox != null);

            var newAdorner = new TextAdorner(this, textBox);
            _adornerLayer.Add(newAdorner);
            return newAdorner;
        }

        class TextAdorner : Adorner
        {
            [NotNull]
            private readonly TextBox _textBox;
            [NotNull]
            private readonly TextBlock _content = new TextBlock();
            private int _charIndex;

            public TextAdorner([NotNull] TextBoxVisibleWhiteSpaceDecorator owner, [NotNull] TextBox adornedElement)
                : base(adornedElement)
            {
                Contract.Requires(owner != null);
                Contract.Requires(adornedElement != null);

                _textBox = adornedElement;
                BindingOperations.SetBinding(_content, TextBlock.FontSizeProperty, new Binding { Path = new PropertyPath(TextBlock.FontSizeProperty), Source = adornedElement });
                BindingOperations.SetBinding(_content, TextBlock.ForegroundProperty, new Binding { Path = new PropertyPath(WhiteSpaceColorProperty), Source = owner });
                BindingOperations.SetBinding(_content, OpacityProperty, new Binding { Path = new PropertyPath(WhiteSpaceOpacityProperty), Source = owner });
            }

            [NotNull]
            public TextAdorner Apply(int charIndex, string text)
            {
                _charIndex = charIndex;
                _content.Text = text;

                return this;
            }

            [NotNull]
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                var baseTransform = base.GetDesiredTransform(transform);
                var desiredTransform = CreateTransform();

                if ((baseTransform != null) && !baseTransform.Equals(Transform.Identity))
                {
                    return baseTransform.MergeWith(desiredTransform);
                }

                return desiredTransform;
            }

            protected override int VisualChildrenCount => 1;

            [NotNull]
            protected override Visual GetVisualChild(int index)
            {
                return _content;
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _content.Measure(constraint);
                return _content.DesiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _content.Arrange(new Rect(finalSize));
                return finalSize;
            }

            [NotNull]
            private GeneralTransform CreateTransform()
            {
                Contract.Ensures(Contract.Result<GeneralTransform>() != null);

                try
                {
                    var rect = _textBox.GetRectFromCharacterIndex(_charIndex);
                    if (!rect.IsEmpty)
                    {
                        var nextIndex = _charIndex + 1;
       
                        // ReSharper disable once PossibleNullReferenceException
                        if (nextIndex < _textBox.Text.Length)
                        {
                            var rect2 = _textBox.GetRectFromCharacterIndex(nextIndex);
                            if (!rect2.IsEmpty && (Math.Abs(rect2.Top - rect.Top) < double.Epsilon))
                            {
                                return new TranslateTransform { X = (rect.Left + rect2.Left - _content.DesiredSize.Width) / 2, Y = rect.Top };
                            }
                        }

                        return new TranslateTransform { X = rect.Left + 1, Y = rect.Top };
                    }
                }
                catch (ArgumentException)
                {
                }

                return Transform.Identity;
            }

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            [Conditional("CONTRACTS_FULL")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_textBox != null);
                Contract.Invariant(_content != null);
            }
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_adornerDecorator != null);
            Contract.Invariant(_adornerLayer != null);
            Contract.Invariant(_adorners != null);
        }
    }
}
