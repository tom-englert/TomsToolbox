namespace SampleApp.Samples;

using System;
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

    public override string ToString()
    {
        return "Misc.";
    }

    public DateTime OperationStarted { get; set; } = DateTime.Now;

    public TimeSpan MinimumDuration { get; set; } = TimeSpan.FromMinutes(0.2);

    public ICommand ItemsControlDefaultCommand => new DelegateCommand<string>(item => MessageBox.Show(item + " clicked."));

    public ICommand GCCollectCommand => new DelegateCommand(GC.Collect);

    public ICommand CheckForAdminRightsCommand => new DelegateCommand(CheckForAdminRights);
    
    public string? TextWithError { get; set;  }

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
}
