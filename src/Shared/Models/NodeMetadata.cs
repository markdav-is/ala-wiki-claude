using System.Text.Json.Serialization;

namespace AlaWiki.Shared.Models;

/// <summary>
/// Visualization metadata for a single wiki page/node.
/// This is stored in .ala-wiki.json files alongside wiki content.
/// </summary>
public class NodeMetadata
{
    /// <summary>
    /// Relative path to the wiki page this metadata applies to.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Custom display label (overrides page title).
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Node background color (hex or named color).
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Icon identifier for the node.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Whether this node should be collapsed by default in the mind map.
    /// </summary>
    [JsonPropertyName("collapsed")]
    public bool Collapsed { get; set; }

    /// <summary>
    /// Node shape: "rectangle", "ellipse", "rounded", "diamond".
    /// </summary>
    [JsonPropertyName("shape")]
    public string Shape { get; set; } = "rounded";

    /// <summary>
    /// Priority/importance level (1-5) affecting visual weight.
    /// </summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>
    /// Tags for filtering and grouping in visualization.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Custom notes visible in mind map tooltip.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Links to other pages (creates cross-references in mind map).
    /// </summary>
    [JsonPropertyName("links")]
    public List<string> Links { get; set; } = [];

    /// <summary>
    /// Custom X position for manual layout (null = auto-layout).
    /// </summary>
    [JsonPropertyName("x")]
    public double? X { get; set; }

    /// <summary>
    /// Custom Y position for manual layout (null = auto-layout).
    /// </summary>
    [JsonPropertyName("y")]
    public double? Y { get; set; }
}
