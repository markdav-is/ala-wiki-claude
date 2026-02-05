using Microsoft.Extensions.DependencyInjection;
using Oqtane.Services;
using AlaWiki.Module.MindMap.Client.Services;
using AlaWiki.Module.MindMap.Shared.Interfaces;

namespace AlaWiki.Module.MindMap.Client.Startup;

public class ClientStartup : IClientStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IWikiConnectionService, WikiConnectionService>();
        services.AddScoped<IMindMapService, MindMapService>();
    }
}
