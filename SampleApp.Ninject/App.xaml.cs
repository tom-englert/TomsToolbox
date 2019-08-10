namespace SampleApp.Ninject
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using SampleApp.Ninject.DIAdapters;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Composition.XamlExtensions;
    using TomsToolbox.Wpf.Styles;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : IDisposable
    {
        private DIAdapter _diAdapter;

        public App()
        {
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnStartup([CanBeNull] StartupEventArgs e)
        {
            base.OnStartup(e);

            VisualComposition.Trace += (sender, args) => Trace.WriteLine(args.Text);
            BindingErrorTracer.Start(BindingErrorCallback);

            _diAdapter = new DIAdapter();

            var exportProvider = _diAdapter.Initialize();

            Resources.MergedDictionaries.Insert(0, WpfStyles.GetDefaultStyles().RegisterDefaultWindowStyle());
            Resources.MergedDictionaries.Add(DataTemplateManager.CreateDynamicDataTemplates(exportProvider));

            var mainWindow = exportProvider.GetExportedValue<MainWindow>();

            MainWindow = mainWindow;

            mainWindow.Show();
        }

        private void BindingErrorCallback([CanBeNull] string msg)
        {
            Dispatcher?.BeginInvoke((Action)(() => MessageBox.Show(msg)));
        }

        protected override void OnExit([CanBeNull] ExitEventArgs e)
        {
            Dispose();

            base.OnExit(e);
        }

        public void Dispose()
        {
            _diAdapter?.Dispose();
        }
    }
}
