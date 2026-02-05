using AlaWiki.Shared.Models;

namespace AlaWiki.Shared.Interfaces;

/// <summary>
/// Service interface for managing wiki connections.
/// </summary>
public interface IWikiConnectionService
{
    Task<List<WikiConnection>> GetWikiConnectionsAsync(int moduleId);
    Task<WikiConnection?> GetWikiConnectionAsync(int wikiConnectionId);
    Task<WikiConnection> AddWikiConnectionAsync(WikiConnection wikiConnection);
    Task<WikiConnection> UpdateWikiConnectionAsync(WikiConnection wikiConnection);
    Task DeleteWikiConnectionAsync(int wikiConnectionId);
    Task<bool> TestConnectionAsync(int wikiConnectionId);
    Task SyncWikiAsync(int wikiConnectionId);
}
