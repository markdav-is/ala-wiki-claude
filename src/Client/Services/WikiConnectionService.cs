using System.Net.Http.Json;
using AlaWiki.Module.MindMap.Shared.Interfaces;
using AlaWiki.Module.MindMap.Shared.Models;
using Oqtane.Services;
using Oqtane.Shared;

namespace AlaWiki.Module.MindMap.Client.Services;

public class WikiConnectionService : ServiceBase, IWikiConnectionService
{
    public WikiConnectionService(HttpClient http, SiteState siteState) : base(http, siteState) { }

    private string ApiUrl => CreateApiUrl("WikiConnection");

    public async Task<List<WikiConnection>> GetWikiConnectionsAsync(int moduleId)
    {
        return await GetJsonAsync<List<WikiConnection>>($"{ApiUrl}?moduleid={moduleId}")
               ?? [];
    }

    public async Task<WikiConnection?> GetWikiConnectionAsync(int wikiConnectionId)
    {
        return await GetJsonAsync<WikiConnection>($"{ApiUrl}/{wikiConnectionId}");
    }

    public async Task<WikiConnection> AddWikiConnectionAsync(WikiConnection wikiConnection)
    {
        return await PostJsonAsync<WikiConnection>(ApiUrl, wikiConnection)
               ?? throw new Exception("Failed to add wiki connection");
    }

    public async Task<WikiConnection> UpdateWikiConnectionAsync(WikiConnection wikiConnection)
    {
        return await PutJsonAsync<WikiConnection>($"{ApiUrl}/{wikiConnection.WikiConnectionId}", wikiConnection)
               ?? throw new Exception("Failed to update wiki connection");
    }

    public async Task DeleteWikiConnectionAsync(int wikiConnectionId)
    {
        await DeleteAsync($"{ApiUrl}/{wikiConnectionId}");
    }

    public async Task<bool> TestConnectionAsync(int wikiConnectionId)
    {
        var response = await GetJsonAsync<TestConnectionResult>($"{ApiUrl}/{wikiConnectionId}/test");
        return response?.Success ?? false;
    }

    public async Task SyncWikiAsync(int wikiConnectionId)
    {
        await PostJsonAsync($"{ApiUrl}/{wikiConnectionId}/sync", new { });
    }

    private class TestConnectionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
