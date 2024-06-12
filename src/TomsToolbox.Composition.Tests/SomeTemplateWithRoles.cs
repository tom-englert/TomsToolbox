namespace TomsToolbox.Composition.Tests;

using System.Windows;

using TomsToolbox.Wpf.Composition.AttributedModel;

enum SomeRoles
{
    Role1,
    Role2
}

[DataTemplate(typeof(int), Role = SomeRoles.Role2)]
class SomeTemplateWithRoles1 : DependencyObject
{
}

[DataTemplate(typeof(int), Role = "Role2")]
class SomeTemplateWithRoles2 : DependencyObject
{
}
