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
        private static readonly IEqualityComparer<IDataTemplateMetadata> ExportsComparer = new DelegateEqualityComparer<IDataTemplateMetadata>(Equals, GetHashCode);

        /// <summary>
        /// Gets the role of the view.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The role</returns>
        public static object GetRole(DependencyObject obj)
        {
            Contract.Requires(obj != null);

            return obj.GetValue(RoleProperty);
        }
        /// <summary>
        /// Sets the role of the view.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRole(DependencyObject obj, object value)
        {
            Contract.Requires(obj != null);

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
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplateManager), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Creates dynamic data templates by looking up all MEF exports with the <see cref="DataTemplateAttribute"/> attribute, 
        /// creating a <see cref="T:System.Windows.DataTemplate"/> resource dictionary entry for every export.
        /// </summary>
        /// <param name="exportProvider">The export provider to search for exports with the <see cref="DataTemplateAttribute"/>.</param>
        /// <returns>
        /// The resource dictionary containing the dynamic data templates. This is usually added to the merged dictionaries of your application's resources.
        /// </returns>
        public static ResourceDictionary CreateDynamicDataTemplates(ExportProvider exportProvider)
        {
            Contract.Requires(exportProvider != null);
            Contract.Ensures(Contract.Result<ResourceDictionary>() != null);

            var dataTemplateResources = new ResourceDictionary();

            var exportMetaData = exportProvider.GetDataTemplateExportsMetadata();

            foreach (var item in exportMetaData)
            {
                Contract.Assume(item != null);

                var viewModel = item.ViewModel;
                var role = item.Role;

                var template = CreateTemplate(viewModel, role);

                dataTemplateResources.Add(CreateKey(viewModel, role), template);
            }

            return dataTemplateResources;
        }

        private static DataTemplate CreateTemplate(Type viewModelType, object role)
        {
            Contract.Requires(viewModelType != null);

            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type viewModel:{0}}}\"><toms:ComposableContentControl {1}/></DataTemplate>";
            var roleParameter = role == null ? string.Empty : string.Format(CultureInfo.InvariantCulture, "Role=\"{0}\"", role);
            var xaml = string.Format(CultureInfo.InvariantCulture, xamlTemplate, viewModelType.Name, roleParameter);

            var context = new ParserContext();
            var contentType = typeof(ComposableContentControl);

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("viewModel", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("toms", contentType.Namespace, contentType.Assembly.FullName);

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
        public static TemplateKey CreateKey(Type dataType, object role)
        {
            Contract.Requires(dataType != null);
            Contract.Ensures(Contract.Result<TemplateKey>() != null);

            if (role != null)
                return new RoleBasedDataTemplateKey(dataType, role);

            return new DataTemplateKey(dataType);
        }

        /// <summary>
        /// Gets the dynamic data template exports.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <returns>All exports.</returns>
        private static IEnumerable<Lazy<DependencyObject, IDataTemplateMetadata>> GetDataTemplateExports(this ExportProvider exportProvider)
        {
            Contract.Requires(exportProvider != null);

            return exportProvider.GetExports<DependencyObject, IDataTemplateMetadata>(ContractName);
        }

        /// <summary>
        /// Gets the view for the specified view model.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="role">The role.</param>
        /// <returns>The view</returns>
        internal static DependencyObject GetDataTemplateView(this ExportProvider exportProvider, Type viewModel, object role)
        {
            Contract.Requires(exportProvider != null);
            Contract.Requires(viewModel != null);

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
        internal static IEnumerable<IDataTemplateMetadata> GetDataTemplateExportsMetadata(this ExportProvider exportProvider)
        {
            Contract.Requires(exportProvider != null);

            return exportProvider.GetExports(typeof(DependencyObject), null, ContractName)
                .Select(AssertCorrectCreationPolicy)
                .Select(GetMetadataView)
                .Where(item => item != null)
                .Distinct(ExportsComparer);
        }

        private static bool Equals(IDataTemplateMetadata left, IDataTemplateMetadata right)
        {
            Contract.Requires(left != null);
            Contract.Requires(right != null);

            return (left.ViewModel == right.ViewModel) && RoleEquals(left.Role, right.Role);
        }

        private static int GetHashCode(IDataTemplateMetadata metadata)
        {
            Contract.Requires(metadata != null);

            return metadata.ViewModel.GetHashCode() + (metadata.Role ?? 0).GetHashCode();
        }

        private static bool IsViewModelForType(this Lazy<object, object> item, Type viewModel, object role)
        {
            var metadata = GetMetadataView(item);

            if (metadata == null)
                return false;

            return (metadata.ViewModel == viewModel) && RoleEquals(metadata.Role, role);
        }

        private static IDataTemplateMetadata GetMetadataView(Lazy<object, object> item)
        {
            var metadataDictionary = item?.Metadata as IDictionary<string, object>;

            return metadataDictionary == null ? null : AttributedModelServices.GetMetadataView<IDataTemplateMetadata>(metadataDictionary);
        }

        private static Lazy<object, object> AssertCorrectCreationPolicy(Lazy<object, object> export)
        {
            Contract.Requires(export != null);

            // Ensure views are created non-shared!

            var metadata = export.Metadata as IDictionary<string, object>;
            object value;
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
        public static bool RoleEquals(object left, object right)
        {
            if (left == null)
                return right == null;

            if (right == null)
                return false;

            return left.ToString() == right.ToString();
        }
    }
}