namespace SampleApp.Samples;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.Main, Sequence=9)]
internal partial class StyxViewModel : INotifyPropertyChanged
{
    public bool IsEnabled { get; set; } = true;

    public IList<ResourceItem> Styles { get; } = ResourceItem.GetAll(typeof(TomsToolbox.Wpf.Styles.ResourceKeys), "Style").OrderBy(item => item.Key.ResourceId).ToArray();

    public override string ToString()
    {
        return "Styx";
    }
}
