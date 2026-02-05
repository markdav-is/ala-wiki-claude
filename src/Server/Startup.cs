using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using AlaWiki.Server.Repository;
using AlaWiki.Server.Services;

namespace AlaWiki.Server;

public class Startup : IServerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<AlaWikiContext>();
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
