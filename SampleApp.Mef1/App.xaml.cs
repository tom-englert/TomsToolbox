namespace SampleApp.Mef1
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Registration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Composition.Mef;
    using TomsToolbox.Wpf.Composition.XamlExtensions;
    using TomsToolbox.Wpf.Styles;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : IDisposable
    {
        [NotNull]
        private readonly AggregateCatalog _compositionCatalog = new AggregateCatalog();
        private readonly CompositionContainer _compositionContainer;
        private readonly IExportProvider _exportProvider;


        public App()
        {
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            _compositionContainer = new CompositionContainer(_compositionCatalog, true);
            _exportProvider = new ExportProviderAdapter(_compositionContainer);
            _compositionContainer.ComposeExportedValue(_exportProvider);
        }

        protected override void OnStartup([CanBeNull] StartupEventArgs e)
        {
            base.OnStartup(e);

            VisualComposition.Trace += (sender, args) => Trace.WriteLine(args.Text);
            BindingErrorTracer.Start(BindingErrorCallback);

            var context = new RegistrationBuilder();
            // Visuals can't be singletons:
            context.ForTypesDerivedFrom<FrameworkElement>()?.SetCreationPolicy(CreationPolicy.NonShared);
            // To demonstrate the ImportExtension with value converters.
            context.ForTypesDerivedFrom<IValueConverter>()?.Export();

            try
            {
                _compositionCatalog.Catalogs.Add(new ApplicationCatalog(context));
            }
            catch (ReflectionTypeLoadException ex)
            {
                MessageBox.Show(ex.Message + "\n" + string.Join("\n", ex.LoaderExceptions.Select(le => le.Message)));
            }


            Resources.MergedDictionaries.Insert(0, WpfStyles.GetDefaultStyles().RegisterDefaultWindowStyle());
            Resources.MergedDictionaries.Add(DataTemplateManager.CreateDynamicDataTemplates(_exportProvider));

            MainWindow = _compositionContainer.GetExportedValue<MainWindow>();
            MainWindow.Show();
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
            _compositionCatalog.Dispose();
            _compositionContainer.Dispose();
        }
    }
}
