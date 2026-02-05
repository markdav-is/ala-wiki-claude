using System.Net.Http.Json;
using AlaWiki.Module.MindMap.Shared.Interfaces;
using AlaWiki.Module.MindMap.Shared.Models;
using Oqtane.Services;
using Oqtane.Shared;

namespace AlaWiki.Module.MindMap.Client.Services;

public class MindMapService : ServiceBase, IMindMapService
{
    public MindMapService(HttpClient http, SiteState siteState) : base(http, siteState) { }

    private string ApiUrl => CreateApiUrl("MindMap");

    public async Task<MindMap> GenerateMindMapAsync(int wikiConnectionId, MindMapSettings? settings = null)
    {
        var url = $"{ApiUrl}/generate/{wikiConnectionId}";
        if (settings != null)
        {
            return await PostJsonAsync<MindMap>(url, settings)
                   ?? throw new Exception("Failed to generate mind map");
        }
        return await GetJsonAsync<MindMap>(url)
               ?? throw new Exception("Failed to generate mind map");
    }

    public async Task<WikiPage> GetWikiTreeAsync(int wikiConnectionId)
    {
        return await GetJsonAsync<WikiPage>($"{ApiUrl}/tree/{wikiConnectionId}")
               ?? throw new Exception("Failed to get wiki tree");
    }

    public async Task<WikiPage?> GetWikiPageAsync(int wikiConnectionId, string path)
    {
        var encodedPath = Uri.EscapeDataString(path);
        return await GetJsonAsync<WikiPage>($"{ApiUrl}/page/{wikiConnectionId}?path={encodedPath}");
    }

    public async Task UpdateNodeMetadataAsync(int wikiConnectionId, string path, NodeMetadata metadata)
    {
        var encodedPath = Uri.EscapeDataString(path);
        await PutJsonAsync($"{ApiUrl}/metadata/{wikiConnectionId}?path={encodedPath}", metadata);
    }

    public async Task CommitMetadataChangesAsync(int wikiConnectionId, string commitMessage)
    {
        await PostJsonAsync($"{ApiUrl}/commit/{wikiConnectionId}", new { Message = commitMessage });
    }
}
