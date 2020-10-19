namespace SampleApp.Samples
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using System.Windows;

    using PropertyChanged;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf.Composition.AttributedModel;
    using TomsToolbox.Wpf.Converters;

    [AttributeUsage(AttributeTargets.All)]
    public class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        public DisplayNameAttribute() : this(string.Empty)
        {
        }

        public DisplayNameAttribute(string displayName) : base(displayName)
        {
        }
    }



    enum ColorTheme
    {
        [DisplayName("System")]
        [Text(ColorThemeViewModel.DictionaryUriKey, "")]
        System,
        [DisplayName("VisualStudio Light")]
        [Text(ColorThemeViewModel.DictionaryUriKey, @"Themes/LightTheme.xaml")]
        Light,
        [DisplayName("VisualStudio Dark")]
        [Text(ColorThemeViewModel.DictionaryUriKey, @"Themes/DarkTheme.xaml")]
        Dark
    }

    [VisualCompositionExport(RegionId.Main, Sequence = 10)]
    [AddINotifyPropertyChangedInterface]
    [Shared]
    class ColorThemeViewModel
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

        [OnChangedMethod(nameof(OnSelectedThemeChanged))]
        public ColorTheme SelectedColorTheme { get; set; } 

        private void OnSelectedThemeChanged()
        {
            _themeContainer.Clear();

            var relativeUri = ObjectToTextConverter.Convert(DictionaryUriKey, SelectedColorTheme);

            if (string.IsNullOrEmpty(relativeUri))
                return;

            _themeContainer.Add(new ResourceDictionary{ Source = GetType().Assembly.GeneratePackUri(relativeUri)});
        }
    }
}
