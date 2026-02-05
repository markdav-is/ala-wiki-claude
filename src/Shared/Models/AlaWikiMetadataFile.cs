using System.Text.Json.Serialization;

namespace AlaWiki.Module.MindMap.Shared.Models;

/// <summary>
/// Represents the .ala-wiki.json file structure (Option 2).
/// One file per folder, containing metadata for all pages in that folder.
/// </summary>
public class AlaWikiMetadataFile
{
    /// <summary>
    /// Schema version for forward compatibility.
    /// </summary>
    [JsonPropertyName("$schema")]
    public string Schema { get; set; } = "https://ala-wiki.dev/schema/v1";

    /// <summary>
    /// Version of this metadata file.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Default settings applied to all nodes in this folder.
    /// </summary>
    [JsonPropertyName("defaults")]
    public FolderDefaults? Defaults { get; set; }

    /// <summary>
    /// Metadata for individual pages, keyed by filename (without extension).
    /// </summary>
    [JsonPropertyName("pages")]
    public Dictionary<string, NodeMetadata> Pages { get; set; } = [];
}

/// <summary>
/// Default visualization settings for a folder.
/// </summary>
public class FolderDefaults
{
    /// <summary>
    /// Default color for all nodes in this folder.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Default icon for all nodes in this folder.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Default shape for all nodes in this folder.
    /// </summary>
    [JsonPropertyName("shape")]
    public string? Shape { get; set; }

    /// <summary>
    /// Whether children are collapsed by default.
    /// </summary>
    [JsonPropertyName("collapsed")]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Tags inherited by all pages in this folder.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}
