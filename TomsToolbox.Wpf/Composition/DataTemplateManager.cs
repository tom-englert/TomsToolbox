namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Access methods for composite data template exports.
    /// </summary>
    public static class DataTemplateManager
    {
        /// <summary>
        /// The contract name used for export.
        /// </summary>
        internal const string ContractName = "{41cf1dfc-9c56-4e06-b177-703b4a24b0e1}";

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
        /// Creates dynamic data templates by looking up all MEF exports with the <see cref="DataTemplateAttribute"/> attribute, 
        /// creating a <see cref="T:System.Windows.DataTemplate"/> resource dictionary entry for every export.
        /// </summary>
        /// <param name="exportProvider">The export provider to search for exports with the <see cref="DataTemplateAttribute"/>.</param>
        /// <returns>
        /// The resource dictionary containing the dynamic data templates. This is usually added to the merged dictionaries of your application's resources.
        /// </returns>
        [NotNull, ItemCanBeNull]
        public static ResourceDictionary CreateDynamicDataTemplates([NotNull] ExportProvider exportProvider)
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
        internal static DependencyObject GetDataTemplateView([NotNull] this ExportProvider exportProvider, [NotNull] Type viewModel, [CanBeNull] object role)
        {

            return exportProvider.GetExports(typeof(DependencyObject), null, ContractName)
                .Where(item => item.IsViewModelForType(viewModel, role))
                .Reverse()  // if multiple exports exist, use the top one, e.g. s.o. wants to override in a special layout module.
                .Select(AssertCorrectCreationPolicy)
                .Where(item => item != null)
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
        private static IEnumerable<IDataTemplateMetadata> GetDataTemplateExportsMetadata([NotNull] this ExportProvider exportProvider)
        {

            return exportProvider.GetExports(typeof(DependencyObject), null, ContractName)
                .Select(AssertCorrectCreationPolicy)
                .Select(GetMetadataView)
                .Where(item => item != null)
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

        private static bool IsViewModelForType([CanBeNull] this Lazy<object, object> item, [CanBeNull] Type viewModel, [CanBeNull] object role)
        {
            var metadata = GetMetadataView(item);

            if (metadata == null)
                return false;

            return (metadata.ViewModel == viewModel) && RoleEquals(metadata.Role, role);
        }

        [CanBeNull]
        private static IDataTemplateMetadata GetMetadataView([CanBeNull] Lazy<object, object> item)
        {
            var metadataDictionary = item?.Metadata as IDictionary<string, object>;

            return metadataDictionary == null ? null : AttributedModelServices.GetMetadataView<IDataTemplateMetadata>(metadataDictionary);
        }

        [NotNull]
        private static Lazy<object, object> AssertCorrectCreationPolicy([NotNull] Lazy<object, object> export)
        {

            // Ensure views are created non-shared!

            var metadata = export.Metadata as IDictionary<string, object>;
            object value;
            // ReSharper disable once AssignNullToNotNullAttribute
            if ((metadata != null) && metadata.TryGetValue(typeof(CreationPolicy).FullName, out value) && CreationPolicy.NonShared.Equals(value))
                return export;

            var target = export.Value;
            var typeName = target?.GetType().Name ?? "<null>";
            var message = "Creation policy of views must be CreationPolicy.NonShared: " + typeName;

            Trace.TraceError(message);

            return export;
        }

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