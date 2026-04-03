Unreleased
- Add TomsToolbox.Avalonia module with core MVVM helpers for Avalonia.

2.22.2
- Fix #21: AdvancedScrollWheelBehavior sometimes scrolls to wrong target position (see https://github.com/icsharpcode/ILSpy/issues/3462)

2.22.1
- Avoid warning NU1608 for System.Composition.AttributedModel in projects using TomsToolbox.Wpf.Composition.AttributedModel

2.22.0
- Fix issues with weak event source when unsubscribing while raising events. (see https://github.com/icsharpcode/ILSpy/issues/3402)
- Merge pull request #20 from Hertzole: Fix not being able to change advanced scroll behavior after setting it

2.21.0
- Fix nullable annotations in WeakEventSource
- Add extension methods so we can use IServiceProvider with the same notation as IExportProvider.

2.20.0
- MultiSelectorExtensions support typed arrays as selection source.

2.19.1
- Stick with System.Composition.AttributedModel Version="[1.4.1]" for Netframework
 
2.19.0
- Support scoped service lifetime for Microsoft.Extensions.DependencyInjection

2.18.1
- Typed MetadataAdapter should be more robust and never throw.

2.18.0
- Composition: Support typed metadata for services

2.17.4:
- Update dependencies

2.17.3:
- Fine-tune AdvancedScrollWheelBehavior, remove dynamic scroll speed adaption

2.17.2:
- Fine-tune AdvancedScrollWheelBehavior, respect system settings.

2.17.1:
- AdvancedScrollWheelBehavior: Adjust scrolling speed to be more reasonable

2.17:
- Replace SmoothScrollingBehavior with AdvancedScrollWheelBehavior
- Fix scrolling behavior in AdvancedScrollWheelBehavior, improve support for touchpad scrolling
- Shortcut to attach AdvancedScrollWheelBehavior with or without animation

2.16:
- Add the smooth scrolling behavior to extend the scroll viewer mouse wheel behavior.

2.15:
- Fix handling of exported types with contract name: exports should not be reachable without contract name.

2.14.0:
- Remove ToDictionary extension method in Net8.0 ff to avoid conflicts with existing methods

2.13.0:
- Fix #18: TableHelper.ParseTable swallows empty lines

2.12.0
- Fix  #17: Trigger Actions not copied when using StyleBindings.Triggers

2.11.0

- Drop native net7, include net8

2.10.0

- Restore netstandard2.0

2.9.1

- Fix focusing of HighlightingTextBlock control

2.9.0

- Add new HighlightingTextBlock control

2.8.10

- Fix memory leak in Validation.ShowErrorInTooltip

2.8.9

- DependencyProperty change tracker should no make assumptions about threads when attaching or releasing event handlers

2.8.8

- Fix ObservableObject: Using RelayEventsOf may cause memory leak

2.8.7

- Fix #16: Layout Options context menu over maximize button is not visible on Win11

2.8.6

- Fix #16: Layout Options context menu over maximize button is not visible on Win11

2.8.5

- Revert upgrade of System.Composition.AttributedModel

2.8.4

- Fix #14: PresentationFrameworkExtensions.GetPhysicalPixelSize should not trow exceptions
- add net6.0 Support

2.8.3

- Microsoft.Extensions/ExportProviderAdapter service retrieval is compliant with ServiceProvider
 
2.8.2

- Microsoft.Extensions/ExportProviderAdapter => GetExportedValueOrDefault may fail if implementation is un-registered.

2.8.1

- Fix: Nested types are enumerated twice in MetadataReader

2.8.0

- Fix design flaw in MicrosoftExtensions ExportProviderAdapter

2.7.13

- Add GetServiceInfo methods

2.7.12

- Composition.Analyzer: also inspect constructors
- ExportProviderAdapter must be disposable

2.7.11

- Fix Composition.Analyzer nuget package
 
2.7.10

- Fix Composition.Analyzer nuget package

2.7.9

- Fix namespace of NonSharedAttribute

2.7.8

- Add analyzer to support writing DI-framework independent code.

2.7.7

- Fix UserAccountControl issues with long names/passwords

2.7.6

- Polyline: set canvas anchors on the child of the ViewportCanvas.

2.7.5

- ViewportCanvas: Fix minimum constraints

2.7.4

- ListBoxSelectAllBehavior: SelectAll() must not be called when ListBox is not loaded.

2.7.3

- Fix #11: ReplaceTextConverter may return wrong result

2.7.2

- Fix nullable signature of DelegateEqualityComparer
- Fix focus visual style for the CheckBox w/o content

2.7.1

- Fix possible stack overflow

2.7.0

- Use C#9, migrate Nullable annotations, drop JetBrains.Annotations for nullability

2.6.1

- support net5.0-windows and netcoreapp3.1 targets

2.5.5

- Fix restore button image

2.5.4

- Modernize WindowStyle

2.5.3

- Adjust checkable style of MenuItem

2.5.2

- CustomNonClientAreaBehavior: DwmExtendFrameIntoClientArea uses margin of 1 to not interfere with child windows (e.g. via WinForms interop)

2.5.1

- Fix TabControlStyle: design is more VS-like
- Fix MenuStyle: Add missing InputGestureText + design more VS-like

2.5.0

- Add attached property to CustomNonClientAreaBehavior to control DWM composition.

2.4.4

- Fix unexpected cursor in scroll bar thumb
- Align ListView and ListBox border styles

2.4.3

- Fix #10: missing RecognizesAccessKey in some buttons.

2.4.2

- WpfStyles: Support CheckBoxStyle in ToolBar, fine-tune styles.
 
2.4.1

- WpfStyles: Fix text-clipping in tool-bar; icon margin in Window.

2.4.0

- WpfStyles: add ListView and ToolBar

2.3.2

- Fix nullable annotations

2.3.1

- Fix nullable annotations

2.3

- Add nullable helpers

2.2.1

- MetaDataReader: Support Enums as attribute arguments

2.2

- Add ScrollBar, ScrollViewer and Expander style
- Fix some color usages to fully support theming of styles

2.1.1

- Fix Ninject ExportProvider for singletons with multiple exports.

2.1

- Introduce SemanticVersion class
- WPF window style: allow to have some additional content in the title bar.
- Add GitHubTasks to check for new releases.
- Improve performance of TextBoxVisibleWhiteSpaceDecorator.

2.0.2

- MetadataReader properly reads MEF1 creation policy.
- Fine-tune nullability annotations.
- Fix Ninject bindings for singletons with multiple exports.

2.0.1

- Refactored for target frameworks Net 4.5, NetStandard 2.0/Net Core 3.0
- Renamed TomsToolbox.Core to TomsToolbox.Essentials to avoid naming confusion with .NetCore
- Visual Composition: Decoupled DI container from usage. Now you can use the DI container of your choice.
- Migrated System.Windows.Interactivity to Microsoft.Xaml.Behaviors

1.0.75.0

- Add ThreadBoundTaskScheduler and BindingRelay class.

1.0.74.0

- Fix ComboBoxStyle: Cursor of editable combo box is not visible in dark mode.

1.0.73.0

- Added StyleBindings also for Column- and RowDefinitions
- VisualCompositionBehavior: Defer ExportProvider check to avoid false warnings; during unload export provider is released while the visual is still loaded.

1.0.72.0

- Extend tracing of VisualComposition

1.0.71.0

- Add trace capabilities to VisualComposition

1.0.70.0

- Correctly redraw window resize buttons when resizing window via mouse drag
- Fix in CustomNonClientAreaBehavior: When taskbar is set to auto-hide, it sometimes does not appear when window is maximized.
- Fix SampleApp click once prerequisites

1.0.69.0

- Fix: missing 4.5 extensions in TomsToolbox.Desktop

1.0.68.0

- Add an icon control that dynamically displays the image best fitting the actual size.
- Fix possible flickering of window when using the custom style.
- Provide an explicit .net45 version with all packages to fix issues with referenced packages (e.g. System.Windows.Interactivity).

1.0.67.0

- Use weak reference in ObservableObjectBase.RelayEvents to avoid memory leaks

1.0.66.0

- Add dependency property change tracker for framework elements that avoids accidental memory leaks

1.0.65.0

- Add XAML extensions for Button
- CustomNonClientAreaBehavior: Fix Maximized window handling
- Fix memory leaks in visual composition
- Add a TimerTrigger that continuously fires and avoids the memory leak of the Microsoft.Expression.Interactivity version.
- Add a WeakEventSource class
- Drop CodeContracts support

1.0.64.0

- Speedup TextBoxVisibleWhiteSpaceDecorator

1.0.63.0

- Rename private method SetRegionId, R# gets confused and shows false errors.
- Add missing nullability annotations

1.0.62.0

- Add ThemeResourceLoader support to window style

1.0.61.0

- Remove runtime dependency to Jetbrains.Annotations, replace with external annotations
- Add missing nullability annotations

1.0.60.0

- Fix data grid header styles
- Add possibility to set more than on group style via StyleBindings
- Add style for Window class
- Add ToDictionary from IEnumerable{KeyValuePair} extension method
- Improve DispatcherThrottle implementation
- Support NetStandard (TomsToolbox.Essentials)

1.0.59.0

- Add a VirtualizingDoubleClickPanel (same as DoubleClickPanel, but derived from VirtualizingPanel).

1.0.58.0

- Fix ItemsControlExtensions.DefaultItemCommand: Avoid duplicate executions on nested controls.

1.0.57.0

- Added R# [CanBeNull] on all unannotated parameters.

1.0.56.0

- CustomNonClientAreaBehavior: Schedule a redraw after WM_NCACTIVATE; if DWM composition is disabled, DefWindowProc(...WM_NCACTIVATE..) *does* some extra drawing on the window frame.

1.0.55.0

- Add GetItemContainers extension method for ItemsControl.
- InPlaceEdit: Support text alignment and begin edit on enter if focused.
- Add VisualAncestors extension method.
- Add a validation template for Validar.Fody with DataAnnotations.
- Added HashCode class.

1.0.54.0

- Remove reference to JetBrains.ExternalAnnotations.

1.0.53.0

- Added WPF Styles.
- Added R# [NotNull] annotations to complement Code Contracts.

1.0.52.0

- Extend CommandRoutingBehavior to track focus to update the active instance.

1.0.51.0

- Change resource keys to workaround https://connect.microsoft.com/VisualStudio/feedback/details/2993889/.
- Remove WeakReference&lt;T&gt; in .Net45.

1.0.50.0

- ObservableObject: Pass validation context to ValidationAttribute.

1.0.49.0

- Speed up SelectGroupOnGroupHeaderClickBehavior.

1.0.48.0

- VisualComposition runtime error handling.

1.0.47.0

- Add ListBoxSelectAllBehavior.

1.0.46.0

- CustomNonClientAreaBehavior: Handle SizeToContent.

1.0.45.0

- Add liveTrackingProperties parameter also to ObservableWhere extension.
- Use DWM to draw the glass frame in custom windows + minor fixes.
- Add generic dialog commands to build generic dialogs.

1.0.44.0

- Support live filtering in ObservableFilteredCollection/ObservableWhere.

1.0.43.0

- Fix performance issues of the ObservableCompositeCollection indexer.

1.0.42.0

- Add ImportBehavior.
- DataTemplate.Role also works with ContentPresenter.

1.0.41.0

- Refactor visual composition to not require the IComposablePart interface and allow lazy setting of the export provider.

1.0.40.0

- Extend ObservableObject to enable validation error tracking.
- Add additional classes.

1.0.39.0

- ObservableCompositeCollection: Property changed for Count property not always raised.

1.0.38.0

- Split ObservableObject to have a serializable base class.
- Focus not set when MultiSelectorExtensions selects single item via SourceSelection_CollectionChanged.

1.0.37.0

- Fix command routing behavior: Detach does not work if called after unloading.

1.0.36.0

- Role property should be of type object everywhere.

1.0.35.0

- Make attributes serializable.
- Define xmlns prefix.
- Fix missing parameters in AppDomainHelper.

1.0.34.0

- Add UnaryOperationConverter and AggregatingMultiValueConverter.
- Enhance DateTimeSource and UpdatePropertyAction.
- Fix BinaryOperationConverter for DateTime operations.

1.0.33.0

- Attributed validation support for ObservableObject.
- ClipoardHelper: Default to TEXT, as CSV is not Unicode.
- Enhance all converters (usage, error handling, tracing).

1.0.32.0

- UpdatePropertyAction: Stop updating the binding when the target has been unloaded.
- Added: AssemblyExtensions, HighResolutionTimer.
- Fix ClipboardHelper (broken parsing of quoted strings).

1.0.31.0

- Clipboard helper: proper handling for a single cell that contains only white space.

1.0.30.0

- Add clipboard helper to support table data exchange via clipboard.

1.0.29.0

- Fix raise condition in ThreadWithDispatcher.
- Visual composition behavior works with free threaded export provider.
- ItemsControlCompositionBehavior forces the selection of the first element after applying the content if the target is a selector.

1.0.28.0

- Fix/Rename ObservableFilteredCollection and add ObservableWhere extension.

1.0.27.0

- Improve ObjectToText converter.
- Add StringToObject converter.

1.0.26.0

- Fix typo in CommandRoutingBehavior.IsChecked property.

1.0.25.0

- Composite commands re-factored.

1.0.24.0

- Add Logical, Arithmetic, and Composite MultiValueConverters.

1.0.23.0

- Add KeyboardNavigation helper class.
- Add BinaryOperationConverter class.

1.0.22.0

- UpdatePropertiesAction => UpdatePropertyAction, improves usability; add usage sample.

1.0.21.0

- Extend CommandRoutingBehavior by CommandParameter.
- Add WindowButtonsBehavior for default button handling.

1.0.20.0

- Add DataTemplate helper.
- Add Interactivity style bindings.
- Add CustomNonClientAreaBehavior class.
- Add DateTimeSource class.
- Add DoubleClickPanel class.
- Add WindowsCommands class.
- AllowRecomposition in ImportExtension class.

1.0.19.0

- Improve import error messages.
- Add composite command handling.

1.0.18.0

- Add LegacyV2RuntimeActivationPolicy helper.
- Restructure composition framework, fix export provider locator issues and adapt sample application to use dynamic export provider locator.
- Add portable version of Core and .Net45 version of Desktop.

1.0.17.0

- Add Import markup extension.
- Simplify ComposableContentControl, improve loading behavior.

1.0.16.0

- Add FrameworkElementBehavior and UpdatePropertiesAction.
- Fix ComposableContentControl loading behavior.

1.0.15.0

- Add performant ToArray() for collections.
- Add HGlobal class for proper handling of unmanaged memory.

1.0.14.0

- Added optional change callback to ObservableObject.SetProperty.

1.0.13.0

- Fix alignment problem in SharedWidthHeaderedContentControl.
- Clean up/improve the sample application.
- MultiSelectorExtensions - no more updating if items control is about to be unloaded.

1.0.12.0

- Adapt DependencyObject usage and extensions after fixing CodeContract.

1.0.11.0

- Add VisualComposition framework.
- Add TextBoxVisibleWhiteSpaceDecorator.

1.0.10.0

- Preserve Jetbrains.Annotations attributes.

1.0.9.0

- Fix a bug in MultiSelectorExtensions.

1.0.8.0

- Add FilteredObservableCollection, XmlExtensions, and dependency property change tracking.

1.0.7.0

- Add new features.

1.0.6.0

- Upgrade JetBrains.Annotations.
- Add culture info helper extensions.

1.0.5.0

- Add type converter support and ObservablePropertyChangeTracker.

1.0.4.0

- Add map control and various behaviors.

1.0.3.0

- Add controls and converters.

1.0.2.0

- Simplified non-generic DelegateCommand (risk of recursive calls).

1.0.1.0

- Fixed NuGet packages: CodeContract libraries were added as reference.

1.0.0.0

- Initial version.
