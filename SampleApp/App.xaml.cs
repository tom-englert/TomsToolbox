namespace SampleApp
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Registration;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop.Composition;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Styles;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : IDisposable
    {
        [NotNull]
        private readonly ICompositionHost _compositionHost = new CompositionHost(false);

        public App()
        {
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            // ReSharper disable once PossibleNullReferenceException
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BindingErrorTracer.Start(BindingErrorCallback);

            var context = new RegistrationBuilder();
            // Visuals can't be singletons:
            context.ForTypesDerivedFrom<FrameworkElement>()?.SetCreationPolicy(CreationPolicy.NonShared);
            // To demonstrate the ImportExtension with value converters.
            context.ForTypesDerivedFrom<IValueConverter>()?.Export();
            try
            {
                _compositionHost.AddCatalog(new ApplicationCatalog(context));
            }
            catch (ReflectionTypeLoadException ex)
            {
                MessageBox.Show(ex.Message + "\n" + string.Join("\n", ex.LoaderExceptions.Select(le => le.Message)));
            }


            var dataTemplateResources = DataTemplateManager.CreateDynamicDataTemplates(_compositionHost.Container);
            Resources.MergedDictionaries.Add(dataTemplateResources);

            Resources.MergedDictionaries.Insert(0, WpfStyles.Defaults());

            MainWindow = _compositionHost.GetExportedValue<MainWindow>();
            MainWindow.Show();
        }

        private void BindingErrorCallback([CanBeNull] string msg)
        {
            Dispatcher?.BeginInvoke((Action)(() => MessageBox.Show(msg)));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Dispose();

            base.OnExit(e);
        }

        public void Dispose()
        {
            _compositionHost.Dispose();
        }
        
        
    }
}
