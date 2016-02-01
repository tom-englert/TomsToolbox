namespace SampleApp
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Registration;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Context;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    using SampleApp.Samples;

    using TomsToolbox.Desktop.Composition;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : IDisposable
    {
        private readonly ICompositionHost _compositionHost = new CompositionHost(false);

        public App()
        {
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BindingErrorTracer.Start(BindingErrorCallback);

            var context = new RegistrationBuilder();
            // Visuals can't be singletons:
            context.ForTypesDerivedFrom<FrameworkElement>().SetCreationPolicy(CreationPolicy.NonShared);
            // To demonstrate the ImportExtension with value converters.
            context.ForTypesDerivedFrom<IValueConverter>().Export();

            _compositionHost.AddCatalog(new ApplicationCatalog(context));

            var dataTemplateResources = DataTemplateManager.CreateDynamicDataTemplates(_compositionHost.Container);
            Resources.MergedDictionaries.Add(dataTemplateResources);

            MainWindow = _compositionHost.GetExportedValue<MainWindow>();
            MainWindow.Show();
        }

        private void BindingErrorCallback(string msg)
        {
            Dispatcher.BeginInvoke((Action)(() => MessageBox.Show(msg)));
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

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_compositionHost != null);
        }
    }
}
