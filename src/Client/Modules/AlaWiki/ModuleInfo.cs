using Oqtane.Models;
using Oqtane.Modules;

namespace AlaWiki.Client.Modules.AlaWiki;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new()
    {
        Name = "AlaWiki",
        Description = "Mind-mapping extension for Azure DevOps wikis",
        Version = "1.0.0",
        ServerManagerType = "AlaWiki.Server.Manager.AlaWikiManager, AlaWiki.Server",
        ReleaseVersions = "1.0.0",
        Dependencies = "AlaWiki.Shared",
        PackageName = "AlaWiki",
        Categories = "Content,Visualization"
    };
}
