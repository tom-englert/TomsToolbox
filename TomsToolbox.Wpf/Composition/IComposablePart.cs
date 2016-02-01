namespace TomsToolbox.Wpf.Composition
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Interface to be implemented by all objects supporting visual composition.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "MEF requires an interface for export.")]
    public interface IComposablePart
    {
    }

    /// <summary>
    /// Interface to be implemented by all objects supporting visual composition and require a context.
    /// </summary>
    public interface IComposablePartWithContext : IComposablePart
    {
        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        object CompositionContext { get; set; }
    }

    /// <summary>
    /// Interface to be implemented by all objects supporting visual composition and provide individual objects per context.
    /// </summary>
    public interface IComposablePartFactory : IComposablePart
    {
        /// <summary>
        /// Gets the part for the specified context.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <returns>The part to be used in composition.</returns>
        object GetPart(object compositionContext);
    }
}
