using Oqtane.Models;
using Oqtane.Modules;

namespace AlaWiki.Module.MindMap.Client.Modules;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new()
    {
        Name = "AlaWiki MindMap",
        Description = "Mind-mapping extension for Azure DevOps wikis",
        Version = "1.0.0",
        ServerManagerType = "AlaWiki.Module.MindMap.Server.Manager.MindMapManager, AlaWiki.Module.MindMap.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        Dependencies = "AlaWiki.Module.MindMap.Shared.Oqtane",
        PackageName = "AlaWiki.Module.MindMap",
        Categories = "Content,Visualization"
    };
}
