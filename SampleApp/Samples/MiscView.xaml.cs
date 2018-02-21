namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MiscView.xaml
    /// </summary>
    [DataTemplate(typeof(MiscViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MiscView
    {
        public MiscView()
        {
            InitializeComponent();
        }
    }
}
