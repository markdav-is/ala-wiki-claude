using Microsoft.Extensions.DependencyInjection;
using Oqtane.Services;
using AlaWiki.Client.Services;
using AlaWiki.Shared.Interfaces;

namespace AlaWiki.Client;

public class Startup : IClientStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IWikiConnectionService, WikiConnectionService>();
        services.AddScoped<IMindMapService, MindMapService>();
    }
}
