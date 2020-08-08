namespace SampleApp
{
    using System.Windows;

    using TomsToolbox.Wpf.Composition.AttributedModel;

    /// <summary>
    /// Interaction logic for WindowTitleDecoratorView.xaml
    /// </summary>
    [DataTemplate(typeof(WindowTitleDecoratorViewModel))]
    public partial class WindowTitleDecoratorView
    {
        public WindowTitleDecoratorView()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
