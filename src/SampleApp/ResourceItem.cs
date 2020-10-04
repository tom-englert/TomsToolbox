namespace AVL.Styx
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using JetBrains.Annotations;

    using PropertyChanged;

    [AddINotifyPropertyChangedInterface]
    public class ResourceItem
    {
        [NotNull] private readonly string _suffix;

        private ResourceItem([NotNull] ComponentResourceKey key, [NotNull] object value, [NotNull] string suffix)
        {
            _suffix = suffix;
            Key = key;
            Value = value;
        }

        [NotNull]
        public ComponentResourceKey Key { get; }

        [NotNull]
        public object Value { get; }

        [NotNull]
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

        [NotNull]
        private string GetDescription(Color color)
        {
            return string.Format(CultureInfo.InvariantCulture, "#{0:X2}{1:X2}{2:X2}   ({0}/{1}/{2})", color.R, color.G, color.B);
        }

        public override string ToString()
        {
            return ((string)Key.ResourceId).Replace(_suffix, string.Empty);
        }

        [NotNull]
        public static IList<ResourceItem> GetAll([NotNull] Type type, [NotNull] string suffix)
        {
            return type
                .GetFields()
                .Where(field => field.Name.EndsWith(suffix))
                .Select(field => field.GetValue(null) as ComponentResourceKey)
                .Where(key => key != null)
                .Select(key => new ResourceItem(key, Application.Current.FindResource(key), suffix))
                .ToArray();
        }
    }
}