namespace TomsToolbox.Wpf.Composition;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using TomsToolbox.Composition;
using TomsToolbox.Essentials;

using HashCode = Essentials.HashCode;

/// <summary>
/// Access methods for composite data template exports.
/// </summary>
public static class DataTemplateManager
{
    /// <summary>
    /// A comparer to compare exports for dynamic data templates.
    /// </summary>
    private static readonly IEqualityComparer<IDataTemplateMetadata> ExportsComparer = new DelegateEqualityComparer<IDataTemplateMetadata>(Equals, GetHashCode);

    /// <summary>
    /// Gets the role of the view.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The role</returns>
    public static object? GetRole(DependencyObject obj)
    {
        return obj.GetValue(RoleProperty);
    }
    /// <summary>
    /// Sets the role of the view.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetRole(DependencyObject obj, object? value)
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
    public static readonly DependencyProperty RoleProperty =
        DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplateManager), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Creates dynamic data templates by looking up all MEF exports with the DataTemplateAttribute attribute,
    /// creating a <see cref="T:System.Windows.DataTemplate"/> resource dictionary entry for every export.
    /// </summary>
    /// <param name="exportProvider">The export provider to search for exports with the DataTemplateAttribute.</param>
    /// <returns>
    /// The resource dictionary containing the dynamic data templates. This is usually added to the merged dictionaries of your application's resources.
    /// </returns>
    public static ResourceDictionary CreateDynamicDataTemplates(IExportProvider exportProvider)
    {
        var dataTemplateResources = new ResourceDictionary();

        var exportMetaData = exportProvider.GetDataTemplateExportsMetadata();

        foreach (var item in exportMetaData)
        {
            var viewModel = item.DataType;
            var role = item.Role;

            if (viewModel == null)
                continue;

            var template = CreateTemplate(viewModel, role);

            dataTemplateResources.Add(CreateKey(viewModel, role), template);
        }

        return dataTemplateResources;
    }

    private static DataTemplate CreateTemplate(Type viewModelType, object? role)
    {
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
    public static TemplateKey CreateKey(Type dataType, object? role)
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
    internal static DependencyObject? GetDataTemplateView(this IExportProvider exportProvider, Type viewModel, object? role)
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
    private static IEnumerable<IDataTemplateMetadata> GetDataTemplateExportsMetadata(this IExportProvider exportProvider)
    {
        return exportProvider
            .GetExports<IDataTemplateMetadata>(typeof(DependencyObject), XamlExtensions.DataTemplate.ContractName, item => new DataTemplateMetadata(item))
            .Select(item => item.Metadata)
            .ExceptNullItems()
            .Distinct(ExportsComparer);
    }

    private static bool Equals(IDataTemplateMetadata? left, IDataTemplateMetadata? right)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;
        return (left.DataType == right.DataType) && RoleEquals(left.Role, right.Role);
    }

    private static int GetHashCode(IDataTemplateMetadata? metadata)
    {
        if (metadata is null)
            return 0;

        return HashCode.Aggregate(metadata.DataType?.GetHashCode() ?? 0, (metadata.Role ?? 0).GetHashCode());
    }

    private static bool IsViewModelForType(this IExport<object> item, Type viewModel, object? role)
    {
        var itemMetadata = item.Metadata;
        if (itemMetadata == null)
            return false;

        var templateMetadata = new DataTemplateMetadata(itemMetadata);

        return (templateMetadata.DataType == viewModel) && RoleEquals(templateMetadata.Role, role);
    }

    /// <summary>
    /// Compares two roles.
    /// </summary>
    /// <param name="left">The left role.</param>
    /// <param name="right">The right role.</param>
    /// <returns>True it both objects are equal.</returns>
    public static bool RoleEquals(object? left, object? right)
    {
        if (left == null)
            return right == null;

        if (right == null)
            return false;

        return left.ToString() == right.ToString();
    }
}