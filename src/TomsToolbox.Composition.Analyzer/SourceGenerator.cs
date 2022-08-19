﻿using Microsoft.CodeAnalysis;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context =>
        {
            context.AddSource("NonSharedAttribute", @"// <auto-generated/>
namespace System.Composition
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Marker attribute to be able to explicitly mark sharing boundaries of an exported type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional(""NEVER"")]
    internal class NonSharedAttribute : Attribute
    {
    }
}
");
        });
    }
}

#if false

namespace System.Composition
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Marker attribute to be able to explicitly mark sharing boundaries of an exported type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("NEVER")]
    internal class NonSharedAttribute : Attribute
    {

    }
}

#endif