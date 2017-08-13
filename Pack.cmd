CALL :PACK Core
CALL :PACK Desktop
CALL :PACK ObservableCollections
CALL :PACK Wpf
CALL :PACK Wpf.Styles
PAUSE
GOTO :EOF

:PACK
"%ProgramFiles(x86)%\Nuget\nuget" pack TomsToolbox.%1\TomsToolbox.%1.csproj -IncludeReferencedProjects
GOTO :EOF