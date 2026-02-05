namespace AlaWiki.Module.MindMap.Shared.Models;

/// <summary>
/// Represents a wiki page parsed from the Git repository.
/// This is a transient object, not stored in database.
/// </summary>
public class WikiPage
{
    /// <summary>
    /// Relative path from wiki root (e.g., "Architecture/Overview.md").
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Page title extracted from filename or first heading.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Parent folder path (e.g., "Architecture" for "Architecture/Overview.md").
    /// Empty string for root-level pages.
    /// </summary>
    public string ParentPath { get; set; } = string.Empty;

    /// <summary>
    /// Order within parent folder (from .order file or alphabetical).
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Raw markdown content of the page.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Child pages (for folder/section pages).
    /// </summary>
    public List<WikiPage> Children { get; set; } = [];

    /// <summary>
    /// Visualization metadata from .ala-wiki.json file.
    /// </summary>
    public NodeMetadata? Metadata { get; set; }
}
