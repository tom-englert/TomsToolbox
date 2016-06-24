SET VERSION=1.0.45.0

PUSHD "%~dp0Deploy"

CALL :PUSH Core
CALL :PUSH Desktop
CALL :PUSH ObservableCollections
CALL :PUSH Wpf

PAUSE
GOTO :EOF

:PUSH
"%~dp0.nuget\nuget" push TomsToolbox.%1.%VERSION%.nupkg
GOTO :EOF