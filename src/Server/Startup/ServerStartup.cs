using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using AlaWiki.Module.MindMap.Server.Repository;
using AlaWiki.Module.MindMap.Server.Services;

namespace AlaWiki.Module.MindMap.Server.Startup;

public class ServerStartup : IServerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<MindMapContext>();
        services.AddTransient<IWikiConnectionRepository, WikiConnectionRepository>();
        services.AddTransient<IGitWikiService, GitWikiService>();
        services.AddTransient<IMindMapGenerator, MindMapGenerator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // No additional configuration needed
    }

    public void ConfigureMvc(IMvcBuilder mvcBuilder)
    {
        // No additional MVC configuration needed
    }
}
