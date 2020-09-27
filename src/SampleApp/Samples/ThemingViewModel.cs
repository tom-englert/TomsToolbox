namespace SampleApp.Samples
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition;
    using System.Windows;

    using PropertyChanged;

    using TomsToolbox.Wpf.Composition.AttributedModel;

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

    enum Theme
    {
        [DisplayName("System Theme")]
        System,
        [DisplayName("VisualStudio Light Theme")]
        Light,
        [DisplayName("VisualStudio Dark Theme")]
        Dark
    }

    [VisualCompositionExport(RegionId.Main, Sequence = 10)]
    [AddINotifyPropertyChangedInterface]
    [Shared]
    class ThemingViewModel
    {
        private readonly Collection<ResourceDictionary> _themeContainer;

        public ThemingViewModel()
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
        public Theme SelectedTheme { get; set; } 

        private void OnSelectedThemeChanged()
        {
            _themeContainer.Clear();

            switch (SelectedTheme)
            {
                case Theme.System:
                    break;

                case Theme.Light:
                    _themeContainer.Add(new ResourceDictionary{ Source = new Uri(@"pack://application:,,,/SampleApp;component/Themes/LightTheme.xaml")});
                    break;

                case Theme.Dark:
                    _themeContainer.Add(new ResourceDictionary{ Source = new Uri(@"pack://application:,,,/SampleApp;component/Themes/DarkTheme.xaml")});
                    break;
            }
        }
    }
}
