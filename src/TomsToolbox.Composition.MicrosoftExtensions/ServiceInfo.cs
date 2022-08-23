namespace TomsToolbox.Composition.MicrosoftExtensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Information about a service
/// </summary>
/// <param name="Descriptor">The service descriptor</param>
/// <param name="Metadata">The metadata entry for the service.</param>
public record ServiceInfo(ServiceDescriptor Descriptor, IMetadata? Metadata);
