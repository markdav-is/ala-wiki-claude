@echo off
REM AlaWiki.Module.MindMap Debug Build Script
REM Copies module assemblies to Oqtane bin folder for development

set framework=%1
set modulename=%2

REM Update the path below to your local Oqtane installation
set oqtanepath=..\..\Oqtane.Server\bin\Debug\%framework%

if exist "%oqtanepath%" (
    echo Copying %modulename% assemblies to Oqtane...
    xcopy /y "..\Client\bin\Debug\%framework%\%modulename%.Client.Oqtane.dll" "%oqtanepath%"
    xcopy /y "..\Server\bin\Debug\%framework%\%modulename%.Server.Oqtane.dll" "%oqtanepath%"
    xcopy /y "..\Shared\bin\Debug\%framework%\%modulename%.Shared.Oqtane.dll" "%oqtanepath%"
    echo Done.
) else (
    echo Oqtane path not found: %oqtanepath%
    echo Update the oqtanepath variable in debug.cmd
)
