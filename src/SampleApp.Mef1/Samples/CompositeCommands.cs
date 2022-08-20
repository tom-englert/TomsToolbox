namespace SampleApp.Mef1.Samples;

using System.ComponentModel.Composition;

using SampleApp.Mef1.Properties;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Composition.Mef;

[Export]
[VisualCompositionExport(RegionId.Menu)]
[LocalizedDisplayName(StringResourceKey.File)]
[Text(SubRegionIdKey, RegionId.FileSubMenu)]
[Sequence(1)]
public class FileCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.FileSubMenu, RegionId.ToolBar, Sequence = 1)]
[LocalizedDisplayName(StringResourceKey.Open)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/open.png")]
[Text(GroupNameKey, "File")]
public class OpenCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.FileSubMenu, RegionId.ToolBar, Sequence = 2)]
[LocalizedDisplayName(StringResourceKey.Close)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/close.png")]
[Text(GroupNameKey, "File")]
public class CloseCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.Menu, Sequence = 2)]
[LocalizedDisplayName(StringResourceKey.Edit)]
[Text(SubRegionIdKey, RegionId.EditSubMenu)]
public class EditCommand : CommandSourceFactory
{
}


[Export]
[VisualCompositionExport(RegionId.ContextMenu, RegionId.EditSubMenu, RegionId.ToolBar, Sequence = 14)]
[LocalizedDisplayName(StringResourceKey.Delete)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/delete.png")]
[Text(GroupNameKey, "Edit")]
public class DeleteCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.ContextMenu, RegionId.EditSubMenu, RegionId.ToolBar, Sequence = 11)]
[LocalizedDisplayName(StringResourceKey.Cut)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/cut.png")]
[Text(GroupNameKey, "Edit")]
public class CutCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.ContextMenu, RegionId.EditSubMenu, RegionId.ToolBar, Sequence = 12)]
[LocalizedDisplayName(StringResourceKey.Copy)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/copy.png")]
[Text(GroupNameKey, "Edit")]
public class CopyCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.ContextMenu, RegionId.EditSubMenu, RegionId.ToolBar, Sequence = 13)]
[LocalizedDisplayName(StringResourceKey.Paste)]
[Text(IconUriKey, "pack://application:,,,/SampleApp.Mef1;component/Assets/paste.png")]
[Text(GroupNameKey, "Edit")]
public class PasteCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.ContextMenu, RegionId.Menu, Sequence = 20)]
[LocalizedDisplayName(StringResourceKey.FindGroup)]
[Text(GroupNameKey, "Find")]
[Text(SubRegionIdKey, RegionId.FindSubMenu)]
public class FindCommandGroup : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.FindSubMenu, Sequence = 30)]
[LocalizedDisplayName(StringResourceKey.Find)]
[Text(GroupNameKey, "Find")]
public class FindCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.FindSubMenu, Sequence = 31)]
[LocalizedDisplayName(StringResourceKey.Replace)]
[Text(GroupNameKey, "Find")]
public class ReplaceCommand : CommandSourceFactory
{
}

[Export]
[VisualCompositionExport(RegionId.FindSubMenu, Sequence = 99)]
[LocalizedDisplayName(StringResourceKey.Recursive)]
[LocalizedDescription(StringResourceKey.RecursiveToolTip)]
[Text(SubRegionIdKey, RegionId.ContextMenu)]
[Text(GroupNameKey, "Test")]
public class RecursionCommand : CommandSourceFactory
{
}