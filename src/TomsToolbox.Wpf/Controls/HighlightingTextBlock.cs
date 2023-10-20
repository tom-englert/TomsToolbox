namespace TomsToolbox.Wpf.Controls;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using global::Throttle;

using TomsToolbox.Essentials;

/// <summary>
/// A <see cref="TextBlock"/> like control that highlights a search text.
/// </summary>
public class HighlightingTextBlock : ContentControl
{
    private readonly TextBlock _textBlock = new() { Focusable = false };

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingTextBlock" /> class.
    /// </summary>
    public HighlightingTextBlock()
    {
        Content = _textBlock;
    }

    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public object Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="Text"/> property
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(object), typeof(HighlightingTextBlock),
        new PropertyMetadata(default(object), (o, args) => ((HighlightingTextBlock)o).ConstraintsChanged()));

    /// <summary>
    /// Gets or sets the search text that will be highlighted.
    /// </summary>
    public object SearchText
    {
        get { return GetValue(SearchTextProperty); }
        set { SetValue(SearchTextProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="SearchText"/> property
    /// </summary>
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
        nameof(SearchText), typeof(object), typeof(HighlightingTextBlock),
        new PropertyMetadata(default(object), (o, args) => ((HighlightingTextBlock)o).SearchTextChanged()));

    /// <summary>
    /// Gets or sets the brush to high light the search text.
    /// </summary>
    public Brush HighLightBrush
    {
        get { return (Brush)GetValue(HighLightBrushProperty); }
        set { SetValue(HighLightBrushProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="HighLightBrush"/> property
    /// </summary>
    public static readonly DependencyProperty HighLightBrushProperty = DependencyProperty.Register(
        nameof(HighLightBrush), typeof(Brush), typeof(HighlightingTextBlock),
        new PropertyMetadata(default(Brush), (o, args) => ((HighlightingTextBlock)o).ConstraintsChanged()));

    /// <summary>
    /// Gets or sets the font weight applied to the search text; default is bold.
    /// </summary>
    public FontWeight HighLightFontWeight
    {
        get { return (FontWeight)GetValue(HighLightFontWeightProperty); }
        set { SetValue(HighLightFontWeightProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="HighLightFontWeight"/> property
    /// </summary>
    public static readonly DependencyProperty HighLightFontWeightProperty = DependencyProperty.Register(
        nameof(HighLightFontWeight), typeof(FontWeight), typeof(HighlightingTextBlock),
        new PropertyMetadata(FontWeights.Bold, (o, args) => ((HighlightingTextBlock)o).ConstraintsChanged()));

    /// <summary>
    /// Gets or sets the string comparison used to find the highlighting text; default is <see cref="StringComparison.OrdinalIgnoreCase"/>
    /// </summary>
    public StringComparison StringComparison
    {
        get { return (StringComparison)GetValue(StringComparisonProperty); }
        set { SetValue(StringComparisonProperty, value); }
    }
    /// <summary>
    /// Identifies the <see cref="StringComparison"/> property
    /// </summary>
    public static readonly DependencyProperty StringComparisonProperty = DependencyProperty.Register(
        nameof(StringComparison), typeof(StringComparison), typeof(HighlightingTextBlock),
        new PropertyMetadata(StringComparison.OrdinalIgnoreCase, (o, args) => ((HighlightingTextBlock)o).ConstraintsChanged()));

    /// <summary>
    /// Gets the inlines to render in the <see cref="TextBlock"/>
    /// </summary>
    protected InlineCollection Inlines => _textBlock.Inlines;

    /// <summary>
    /// Updates the inlines after any constraints have changed.
    /// </summary>
    protected virtual void UpdateInlines()
    {
        CreateInlines(Inlines, Text, SearchText, HighLightBrush, HighLightFontWeight, StringComparison);
    }

    [Throttled(typeof(Throttle), 500)]
    private void SearchTextChanged()
    {
        UpdateInlines();
    }

    [Throttled(typeof(DispatcherThrottle), (int)DispatcherPriority.Input)]
    private void ConstraintsChanged()
    {
        UpdateInlines();
    }

    private static void CreateInlines(InlineCollection inlines, object value, object parameter, Brush highlightBrush, FontWeight fontWeight, StringComparison stringComparison)
    {
        var newInlines = new Collection<Inline>();

        var text = value?.ToString();

        if (!text.IsNullOrEmpty())
        {
            var searchText = parameter?.ToString();

            if (searchText.IsNullOrEmpty())
            {
                newInlines.Add(new Run(text));
            }
            else
            {
                var searchLength = searchText.Length;

                for (var index = 0; ;)
                {
                    var pos = text.IndexOf(searchText, index, stringComparison);

                    if (pos < 0)
                    {
                        newInlines.Add(new Run(text.Substring(index)));
                        break;
                    }

                    if (pos > index)
                    {
                        newInlines.Add(new Run(text.Substring(index, pos - index)));
                    }

                    newInlines.Add(new Run(text.Substring(pos, searchLength))
                    {
                        FontWeight = fontWeight,
                        Foreground = highlightBrush
                    });

                    index = pos + searchLength;
                    if (index >= text.Length)
                        break;
                }
            }
        }

        if (newInlines.SequenceEqual(inlines, RunComparer.Instance))
            return;

        inlines.Clear();
        inlines.AddRange(newInlines);
    }

    private sealed class RunComparer : IEqualityComparer<Inline>
    {
        public static readonly RunComparer Instance = new();

        public bool Equals(Inline? x, Inline? y)
        {
            return ReferenceEquals(x, y) || Equals(x as Run, y as Run);
        }

        private static bool Equals(Run? x, Run? y)
        {
            if (x is null || y is null)
                return false;

            return string.Equals(x.Text, y.Text, StringComparison.Ordinal) && Equals(x.FontWeight, y.FontWeight) && Equals(x.Foreground, y.Foreground);
        }

        public int GetHashCode(Inline obj)
        {
            if (obj is not Run run)
                return 0;

            return new[]
            {
                run.Text.GetHashCode(),
                run.FontWeight.GetHashCode(),
                run.Foreground.GetHashCode()
            }.Aggregate(HashCode.Aggregate);
        }
    }
}
