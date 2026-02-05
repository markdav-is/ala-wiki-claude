namespace AlaWiki.Shared.Models;

/// <summary>
/// Represents a node in the mind map visualization.
/// Combines WikiPage data with NodeMetadata for rendering.
/// </summary>
public class MindMapNode
{
    /// <summary>
    /// Unique identifier for this node.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display label for the node.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Path to the wiki page.
    /// </summary>
    public string WikiPath { get; set; } = string.Empty;

    /// <summary>
    /// Parent node ID (null for root).
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Child nodes.
    /// </summary>
    public List<MindMapNode> Children { get; set; } = [];

    /// <summary>
    /// Visual styling for this node.
    /// </summary>
    public NodeStyle Style { get; set; } = new();

    /// <summary>
    /// Whether this node is currently collapsed.
    /// </summary>
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Position X (for manual layout).
    /// </summary>
    public double? X { get; set; }

    /// <summary>
    /// Position Y (for manual layout).
    /// </summary>
    public double? Y { get; set; }

    /// <summary>
    /// Tooltip content.
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Tags for filtering.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Cross-reference links to other nodes.
    /// </summary>
    public List<string> Links { get; set; } = [];
}

/// <summary>
/// Visual styling properties for a mind map node.
/// </summary>
public class NodeStyle
{
    public string BackgroundColor { get; set; } = "#ffffff";
    public string BorderColor { get; set; } = "#333333";
    public string TextColor { get; set; } = "#333333";
    public string Shape { get; set; } = "rounded";
    public string? Icon { get; set; }
    public int FontSize { get; set; } = 14;
    public int BorderWidth { get; set; } = 1;
    public int Padding { get; set; } = 8;
}
