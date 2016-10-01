SET VERSION=1.0.52.0

PUSHD "%~dp0Deploy"

CALL :PUSH Core
CALL :PUSH Desktop
CALL :PUSH ObservableCollections
CALL :PUSH Wpf
CALL :PUSH Wpf.Styles

PAUSE
GOTO :EOF

:PUSH
"%~dp0.nuget\nuget" push TomsToolbox.%1.%VERSION%.nupkg
GOTO :EOF