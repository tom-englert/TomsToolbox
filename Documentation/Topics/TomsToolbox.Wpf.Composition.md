# TomsToolbox.Wpf.Composition

A Visual Composition Framework (VCF) to build modular WPF applications with loose component coupling via dependency injection (DI).

This framework is focussing full support of the model-first design:

### View-First vs. Model-First
In most documentations you will find this MVVM pattern:
```xml
<UserControl x:Class="SampleApp.MyView" ... >
    <UserControl.DataContext>
        <my:MyViewModel/>
    </UserControl.DataContext>
```
and the usage is instantiating the control as content of some content control (e.g. `Window`):
```xml
<Window ...>
    <my:MyView>
```

The view is created first, and the view creates it's view model.

However this does not align with the main paradigm of WPF, the data driven design. It's like to feed a `ListBox` control's `Items` with `ListBoxItems` - a thing that you never should do. You would rather feed the `ItemsSource` with a list of models, and define the representation of an item via a `DataTemplate`.

To implement the data driven design, we have to invert the above approach, and create the model first and let it fetch it's view dynamically:
```xml
<Window ...>
    <my:MyViewModel>
```
and somewhere else:
```xml
<ResourceDictionary>
    <DataTemplate DateType="my:MyViewModel">
        <my:MyView />
```

## Declaring and injecting models
To modularize our application we need to have loose coupling between containers and content. To achieve this the VCF provides the concept of regions. This concept was inspired by the [Prism](https://docs.microsoft.
com/en-us/previous-versions/msp-n-p/gg406140) framework, but has been designed to support the model-first approach and declarative DI like MEF.

The container simply declares a region, and the framework injects the model(s) that have registered with this region:

- Container:
```xml
<Window composition:VisualComposition.RegionId="{x:Static myApp:RegionId.Main}" />
```
- Content:
```c#
[VisualCompositionExport(RegionId.Main)]
public class MainViewModel 
{
    ...
}
```
The container control can be any `ContentControl` or `ItemsControl`. The view model(s) can be defined in any module/assembly of your application that has been registered with your DI container. The only information container and content share is the global region id constant.

## Defining data templates
So far we don't have defined a visual representation for the `MainViewModel`, and only it's type name would be displayed as content, so we have to define a `DataTemplate` for it.

Usually you would have to find a `ResourceDictionary` where you can add your `DataTemplate` to, and make the container aware of this resource dictionary at run time:
```xml
<ResourceDictionary>
    <DataTemplate DateType="my:MyViewModel">
        <my:MyView />
```
However this would be contra-dictionary to our modular approach. The container must be agnostic to the location where the visual is defined.

To achieve this the VCF provides an attribute to decorate any `(User)Control`, that registers it as a `DataTemplate`:

- Create a `UserControl` and define the visual representation of the view model

```xml
<UserControl x:Class="MyApp.MyView"
             d:DataContext="{d:DesignInstance my:MyViewModel}"
             ...
             >
```
- And declare it as a data template in the code behind:
```c#
[DataTemplate(typeof(MyViewModel))]
public partial class MyView : UserControl
{
    ...
```
Finally to collect all controls and make them available as data templates you have to add one line to your `App.xaml.cs` after you have populated your DI-container:
```c#
public sealed partial class App : Application
{
    private DIAdapter? _diAdapter;

    protected override void OnStartup([CanBeNull] StartupEventArgs e)
    {
        base.OnStartup(e);

        _diAdapter = new DIAdapter();
        var exportProvider = _diAdapter.Initialize();

        // Collect all types tagged with the [DataTemplate] attribute into a resource dictionary
        var dataTemplateDictionary = DataTemplateManager.CreateDynamicDataTemplates(exportProvider);
        // append the dictionary to the applications resources, so the data templates are available globally, no matter where they are defined.
        Resources.MergedDictionaries.Add(dataTemplateDictionary);


```
