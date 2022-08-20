namespace SampleApp;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using PropertyChanged;

using TomsToolbox.Essentials;

[AddINotifyPropertyChangedInterface]
public class ResourceItem
{
    private readonly string _suffix;

    private ResourceItem(ComponentResourceKey key, object value, string suffix)
    {
        _suffix = suffix;
        Key = key;
        Value = value;
    }

    public ComponentResourceKey Key { get; }

    public object Value { get; }

    public string Description
    {
        get
        {
            var name = ToString();
            var value = Value;

            if (value is SolidColorBrush brush)
            {
                value = brush.Color;
            }

            if (value is Color color)
            {
                return name + "   " + GetDescription(color);
            }

            return name;
        }
    }

    private string GetDescription(Color color)
    {
        return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}   ({0}/{1}/{2})", color.R, color.G, color.B);
    }

    public override string ToString()
    {
        return ((string)Key.ResourceId).Replace(_suffix, string.Empty);
    }

    public static IList<ResourceItem> GetAll(Type type, string suffix)
    {
        return type
            .GetFields()
            .Where(field => field.Name.EndsWith(suffix))
            .Select(field => field.GetValue(null) as ComponentResourceKey)
            .ExceptNullItems()
            .Select(key => new ResourceItem(key, Application.Current.FindResource(key), suffix))
            .ToArray();
    }
}