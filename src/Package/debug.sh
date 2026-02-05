#!/bin/bash
# AlaWiki.Module.MindMap Debug Build Script
# Copies module assemblies to Oqtane bin folder for development

framework=$1
modulename=$2

# Update the path below to your local Oqtane installation
oqtanepath="../../Oqtane.Server/bin/Debug/$framework"

if [ -d "$oqtanepath" ]; then
    echo "Copying $modulename assemblies to Oqtane..."
    cp "../Client/bin/Debug/$framework/$modulename.Client.Oqtane.dll" "$oqtanepath/"
    cp "../Server/bin/Debug/$framework/$modulename.Server.Oqtane.dll" "$oqtanepath/"
    cp "../Shared/bin/Debug/$framework/$modulename.Shared.Oqtane.dll" "$oqtanepath/"
    echo "Done."
else
    echo "Oqtane path not found: $oqtanepath"
    echo "Update the oqtanepath variable in debug.sh"
fi
