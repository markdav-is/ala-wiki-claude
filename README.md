# AlaWiki

A mind-mapping extension for Azure DevOps wikis, built as an Oqtane Module in Blazor.

## Features

- **Git Integration**: Connects directly to ADO wiki Git repositories using LibGit2Sharp
- **Mind Map Visualization**: Visualize your wiki structure as an interactive mind map
- **Metadata Storage**: Uses parallel `.ala-wiki.json` files for visualization metadata (colors, icons, shapes, tags)
- **Real-time Editing**: Customize node appearance and commit changes back to the wiki repository
- **Cross-References**: Support for linking between wiki pages beyond the folder hierarchy

## Project Structure

```
src/
├── AlaWiki.sln              # Solution file
├── Client/                   # Blazor WebAssembly client
│   ├── Modules/AlaWiki/     # Razor components
│   │   ├── Index.razor      # Main mind map viewer
│   │   ├── Edit.razor       # Wiki connection management
│   │   ├── Settings.razor   # Display settings
│   │   ├── MindMapView.razor# Recursive node renderer
│   │   └── NodeDetails.razor# Node metadata editor
│   └── Services/            # Client-side API services
├── Server/                   # ASP.NET Core server
│   ├── Controllers/         # API endpoints
│   ├── Services/            # Git and mind map services
│   ├── Repository/          # Data access layer
│   ├── Manager/             # Module lifecycle manager
│   └── Migrations/          # SQL migration scripts
├── Shared/                   # Shared models and interfaces
│   ├── Models/              # Data models
│   └── Interfaces/          # Service contracts
└── Package/                  # NuGet/Oqtane packaging
```

## Metadata Format (Option 2)

Visualization metadata is stored in `.ala-wiki.json` files alongside wiki content:

```json
{
  "$schema": "https://ala-wiki.dev/schema/v1",
  "version": "1.0",
  "defaults": {
    "color": "#4a90d9",
    "collapsed": false
  },
  "pages": {
    "Overview": {
      "path": "Overview.md",
      "color": "#50c878",
      "icon": "oi oi-home",
      "shape": "ellipse",
      "priority": 1,
      "tags": ["important", "entry-point"]
    },
    "Architecture": {
      "path": "Architecture.md",
      "color": "#f5a623",
      "icon": "oi oi-layers",
      "collapsed": true,
      "links": ["Security/Overview.md"]
    }
  }
}
```

## Requirements

- .NET 10.0 SDK
- Oqtane Framework 10.0+
- SQL Server, SQLite, MySQL, or PostgreSQL

## Installation

### From NuGet Package

1. Build in Release mode to generate the `.oqp` package
2. In Oqtane admin, go to Module Management
3. Upload the `AlaWiki.x.x.x.oqp` file
4. Click Install

### From Source

```bash
cd src
dotnet build
```

## Configuration

1. Add the AlaWiki module to a page in Oqtane
2. Click "Configure Wiki Connection" or use Edit mode
3. Enter your Azure DevOps wiki Git URL:
   - Format: `https://dev.azure.com/{org}/{project}/_git/{project}.wiki`
4. Provide a Personal Access Token with Code (Read) permissions
5. Click Save and Sync

## Development

### Building

```bash
cd src
dotnet build
```

### Creating Package

```bash
cd src
dotnet build -c Release
```

The `.oqp` file will be in `Package/bin/Release/net10.0/`

## License

MIT License - see [LICENSE](LICENSE) for details.
