namespace TomsToolbox.Wpf.Styles
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Markup;
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
        [NotNull, ItemCanBeNull]
        [Obsolete("Use GetDefaultStyles() and RegisterDefaultWindowStyle() instead.", true)]
        public static ResourceDictionary Defaults([NotNull] Window helperWindow)
        {
            var baseStyles = typeof(ResourceKeys)
                .GetFields()
                .Where(field => field.GetCustomAttributes<DefaultStyleAttribute>(false).Any())
                .Select(field => field.GetValue(null) as ComponentResourceKey)
                .Where(key => key != null)
                .Select(key => helperWindow.FindResource(key) as Style)
                .Where(style => style != null)
                .ToArray();

            var mergedDictionary = new ResourceDictionary();

            foreach (var style in baseStyles)
            {
                mergedDictionary.Add(style!.TargetType, style);
            }

            mergedDictionary.Add(MenuItem.SeparatorStyleKey, helperWindow.FindResource(ResourceKeys.MenuItemSeparatorStyle));

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
        [Obsolete("Use GetDefaultStyles() and RegisterDefaultWindowStyle() instead.", true)]
        public static ResourceDictionary Defaults()
        {
            var helperWindow = new Window();

            helperWindow.BeginInvoke(DispatcherPriority.Background, helperWindow.Close);

            return Defaults(helperWindow);
        }

        /// <summary>
        /// Returns a resource dictionary with the default styles for the window and the common controls.
        /// </summary>
        /// <returns>
        /// A resource dictionary containing the default styles.
        /// </returns>
        /// <remarks>
        /// Typical usage is:
        /// <code language="C#"><![CDATA[
        /// Resources.MergedDictionaries.Add(GetDefaultStyles());
        /// ]]></code></remarks>
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary GetDefaultStyles()
        {
            var resourceKeysType = typeof(ResourceKeys);

            var baseStyleKeys = resourceKeysType
                .GetFields()
                .Where(field => field.GetCustomAttributes<DefaultStyleAttribute>(false).Any());

            var styleFragments = baseStyleKeys.Select(GenerateDefaultStyleFragment).ToList();

            styleFragments.Add("<Style x:Key=\"{x:Static MenuItem.SeparatorStyleKey}\" TargetType=\"Separator\" BasedOn=\"{StaticResource {x:Static styles:ResourceKeys.MenuItemSeparatorStyle}}\" />");

            var xaml = string.Format(CultureInfo.InvariantCulture, "<ResourceDictionary>\n{0}\n</ResourceDictionary>", string.Join("\n", styleFragments));

            var xamlTypeMapper = new XamlTypeMapper(new string[0]);
            xamlTypeMapper.AddMappingProcessingInstruction("styles", resourceKeysType.Namespace ?? string.Empty, resourceKeysType.Assembly.FullName);

            var context = new ParserContext { XamlTypeMapper = xamlTypeMapper };

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("styles", "styles");

            return (ResourceDictionary)XamlReader.Parse(xaml, context);
        }

        /// <summary>
        /// Registers the default window style from the window style found in the specified <paramref name="resourceDictionary"/>.
        /// </summary>
        /// <param name="resourceDictionary">The resource dictionary containing the window style.</param>
        /// <returns>The <paramref name="resourceDictionary"/> to enable fluent notation.</returns>
        /// <remarks>
        /// Typical usage is:
        /// <code language="C#"><![CDATA[
        /// Resources.MergedDictionaries.Add(GetDefaultStyles().RegisterDefaultWindowStyle());
        /// ]]></code></remarks>
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary RegisterDefaultWindowStyle([NotNull] this ResourceDictionary resourceDictionary)
        {
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(resourceDictionary[typeof(Window)]));

            return resourceDictionary;
        }


        [NotNull]
        private static string GenerateDefaultStyleFragment([NotNull] FieldInfo field)
        {
            const string styleNameSuffix = "Style";
            var fieldName = field.Name;

            Debug.Assert(fieldName.EndsWith(styleNameSuffix));

            var typeName = fieldName.Substring(0, fieldName.Length - styleNameSuffix.Length);

            return string.Format(CultureInfo.InvariantCulture, "<Style TargetType=\"{0}\" BasedOn=\"{{StaticResource {{x:Static styles:ResourceKeys.{0}Style}}}}\" />", typeName);
        }

        /// <summary>
        /// Gets the applications title from the <see cref="AssemblyTitleAttribute"/>.
        /// </summary>
        [CanBeNull]
        public static string? ApplicationTitle
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
        /// Identifies the <see cref="P:TomsToolbox.Wpf.Styles.WpfStyles.WindowTitleDecorator"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Allows to add content to the title bar of the window.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty WindowTitleDecoratorProperty = DependencyProperty.RegisterAttached(
                    "WindowTitleDecorator", typeof(object), typeof(WpfStyles), new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// Sets the <see cref="P:TomsToolbox.Wpf.Styles.WpfStyles.WindowTitleDecorator" /></summary>
        /// <param name="element">The target element.</param>
        /// <param name="value">The value.</param>
        public static void SetWindowTitleDecorator(DependencyObject element, object value)
        {
            element.SetValue(WindowTitleDecoratorProperty, value);
        }
        /// <summary>
        /// Gets the <see cref="P:TomsToolbox.Wpf.Styles.WpfStyles.WindowTitleDecorator" /></summary>
        /// <param name="element">The element.</param>
        /// <returns>The title decorator</returns>
        public static object GetWindowTitleDecorator(DependencyObject element)
        {
            return element.GetValue(WindowTitleDecoratorProperty);
        }

        /// <summary>
        /// Gets the small application icon (16x16) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource? SmallApplicationIcon => NativeMethods.GetApplicationIcon(16);

        /// <summary>
        /// Gets the medium application icon (32x32) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource? MediumApplicationIcon => NativeMethods.GetApplicationIcon(32);

        /// <summary>
        /// Gets the large application icon (48x48) from the running executable.
        /// </summary>
        [CanBeNull]
        public static ImageSource? LargeApplicationIcon => NativeMethods.GetApplicationIcon(48);

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = false)]
            private static extern IntPtr GetModuleHandle([CanBeNull] string? lpModuleName);

            // ReSharper disable once InconsistentNaming
            private static readonly IntPtr IDI_APPLICATION = new IntPtr(0x7F00);

            [CanBeNull]
            public static ImageSource? GetApplicationIcon(int size)
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
