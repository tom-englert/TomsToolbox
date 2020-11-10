# Toms Toolbox

Toms Toolbox is a collection of DotNet assemblies containing various tools and extensions to simplify every days DotNet development.

This documentation provides an overview of the library, and how it can help to solve various problems and improve your code.

A detailed documentation of the full API is available [here](https://tom-englert.github.io/TomsToolbox/)

## TomsToolbox.Essentials 

This library contains methods to enhance code based on `netstandard2.0` objects. It has extension methods for essential objects like attributes, collections, date-time operations, etc.

## TomsToolbox.Desktop

This library contains extensions to objects only available when targeting Windows Desktop applications, like handles or native memory operations.

## TomsToolbox.Composition.*

This is a set of libraries providing an abstraction layer to DI-containers, so you can use dependency injection and leverage the annotations of `System.Composition.AttributedModel` without having to commit to a specific DI-container.

Reusable implementations for MEF1, MEF2, and Ninject are provided.

## TomsToolbox.ObservableCollections

Extensions, adapters and helpers for the `ObservableCollection` class, like observable versions of popular LINQ extensions, e.g. 
- `ObservableSelect()` (observable projection of items)
- `ObservableSelectMany()` (observable flattening)
- `ObservableCast()` (observable casting)
- `ObservableWhere()` (observable filtering)

## TomsToolbox.Wpf.*

Controls, converters, extensions, behaviors, triggers, styles and helpers for WPF applications.

Check out the [Sample Application](https://github.com/tom-englert/TomsToolbox/releases/latest) to get a glimpse of what you can do. 

### [TomsToolbox.Wpf.Styles](Topics/TomsToolbox.Wpf.Styles.md) 
Provides themeable modern styles for most standard WPF components by adding just one line of code to your project.