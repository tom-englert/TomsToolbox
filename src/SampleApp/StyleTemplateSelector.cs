namespace SampleApp;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

public class StyleTemplateSelector : DataTemplateSelector
{
    private const string StyleExplorer = "Style Explorer";

    private static readonly string[] _itemsSource = Enumerable.Range(1, 10).Select(i => StyleExplorer + " Item " + i).ToArray();

    public override DataTemplate? SelectTemplate(object item, DependencyObject? container)
    {
        if (!(item is Style style))
            return null;

        if (!(container is FrameworkElement element))
            return null;

        var targetType = style.TargetType;

        var dt = element.TryFindResource(targetType.Name) as DataTemplate;

        return dt ?? DynamicTemplate(targetType);
    }

    private static DataTemplate DynamicTemplate(Type? targetType)
    {
        var visualTree = new FrameworkElementFactory(targetType);

        if (typeof(HeaderedContentControl).IsAssignableFrom(targetType))
        {
            visualTree.SetValue(HeaderedContentControl.HeaderProperty, StyleExplorer);
        }

        if (typeof(HeaderedItemsControl).IsAssignableFrom(targetType))
        {
            visualTree.SetValue(HeaderedItemsControl.HeaderProperty, StyleExplorer);
        }

        if (typeof(ContentControl).IsAssignableFrom(targetType))
        {
            visualTree.SetValue(ContentControl.ContentProperty, StyleExplorer);
        }

        if (typeof(ItemsControl).IsAssignableFrom(targetType))
        {
            visualTree.SetValue(ItemsControl.ItemsSourceProperty, new Binding { Source = _itemsSource });
        }

        return new DataTemplate(targetType)
        {
            VisualTree = visualTree
        };
    }
}