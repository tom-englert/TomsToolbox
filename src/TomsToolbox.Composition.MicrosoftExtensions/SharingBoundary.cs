namespace TomsToolbox.Composition.MicrosoftExtensions;

/// <summary>
/// Defines the supported sharing boundaries for service lifetimes.
/// </summary>
public static class SharingBoundary
{
    /// <summary>
    /// Represents a globally shared service lifetime.
    /// </summary>
    public const string? Global = null;

    /// <summary>
    /// Represents a scoped service lifetime.
    /// </summary>
    public const string Scoped = "Scoped";
}
