using System.Text.Json;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Shared;
using AlaWiki.Shared.Models;
using AlaWiki.Server.Repository;

namespace AlaWiki.Server.Manager;

public class AlaWikiManager : IInstallable, IPortable
{
    private readonly ISqlRepository _sql;
    private readonly IWikiConnectionRepository _repository;

    public AlaWikiManager(ISqlRepository sql, IWikiConnectionRepository repository)
    {
        _sql = sql;
        _repository = repository;
    }

    public bool Install(Tenant tenant, string version)
    {
        return _sql.ExecuteScript(tenant, GetType().Assembly, "AlaWiki." + version + ".sql");
    }

    public bool Uninstall(Tenant tenant)
    {
        return _sql.ExecuteScript(tenant, GetType().Assembly, "AlaWiki.Uninstall.sql");
    }

    public string ExportModule(Module module)
    {
        var connections = _repository.GetWikiConnections(module.ModuleId).ToList();

        // Remove sensitive data before export
        foreach (var connection in connections)
        {
            connection.PersonalAccessToken = null;
            connection.LocalPath = null;
        }

        return JsonSerializer.Serialize(connections);
    }

    public void ImportModule(Module module, string content, string version)
    {
        var connections = JsonSerializer.Deserialize<List<WikiConnection>>(content);

        if (connections != null)
        {
            foreach (var connection in connections)
            {
                connection.WikiConnectionId = 0;
                connection.ModuleId = module.ModuleId;
                connection.LocalPath = null;
                connection.LastSyncedOn = null;
                _repository.AddWikiConnection(connection);
            }
        }
    }
}
