﻿namespace SampleApp.Ninject;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

using Microsoft.Extensions.DependencyInjection;

using SampleApp.MicrosoftExtensions.DIAdapters;

using TomsToolbox.Composition;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Composition.XamlExtensions;
using TomsToolbox.Wpf.Styles;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App
{
    private ServiceProvider? _serviceProvider;

    public App()
    {
        // Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        // CustomNonClientAreaBehavior.DisableGlassFrameProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(true));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        VisualComposition.Trace += (_, args) => Trace.WriteLine(args.Text);
        BindingErrorTracer.Start(BindingErrorCallback);


        _serviceProvider = DIAdapter.Initialize();
        var exportProvider = _serviceProvider.GetRequiredService<IExportProvider>();

        ExportProviderLocator.Register(exportProvider);

        Resources.MergedDictionaries.Insert(0, WpfStyles.GetDefaultStyles().RegisterDefaultWindowStyle());
        Resources.MergedDictionaries.Add(DataTemplateManager.CreateDynamicDataTemplates(exportProvider));

        var mainWindow = exportProvider.GetExportedValue<MainWindow>();

        MainWindow = mainWindow;

        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        
        base.OnExit(e);
    }

    private void BindingErrorCallback(string msg)
    {
        if (msg.StartsWith("System.Windows.Data Error: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.DataGrid"))
            return;

        Dispatcher?.BeginInvoke((Action)(() => MessageBox.Show(msg)));
    }
}
