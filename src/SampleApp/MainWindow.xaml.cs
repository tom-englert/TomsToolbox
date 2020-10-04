namespace SampleApp
{
    using System.Composition;
    using System.Windows;
    using System.Windows.Media;

    using JetBrains.Annotations;

    using SampleApp.Themes;

    using TomsToolbox.Composition;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export, Export(typeof(IThemeManager)), Shared]
    public partial class MainWindow : IThemeManager
    {
        [ImportingConstructor]
        public MainWindow([NotNull] IExportProvider exportProvider)
        {
            this.SetExportProvider(exportProvider);

            InitializeComponent();
        }

        private void ShowPopup_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow { Owner = this }.ShowDialog();
        }

        public bool IsDarkTheme
        {
            get => (bool) GetValue(IsDarkThemeProperty);
            set => SetValue(IsDarkThemeProperty, value);
        }
        public static readonly DependencyProperty IsDarkThemeProperty = DependencyProperty.Register(
            "IsDarkTheme", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ((e.Property != ForegroundProperty) && (e.Property != BackgroundProperty))
                return;

            var foreground = ToGray((Foreground as SolidColorBrush)?.Color);
            var background = ToGray((Background as SolidColorBrush)?.Color);

            IsDarkTheme = background < foreground;
        }

        private static double ToGray(Color? color)
        {
            return color?.R * 0.3 + color?.G * 0.59 + color?.B * 0.11 ?? 0.0;
        }
    }
}