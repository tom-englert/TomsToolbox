namespace TomsToolbox.Composition.Tests;

using TomsToolbox.Wpf.Composition.AttributedModel;

enum SomeRoles
{
    Role1,
    Role2
}

[DataTemplate(typeof(int), Role = SomeRoles.Role2)]
class SomeTemplateWithRoles1
{
}

[DataTemplate(typeof(int), Role = "Role2")]
class SomeTemplateWithRoles2
{
}