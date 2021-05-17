namespace SampleApp
{
    using System.ComponentModel;
    using System.Composition;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows.Input;

    using TomsToolbox.Wpf;

    [Export]
    class WindowTitleDecoratorViewModel : INotifyPropertyChanged
    {
        public WindowTitleDecoratorViewModel()
        {
            CheckForUpdate();
        }

        public string? FileVersion => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        public string? UpdateLink { get; private set; }

        public ICommand HyperlinkClickCommand => new DelegateCommand(HyperlinkClick);

        private void HyperlinkClick()
        {
            Process.Start(new ProcessStartInfo(UpdateLink) { UseShellExecute = true });
        }

        private async void CheckForUpdate()
        {
            try
            {
                UpdateLink = await TomsToolbox.GitHub.GitHubTasks.FindUpdate("tom-englert", "TomsToolbox");
            }
            catch
            {
                // API limit exceeded.
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
