using AlaWiki.Module.MindMap.Shared.Models;

namespace AlaWiki.Module.MindMap.Shared.Interfaces;

/// <summary>
/// Service interface for mind map generation and manipulation.
/// </summary>
public interface IMindMapService
{
    /// <summary>
    /// Generates a mind map from a wiki connection.
    /// </summary>
    Task<MindMap> GenerateMindMapAsync(int wikiConnectionId, MindMapSettings? settings = null);

    /// <summary>
    /// Gets the wiki page tree structure.
    /// </summary>
    Task<WikiPage> GetWikiTreeAsync(int wikiConnectionId);

    /// <summary>
    /// Gets a single wiki page with content.
    /// </summary>
    Task<WikiPage?> GetWikiPageAsync(int wikiConnectionId, string path);

    /// <summary>
    /// Updates metadata for a wiki page.
    /// </summary>
    Task UpdateNodeMetadataAsync(int wikiConnectionId, string path, NodeMetadata metadata);

    /// <summary>
    /// Commits metadata changes back to the wiki repository.
    /// </summary>
    Task CommitMetadataChangesAsync(int wikiConnectionId, string commitMessage);
}
