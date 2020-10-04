namespace SampleApp.Samples
{
    using System.Collections.Generic;
    using System.Linq;

    using AVL.Styx;

    using PropertyChanged;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    [VisualCompositionExport(RegionId.Main, Sequence=9)]
    [AddINotifyPropertyChangedInterface]
    class StyxViewModel
    {
        public bool IsEnabled { get; set; } = true;

        public IList<ResourceItem> Styles { get; } = ResourceItem.GetAll(typeof(TomsToolbox.Wpf.Styles.ResourceKeys), "Style").OrderBy(item => item.Key.ResourceId).ToArray();

        public override string ToString()
        {
            return "Styx";
        }
    }
}
