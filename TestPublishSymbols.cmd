SET VERSION=1.0.4.0

PUSHD %~dp0\Deploy

REM CALL :PUSH Core
REM CALL :PUSH Desktop
REM CALL :PUSH ObservableCollections
CALL :PUSH Wpf

PAUSE
GOTO :EOF

:PUSH
nuget push TomsToolbox.%1.%VERSION%.symbols.nupkg
GOTO :EOF