namespace SampleApp.Samples;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Input;

using TomsToolbox.Desktop;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

[VisualCompositionExport(RegionId.Main, Sequence = 99)]
internal partial class MiscViewModel : INotifyPropertyChanged, IDataErrorInfo
{
    private string? _userName;

    public MiscViewModel()
    {
        SelectedItemsAsObservableCollection = new ObservableCollection<KeyValuePair<int, string>>([Items[0], Items[1], Items[2]]);
        SelectedItemsAsObservableCollection.CollectionChanged += SelectedItemsAsObservableCollection_CollectionChanged;
    }

    public override string ToString()
    {
        return "Misc.";
    }

    public DateTime OperationStarted { get; set; } = DateTime.Now;

    public TimeSpan MinimumDuration { get; set; } = TimeSpan.FromMinutes(0.2);

    public ICommand ItemsControlDefaultCommand => new DelegateCommand<string>(item => MessageBox.Show(item + " clicked."));

    public ICommand GCCollectCommand => new DelegateCommand(GC.Collect);

    public ICommand CheckForAdminRightsCommand => new DelegateCommand(CheckForAdminRights);

    public string? TextWithError { get; set; }

    public ICommand MouseClickedCommand => new DelegateCommand(() => MessageBox.Show("clicked!."));

    private void CheckForAdminRights()
    {
        bool CheckRights()
        {
            var credential = new NetworkCredential(_userName ?? string.Empty, string.Empty);
            var lastError = 0;

            var parent = Application.Current?.MainWindow?.GetWindowHandle() ?? IntPtr.Zero;

            while ((credential = UserAccountControl.PromptForCredential(parent, "Test", "Login", lastError, credential)) != null)
            {
                lastError = credential.LogOnInteractiveUser(out var userToken);

                using (userToken)
                {
                    if (lastError != 0 || userToken == null)
                    {
                        continue;
                    }

                    if (UserAccountControl.IsUserInAdminGroup(userToken) ||
                        UserAccountControl.IsUserInGroup(userToken, "LocalAdmins"))
                    {
                        _userName = credential.UserName;
                        return true;
                    }

                    lastError = 740; // (0x2E4) ERROR_ELEVATION_REQUIRED, The requested operation requires elevation.
                }
            }

            return false;
        }

        MessageBox.Show(CheckRights() ? "User is Admin" : "User is not an Admin");
    }

    public string Error => String.Empty;

    public string this[string columnName]
    {
        get
        {
            if (columnName == nameof(TextWithError))
            {
                return @"This binding always returns an error";
            }

            return string.Empty;
        }
    }

    public KeyValuePair<int, string>[] Items { get; } = Enumerable.Range(0, 10)
        .Select(i => new KeyValuePair<int, string>(i, $"Item {i}")).ToArray();

    public ObservableCollection<KeyValuePair<int, string>> SelectedItemsAsObservableCollection { get; }

    public object[] SelectedItemsAsArray
    {
        get => SelectedItemsAsObservableCollection.OfType<object>().ToArray();
        set
        {
            if (SelectedItemsAsObservableCollection.OfType<object>().SequenceEqual(value))
                return;

            SelectedItemsAsObservableCollection.Clear();
            foreach (var item in value)
            {
                SelectedItemsAsObservableCollection.Add((KeyValuePair<int, string>)item);
            }
        }
    }

    public KeyValuePair<int, string>[] SelectedItemsAsTypedArray
    {
        get => SelectedItemsAsObservableCollection.ToArray();
        set
        {
            if (SelectedItemsAsObservableCollection.SequenceEqual(value))
                return;

            SelectedItemsAsObservableCollection.Clear();
            foreach (var item in value)
            {
                SelectedItemsAsObservableCollection.Add((KeyValuePair<int, string>)item);
            }
        }
    }

    private void SelectedItemsAsObservableCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (sender is ObservableCollection<KeyValuePair<int, string>> collection)
        {
            SelectedItemsAsArray = collection.OfType<object>().ToArray();
            SelectedItemsAsTypedArray = collection.ToArray();
        }
    }
}
