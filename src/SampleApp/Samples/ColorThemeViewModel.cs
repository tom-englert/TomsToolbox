namespace SampleApp.Samples
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using System.Windows;

    using PropertyChanged;

    using SampleApp.Properties;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.AttributedModel;
    using TomsToolbox.Wpf.Converters;

    enum ColorTheme
    {
        [LocalizedDisplayName(StringResourceKey.ColorTheme_System)]
        [Text(ColorThemeViewModel.DictionaryUriKey, "")]
        System,
        [LocalizedDisplayName(StringResourceKey.ColorTheme_Light)]
        [Text(ColorThemeViewModel.DictionaryUriKey, @"Themes/LightTheme.xaml")]
        Light,
        [LocalizedDisplayName(StringResourceKey.ColorTheme_VisualStudioDark)]
        [Text(ColorThemeViewModel.DictionaryUriKey, @"Themes/DarkTheme.xaml")]
        Dark
    }

    [VisualCompositionExport(RegionId.Main, Sequence = 10)]
    [AddINotifyPropertyChangedInterface]
    [Shared]
    class ColorThemeViewModel : ObservableObject
    {
        public const string DictionaryUriKey = nameof(ColorTheme);

        private readonly Collection<ResourceDictionary> _themeContainer;

        public ColorThemeViewModel()
        {
            var themeDictionary = new ResourceDictionary();
            var applicationDictionaries = Application.Current.Resources.MergedDictionaries;
            applicationDictionaries.Insert(0, themeDictionary);
            _themeContainer = themeDictionary.MergedDictionaries;
        }

        public override string ToString()
        {
            return "Theming";
        }

        [OnChangedMethod(nameof(OnSelectedColorThemeChanged))]
        public ColorTheme SelectedColorTheme { get; set; } 

        private void OnSelectedColorThemeChanged()
        {
            _themeContainer.Clear();

            var relativeUri = ObjectToTextConverter.Convert(DictionaryUriKey, SelectedColorTheme);

            if (relativeUri.IsNullOrEmpty())
                return;

            _themeContainer.Add(new ResourceDictionary{ Source = GetType().Assembly.GeneratePackUri(relativeUri)});
        }
    }
}
