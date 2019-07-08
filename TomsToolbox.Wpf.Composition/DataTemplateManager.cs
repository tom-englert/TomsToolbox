namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Access methods for composite data template exports.
    /// </summary>
    public static class DataTemplateManager
    {
        /// <summary>
        /// A comparer to compare exports for dynamic data templates.
        /// </summary>
        [NotNull] private static readonly IEqualityComparer<IDataTemplateMetadata> ExportsComparer = new DelegateEqualityComparer<IDataTemplateMetadata>(Equals, GetHashCode);

        /// <summary>
        /// Gets the role of the view.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The role</returns>
        [CanBeNull]
        public static object GetRole([NotNull] DependencyObject obj)
        {
            return obj.GetValue(RoleProperty);
        }
        /// <summary>
        /// Sets the role of the view.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRole([NotNull] DependencyObject obj, [CanBeNull] object value)
        {
            obj.SetValue(RoleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.Composition.DataTemplateManager.Role"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Gets or sets the role associated with the view.
        /// </summary>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplateManager), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Creates dynamic data templates by looking up all MEF exports with the DataTemplateAttribute attribute,
        /// creating a <see cref="T:System.Windows.DataTemplate"/> resource dictionary entry for every export.
        /// </summary>
        /// <param name="exportProvider">The export provider to search for exports with the DataTemplateAttribute.</param>
        /// <returns>
        /// The resource dictionary containing the dynamic data templates. This is usually added to the merged dictionaries of your application's resources.
        /// </returns>
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary CreateDynamicDataTemplates([NotNull] IExportProvider exportProvider)
        {
            var dataTemplateResources = new ResourceDictionary();

            var exportMetaData = exportProvider.GetDataTemplateExportsMetadata();

            foreach (var item in exportMetaData)
            {
                var viewModel = item.ViewModel;
                var role = item.Role;

                var template = CreateTemplate(viewModel, role);

                dataTemplateResources.Add(CreateKey(viewModel, role), template);
            }

            return dataTemplateResources;
        }

        [CanBeNull]
        private static DataTemplate CreateTemplate([NotNull] Type viewModelType, [CanBeNull] object role)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type viewModel:{0}}}\"><toms:ComposableContentControl {1}/></DataTemplate>";
            var roleParameter = role == null ? string.Empty : string.Format(CultureInfo.InvariantCulture, "Role=\"{0}\"", role);
            var xaml = string.Format(CultureInfo.InvariantCulture, xamlTemplate, viewModelType.Name, roleParameter);

            var context = new ParserContext();
            var contentType = typeof(ComposableContentControl);

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            // ReSharper disable AssignNullToNotNullAttribute
            context.XamlTypeMapper.AddMappingProcessingInstruction("viewModel", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("toms", contentType.Namespace, contentType.Assembly.FullName);
            // ReSharper restore AssignNullToNotNullAttribute

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("viewModel", "viewModel");
            context.XmlnsDictionary.Add("toms", "toms");

            return (DataTemplate)XamlReader.Parse(xaml, context);
        }

        /// <summary>
        /// Creates the template key.
        /// </summary>
        /// <param name="dataType">The type for which the template is used to display items.</param>
        /// <param name="role">The optional role.</param>
        /// <returns>The key for the specified parameters.</returns>
        [NotNull]
        public static TemplateKey CreateKey([NotNull] Type dataType, [CanBeNull] object role)
        {
            if (role != null)
                return new RoleBasedDataTemplateKey(dataType, role);

            return new DataTemplateKey(dataType);
        }

        /// <summary>
        /// Gets the view for the specified view model.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="role">The role.</param>
        /// <returns>The view</returns>
        [CanBeNull]
        internal static DependencyObject GetDataTemplateView([NotNull] this IExportProvider exportProvider, [NotNull] Type viewModel, [CanBeNull] object role)
        {
            return exportProvider.GetExports(typeof(DependencyObject), XamlExtensions.DataTemplate.ContractName)
                .Where(item => item.IsViewModelForType(viewModel, role))
                .Reverse()  // if multiple exports exist, use the top one, e.g. s.o. wants to override in a special layout module.
                // .Select(AssertCorrectCreationPolicy)
                .Select(item => item.Value)
                .OfType<DependencyObject>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets all the meta data for the exports.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <returns>The meta data of all exports.</returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<IDataTemplateMetadata> GetDataTemplateExportsMetadata([NotNull] this IExportProvider exportProvider)
        {
            return exportProvider
                .GetExports<IDataTemplateMetadata>(typeof(DependencyObject), XamlExtensions.DataTemplate.ContractName, item => new DataTemplateMetadata(item))
                .Select(item => item.Metadata)
                // .Select(AssertCorrectCreationPolicy)
                .Distinct(ExportsComparer);
        }

        private static bool Equals([NotNull] IDataTemplateMetadata left, [NotNull] IDataTemplateMetadata right)
        {
            return (left.ViewModel == right.ViewModel) && RoleEquals(left.Role, right.Role);
        }

        private static int GetHashCode([NotNull] IDataTemplateMetadata metadata)
        {
            return metadata.ViewModel.GetHashCode() + (metadata.Role ?? 0).GetHashCode();
        }

        private static bool IsViewModelForType([CanBeNull] this ILazy<object> item, [CanBeNull] Type viewModel, [CanBeNull] object role)
        {
            var metadata = new DataTemplateMetadata(item?.Metadata);

            return (metadata.ViewModel == viewModel) && RoleEquals(metadata.Role, role);
        }

        /*
        [CanBeNull]
        private static IDataTemplateMetadata GetMetadataView([CanBeNull] ILazy<object, object> item)
        {
            return item?.Metadata is IDictionary<string, object> metadataDictionary ? AttributedModelServices.GetMetadataView<IDataTemplateMetadata>(metadataDictionary) : null;
        }

        [NotNull]
        private static ILazy<object, object> AssertCorrectCreationPolicy([NotNull] ILazy<object, object> export)
        {
            // Ensure views are created non-shared!

            // ReSharper disable once AssignNullToNotNullAttribute
            if ((export.Metadata is IDictionary<string, object> metadata) && metadata.TryGetValue(typeof(CreationPolicy).FullName, out var value) && CreationPolicy.NonShared.Equals(value))
                return export;

            var target = export.Value;
            var typeName = target?.GetType().Name ?? "<null>";
            var message = "Creation policy of views should be CreationPolicy.NonShared: " + typeName;

            Trace.TraceError(message);

            return export;
        }
        */

        /// <summary>
        /// Compares two roles.
        /// </summary>
        /// <param name="left">The left role.</param>
        /// <param name="right">The right role.</param>
        /// <returns>True it both objects are equal.</returns>
        public static bool RoleEquals([CanBeNull] object left, [CanBeNull] object right)
        {
            if (left == null)
                return right == null;

            if (right == null)
                return false;

            return left.ToString() == right.ToString();
        }
    }
}