namespace TomsToolbox.Wpf.Composition;

using System;
using System.Linq;
using Avalonia;

using TomsToolbox.Composition;

/// <summary>
/// Provides location service for the export provider to the Avalonia UI tree.
/// </summary>
public static class ExportProviderLocator
{
    // Dummy owner class required for Avalonia RegisterAttached (can't use static class as TOwner)
    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class Owner : AvaloniaObject { }

    private static IExportProvider? _defaultExportProvider;

    /// <summary>
    /// Registers the specified export provider.
    /// </summary>
    /// <param name="exportProvider">The export provider.</param>
    public static void Register(IExportProvider exportProvider)
    {
        // Avalonia doesn't support OverrideMetadata like WPF.
        // Store the global default in a static field for fallback.
        _defaultExportProvider = exportProvider;
    }

    /// <summary>
    /// Gets the active export provider for the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// The exports provider.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">Export provider must be registered in the visual tree</exception>
    public static IExportProvider GetExportProvider(this AvaloniaObject obj)
    {
        // Fall back to the default registered provider
        return (obj.GetValue(ExportProviderProperty) ?? _defaultExportProvider) ?? throw new InvalidOperationException(GetMissingExportProviderMessage(obj));
    }
    /// <summary>
    /// Gets the active export provider for the specified object, or <c>null</c> if no export provider is registered.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// The exports provider.
    /// </returns>
    public static IExportProvider? TryGetExportProvider(this AvaloniaObject obj)
    {
        return obj.GetValue(ExportProviderProperty) ?? _defaultExportProvider;
    }
    /// <summary>
    /// Sets the export provider.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetExportProvider(this AvaloniaObject obj, IExportProvider? value)
    {
        obj.SetValue(ExportProviderProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wpf.Composition.ExportProviderLocator.ExportProvider"/> attached property.
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// Makes the export provider available in the Avalonia visual tree.
    /// </summary>
    /// </AttachedPropertyComments>
    public static readonly AttachedProperty<IExportProvider?> ExportProviderProperty =
        AvaloniaProperty.RegisterAttached<Owner, AvaloniaObject, IExportProvider?>("ExportProvider", inherits: true);

    /// <summary>
    /// Gets the message to show when an export provider could not be located in the visual tree.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The message.</returns>
    public static string GetMissingExportProviderMessage(this AvaloniaObject obj)
    {
        return "Export provider must be registered in the visual tree " + string.Join("/", obj.AncestorsAndSelf().Reverse().Select(o => o?.GetType().Name));
    }
}
