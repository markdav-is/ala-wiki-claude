namespace AlaWiki.Shared.Models;

/// <summary>
/// Represents a complete mind map visualization.
/// </summary>
public class MindMap
{
    /// <summary>
    /// Wiki connection this mind map is generated from.
    /// </summary>
    public int WikiConnectionId { get; set; }

    /// <summary>
    /// Name of the wiki/mind map.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Root node of the mind map tree.
    /// </summary>
    public MindMapNode? Root { get; set; }

    /// <summary>
    /// Flat list of all edges (for cross-reference links).
    /// </summary>
    public List<MindMapEdge> Edges { get; set; } = [];

    /// <summary>
    /// Global visualization settings.
    /// </summary>
    public MindMapSettings Settings { get; set; } = new();

    /// <summary>
    /// Git commit hash this map was generated from.
    /// </summary>
    public string? CommitHash { get; set; }

    /// <summary>
    /// When the map was last generated.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents an edge/link between nodes.
/// </summary>
public class MindMapEdge
{
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string EdgeType { get; set; } = "hierarchy"; // "hierarchy" or "reference"
    public string Color { get; set; } = "#999999";
}

/// <summary>
/// Global settings for mind map visualization.
/// </summary>
public class MindMapSettings
{
    public string LayoutDirection { get; set; } = "right"; // "right", "left", "down", "up", "radial"
    public int NodeSpacingX { get; set; } = 200;
    public int NodeSpacingY { get; set; } = 50;
    public bool ShowCrossReferences { get; set; } = true;
    public string Theme { get; set; } = "light";
    public int MaxDepth { get; set; } = 10;
    public List<string>? FilterTags { get; set; }
}
