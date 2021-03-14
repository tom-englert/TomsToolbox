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

    using TomsToolbox.Essentials;

    /// <summary>
    /// Helper methods to ease dealing with the styles.
    /// </summary>
    public static class WpfStyles
    {
        [NotNull]
        private static readonly object _defaultStylesKey = new object();

        /// <summary>
        /// Gets the state of the ensure default styles feature.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if the ensure default styles feature is active.</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetEnsureDefaultStyles([NotNull] Window obj)
        {
            return obj.GetValue<bool>(EnsureDefaultStylesProperty);
        }
        /// <summary>
        /// Sets the state of the ensure default styles feature.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">If set to <c>true</c>, the system will ensure that the default styles are registered for the window.
        /// If the default styles are not registered in the resources of the <see cref="Application" />, the styles will be registered in the resources of the <see cref="Window" />.</param>
        public static void SetEnsureDefaultStyles([NotNull] Window obj, bool value)
        {
            obj.SetValue(EnsureDefaultStylesProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.Styles.WpfStyles.EnsureDefaultStyles"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty EnsureDefaultStylesProperty =
            DependencyProperty.RegisterAttached("EnsureDefaultStyles", typeof(bool), typeof(WpfStyles), new FrameworkPropertyMetadata(false, EnsureDefaultStyles_Changed));

        private static void EnsureDefaultStyles_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window window))
                throw new InvalidOperationException("EnsureDefaultStylesProperty must be attached to a Window only");

            if (!true.Equals(window.TryFindResource(_defaultStylesKey)))
            {
                RegisterDefaultStyles(window.Resources);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="IsCaptionVisibleProperty"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns><c>true</c> if the caption of the associated window should be visible</returns>
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsCaptionVisible([NotNull] Window window)
        {
            return (bool)(window.GetValue(IsCaptionVisibleProperty) ?? false);
        }
        /// <summary>
        /// Sets the value of the <see cref="IsCaptionVisibleProperty"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="value">if set to <c>true</c>, the caption of the associated window will be visible.</param>
        public static void SetIsCaptionVisible([NotNull] Window window, bool value)
        {
            window.SetValue(IsCaptionVisibleProperty, value);
        }
        /// <summary>
        /// A property to control the visibility of the caption area in a window. If the caption is e.g. included in a ribbon control, the caption should not be shown by the window.
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty IsCaptionVisibleProperty = DependencyProperty.RegisterAttached(
            "IsCaptionVisible", typeof(bool), typeof(WpfStyles), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Registers the default styles for the common controls and sets the default style for the Window.
        /// </summary>
        /// <param name="resources">The resource dictionary to which the default style definitions will be added, usually <see cref="Application.Resources" />.</param>
        /// <remarks>
        /// This method calls OverrideMetadata for the window style, and can only be called once per process!<para/>
        /// </remarks>
        public static void RegisterDefault([NotNull, ItemNotNull] this ResourceDictionary resources)
        {
            resources.RegisterDefaultStyles().RegisterDefaultWindowStyle();
        }

        /// <summary>
        /// Registers the default styles for the common controls.
        /// </summary>
        /// <param name="resources">
        /// The resource dictionary to which the default style definitions will be added, usually <see cref="Application.Resources" />.
        /// </param>
        [NotNull, ItemNotNull]
        public static ResourceDictionary RegisterDefaultStyles([NotNull, ItemNotNull] this ResourceDictionary resources)
        {
            resources.MergedDictionaries.Insert(0, GetDefaultStyles());

            return resources;
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
                .ExceptNullItems()
                .Select(key => helperWindow.FindResource(key) as Style)
                .ExceptNullItems()
                .ToArray();

            var mergedDictionary = new ResourceDictionary();

            foreach (var style in baseStyles)
            {
                mergedDictionary.Add(style.TargetType, style);
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

            var fieldInfos = resourceKeysType.GetFields();

            var styleFragments = fieldInfos.Select(GenerateDefaultStyleFragment)
                .Where(item => item != null);

            var xaml = string.Format(CultureInfo.InvariantCulture, "<ResourceDictionary>\n{0}\n</ResourceDictionary>", string.Join("\n", styleFragments));

            var xamlTypeMapper = new XamlTypeMapper(new string[0]);
            xamlTypeMapper.AddMappingProcessingInstruction("styles", resourceKeysType.Namespace, resourceKeysType.Assembly.FullName);

            var context = new ParserContext { XamlTypeMapper = xamlTypeMapper };

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("styles", "styles");

            var resourceDictionary = (ResourceDictionary)XamlReader.Parse(xaml, context);

            resourceDictionary.Add(_defaultStylesKey, true);

            return resourceDictionary;
        }

        private static string? GenerateDefaultStyleFragment([NotNull] FieldInfo field)
        {
            var attribute = field.GetCustomAttributes<DefaultStyleAttribute>(false).SingleOrDefault();
            if (attribute == null)
                return null;

            var fieldName = field.Name;
            var targetTypeName = attribute.TargetType.Name;
            var declaringTypeName = attribute.ResourceKeyDeclaringType?.Name;
            var resourceKeyPropertyName = attribute.ResourceKeyPropertyName;

            if (resourceKeyPropertyName == null || declaringTypeName == null)
            {
                return string.Format(CultureInfo.InvariantCulture, "<Style TargetType=\"{0}\" BasedOn=\"{{StaticResource {{x:Static styles:ResourceKeys.{1}}}}}\" />", targetTypeName, fieldName);
            }

            return string.Format(CultureInfo.InvariantCulture, "<Style TargetType=\"{0}\" BasedOn=\"{{StaticResource {{x:Static styles:ResourceKeys.{1}}}}}\" x:Key=\"{{x:Static {2}.{3}}}\" />", targetTypeName, fieldName, declaringTypeName, resourceKeyPropertyName);
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
        /// Gets or sets the alternate application icon identifier, i.e. the native resource id under which the application icon can be loaded.
        /// </summary>
        public static int? ApplicationIconId { get; set; }

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
            private const int IDI_APPLICATION = 0x7F00;

            [CanBeNull]
            public static ImageSource? GetApplicationIcon(int size)
            {
                try
                {
                    var hIcon = new[] { ApplicationIconId, IDI_APPLICATION, 1 }
                        .Where(i => i.HasValue)
                        .Select(i => i!.Value)
                        .Select(i => LoadImage(GetModuleHandle(null!), new IntPtr(i), 1, size, size, 0))
                        .FirstOrDefault(i => i != IntPtr.Zero);

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
        public DefaultStyleAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public DefaultStyleAttribute(Type targetType, Type resourceKeyDeclaringType, string resourceKeyPropertyName)
        {
            TargetType = targetType;
            ResourceKeyPropertyName = resourceKeyPropertyName;
            ResourceKeyDeclaringType = resourceKeyDeclaringType;
        }

        public Type TargetType { get; }

        public Type? ResourceKeyDeclaringType { get; }

        public string? ResourceKeyPropertyName { get; }
    }
}
