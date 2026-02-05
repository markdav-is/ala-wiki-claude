#!/bin/bash
# AlaWiki.Module.MindMap Release Build Script
# Creates an Oqtane module package (.oqp)

framework=$1
modulename=$2
version="1.0.0"

echo "Creating Oqtane package for $modulename v$version..."

# Create package directory
rm -rf "bin/Release/$framework/package"
mkdir -p "bin/Release/$framework/package"

# Copy assemblies
cp "../Client/bin/Release/$framework/$modulename.Client.Oqtane.dll" "bin/Release/$framework/package/"
cp "../Server/bin/Release/$framework/$modulename.Server.Oqtane.dll" "bin/Release/$framework/package/"
cp "../Shared/bin/Release/$framework/$modulename.Shared.Oqtane.dll" "bin/Release/$framework/package/"

# Copy LibGit2Sharp native binaries if present
if [ -d "../Server/bin/Release/$framework/lib" ]; then
    cp -r "../Server/bin/Release/$framework/lib" "bin/Release/$framework/package/"
fi

# Copy static web assets if present
if [ -d "../Server/bin/Release/$framework/wwwroot" ]; then
    cp -r "../Server/bin/Release/$framework/wwwroot" "bin/Release/$framework/package/"
fi

# Create the .oqp package (ZIP file)
cd "bin/Release/$framework/package"
zip -r "../$modulename.$version.oqp" .
cd -

echo "Package created: bin/Release/$framework/$modulename.$version.oqp"
