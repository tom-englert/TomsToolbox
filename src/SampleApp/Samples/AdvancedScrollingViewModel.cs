using System.ComponentModel;
using System.Composition;

using TomsToolbox.Wpf.Composition.AttributedModel;

namespace SampleApp.Samples;

[Export]
[VisualCompositionExport(RegionId.Main, Sequence = 12)]
[Shared]
public partial class AdvancedScrollingViewModel : INotifyPropertyChanged
{
    private static readonly Random _randomNumberGenerator = new();

    public override string ToString()
    {
        return "Advanced Scrolling";
    }

    public ICollection<string> SampleData { get; } = Enumerable.Range(1, 10000)
        .Select(i => string.Join(" ", Enumerable.Range(i, 1000).Select(k => _randomNumberGenerator.Next('A', 'Z'))))
        .ToList();
}
