namespace SampleApp.Themes
{
    using System.Windows;

    using JetBrains.Annotations;

    public class ResourceKeys
    {
        [NotNull] public static readonly ResourceKey ThemedImageStyle = new ComponentResourceKey(typeof(ResourceKeys), "ThemedImageStyle");
    }
}
