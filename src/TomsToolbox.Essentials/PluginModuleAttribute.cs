namespace TomsToolbox.Essentials;

using System;

/// <summary>
/// Denotes an assembly as a loadable module.<para/>
/// Categories may be assigned to implement dynamic loading of modules by category. 
/// </summary>
[CLSCompliant(false)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
[Serializable]
public sealed class PluginModuleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginModuleAttribute"/> class.
    /// </summary>
    /// <param name="categories">The categories.</param>
    public PluginModuleAttribute(params string[] categories)
    {
        Categories = categories;
    }

    /// <summary>
    /// Gets the categories for this module.
    /// </summary>
    public string[] Categories
    {
        get;
    }
}