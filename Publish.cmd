IF /I NOT "%1" == "true" GOTO :EOF

CALL :PUSH Core
CALL :PUSH Desktop
CALL :PUSH ObservableCollections
CALL :PUSH Wpf
CALL :PUSH Wpf.Styles
GOTO :EOF

:PUSH
"%~dp0.nuget\nuget" push TomsToolbox.%1.1.0.??.0.nupkg
GOTO :EOF