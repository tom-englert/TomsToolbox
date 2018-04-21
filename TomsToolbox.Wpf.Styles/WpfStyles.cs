namespace TomsToolbox.Wpf.Styles
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper methods to ease dealing with the styles.
    /// </summary>
    public static class WpfStyles
    {
        /// <summary>
        /// Returns a resource dictionary with the default styles for the window and the common controls.
        /// </summary>
        /// <param name="helperWindow">A helper window used to access global theme resources.</param>
        /// <returns>
        /// A resource dictionary containing the default styles.
        /// </returns>
        [ContractVerification(false)]
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary Defaults([NotNull] Window helperWindow)
        {
            Contract.Requires(helperWindow != null);
            Contract.Ensures(Contract.Result<ResourceDictionary>() != null);

            var baseStyles = typeof(ResourceKeys)
                .GetFields()
                // ReSharper disable once AssignNullToNotNullAttribute
                .Where(field => field.GetCustomAttributes<DefaultStyleAttribute>(false).Any())
                .Select(field => field.GetValue(null) as ComponentResourceKey)
                .Where(key => key != null)
                .Select(key => helperWindow.FindResource(key) as Style)
                .Where(style => style != null)
                .ToArray();

            var mergedDictionary = new ResourceDictionary();

            foreach (var style in baseStyles)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                // ReSharper disable once PossibleNullReferenceException
                mergedDictionary.Add(style.TargetType, style);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            mergedDictionary.Add(MenuItem.SeparatorStyleKey, helperWindow.FindResource(ResourceKeys.MenuItemSeparatorStyle));

            // ReSharper disable once PossibleNullReferenceException
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(helperWindow.FindResource(ResourceKeys.WindowStyle)));

            return mergedDictionary;
        }

        /// <summary>
        /// Returns a resource dictionary with the default styles for the window and the common controls.
        /// </summary>
        /// <remarks>
        /// This method will created a temporary, hidden helper window.
        /// </remarks>
        /// <returns>
        /// A resource dictionary containing the default styles.
        /// </returns>
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary Defaults()
        {
            Contract.Ensures(Contract.Result<ResourceDictionary>() != null);

            var helperWindow = new Window();

            helperWindow.BeginInvoke(DispatcherPriority.Background, helperWindow.Close);

            return Defaults(helperWindow);
        }

        /// <summary>
        /// Gets the applications title from the <see cref="AssemblyTitleAttribute"/>.
        /// </summary>
        [CanBeNull]
        public static string ApplicationTitle
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    return string.Empty;

                return entryAssembly
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false)
                    .OfType<AssemblyTitleAttribute>()
                    .Select(attr => attr.Title)
                    .FirstOrDefault(title => !string.IsNullOrEmpty(title)) ?? entryAssembly.GetName().Name;
            }
        }

        /// <summary>
        /// Gets the small application icon (16x16) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource SmallApplicationIcon => NativeMethods.GetApplicationIcon(16);

        /// <summary>
        /// Gets the medium application icon (32x32) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource MediumApplicationIcon => NativeMethods.GetApplicationIcon(32);

        /// <summary>
        /// Gets the large application icon (48x48) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource LargeApplicationIcon => NativeMethods.GetApplicationIcon(48);

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = false)]
            private static extern IntPtr GetModuleHandle([CanBeNull] string lpModuleName);

            // ReSharper disable once InconsistentNaming
            private static readonly IntPtr IDI_APPLICATION = new IntPtr(0x7F00);

            [CanBeNull]
            public static ImageSource GetApplicationIcon(int size)
            {
                try
                {
                    var hIcon = LoadImage(GetModuleHandle(null), IDI_APPLICATION, 1, size, size, 0);

                    return hIcon == IntPtr.Zero ? null : Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }

    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class DefaultStyleAttribute : Attribute
    {
    }
}
