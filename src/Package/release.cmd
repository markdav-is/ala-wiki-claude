@echo off
REM AlaWiki.Module.MindMap Release Build Script
REM Creates an Oqtane module package (.oqp)

set framework=%1
set modulename=%2
set version=1.0.0

echo Creating Oqtane package for %modulename% v%version%...

REM Create package directory
if exist "bin\Release\%framework%\package" rmdir /s /q "bin\Release\%framework%\package"
mkdir "bin\Release\%framework%\package"

REM Copy assemblies
copy "..\Client\bin\Release\%framework%\%modulename%.Client.Oqtane.dll" "bin\Release\%framework%\package\"
copy "..\Server\bin\Release\%framework%\%modulename%.Server.Oqtane.dll" "bin\Release\%framework%\package\"
copy "..\Shared\bin\Release\%framework%\%modulename%.Shared.Oqtane.dll" "bin\Release\%framework%\package\"

REM Copy LibGit2Sharp native binaries if present
if exist "..\Server\bin\Release\%framework%\lib" (
    xcopy /y /s "..\Server\bin\Release\%framework%\lib" "bin\Release\%framework%\package\lib\"
)

REM Copy static web assets if present
if exist "..\Server\bin\Release\%framework%\wwwroot" (
    xcopy /y /s "..\Server\bin\Release\%framework%\wwwroot" "bin\Release\%framework%\package\wwwroot\"
)

REM Create the .oqp package (ZIP file)
powershell -Command "Compress-Archive -Path 'bin\Release\%framework%\package\*' -DestinationPath 'bin\Release\%framework%\%modulename%.%version%.oqp' -Force"

echo Package created: bin\Release\%framework%\%modulename%.%version%.oqp
