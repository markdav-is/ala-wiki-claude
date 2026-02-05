using AlaWiki.Module.MindMap.Shared.Models;

namespace AlaWiki.Module.MindMap.Server.Services;

public interface IMindMapGenerator
{
    MindMap GenerateMindMap(WikiPage wikiTree, WikiConnection connection, MindMapSettings? settings = null);
}

public class MindMapGenerator : IMindMapGenerator
{
    // Default colors for depth levels
    private static readonly string[] DepthColors =
    [
        "#4a90d9", // Root - blue
        "#50c878", // Level 1 - green
        "#f5a623", // Level 2 - orange
        "#9b59b6", // Level 3 - purple
        "#e74c3c", // Level 4 - red
        "#1abc9c", // Level 5 - teal
        "#f39c12", // Level 6 - yellow
        "#3498db"  // Level 7+ - light blue
    ];

    public MindMap GenerateMindMap(WikiPage wikiTree, WikiConnection connection, MindMapSettings? settings = null)
    {
        settings ??= new MindMapSettings();

        var mindMap = new MindMap
        {
            WikiConnectionId = connection.WikiConnectionId,
            Name = connection.Name,
            Settings = settings,
            GeneratedAt = DateTime.UtcNow
        };

        var nodeIndex = new Dictionary<string, MindMapNode>();
        var rootNode = ConvertToMindMapNode(wikiTree, null, 0, settings, nodeIndex);
        mindMap.Root = rootNode;

        // Generate cross-reference edges
        mindMap.Edges = GenerateEdges(nodeIndex);

        return mindMap;
    }

    private MindMapNode ConvertToMindMapNode(
        WikiPage page,
        string? parentId,
        int depth,
        MindMapSettings settings,
        Dictionary<string, MindMapNode> nodeIndex)
    {
        if (depth > settings.MaxDepth)
        {
            return new MindMapNode
            {
                Id = GenerateId(page.Path),
                Label = "...",
                WikiPath = page.Path,
                ParentId = parentId,
                Style = new NodeStyle { BackgroundColor = "#cccccc" }
            };
        }

        var nodeId = GenerateId(page.Path);
        var node = new MindMapNode
        {
            Id = nodeId,
            Label = page.Metadata?.Label ?? page.Title,
            WikiPath = page.Path,
            ParentId = parentId,
            Style = CreateNodeStyle(page, depth),
            IsCollapsed = page.Metadata?.Collapsed ?? (depth > 2),
            X = page.Metadata?.X,
            Y = page.Metadata?.Y,
            Tooltip = page.Metadata?.Notes,
            Tags = page.Metadata?.Tags ?? [],
            Links = page.Metadata?.Links ?? []
        };

        nodeIndex[nodeId] = node;

        // Apply tag-based filtering
        if (settings.FilterTags?.Count > 0)
        {
            // Only include nodes that have at least one matching tag or are ancestors
            var hasMatchingTag = node.Tags.Intersect(settings.FilterTags).Any();
            if (!hasMatchingTag && page.Children.Count == 0)
            {
                return null!; // Will be filtered out
            }
        }

        // Process children
        foreach (var child in page.Children)
        {
            var childNode = ConvertToMindMapNode(child, nodeId, depth + 1, settings, nodeIndex);
            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private NodeStyle CreateNodeStyle(WikiPage page, int depth)
    {
        var style = new NodeStyle();

        // Apply metadata styling if available
        if (page.Metadata != null)
        {
            if (!string.IsNullOrEmpty(page.Metadata.Color))
            {
                style.BackgroundColor = page.Metadata.Color;
            }
            else
            {
                style.BackgroundColor = GetDepthColor(depth);
            }

            style.Icon = page.Metadata.Icon;
            style.Shape = page.Metadata.Shape ?? "rounded";

            // Adjust style based on priority
            if (page.Metadata.Priority.HasValue)
            {
                var priority = page.Metadata.Priority.Value;
                style.BorderWidth = Math.Max(1, 4 - priority + 1);
                if (priority <= 2)
                {
                    style.BorderColor = "#e74c3c"; // Red border for high priority
                }
            }
        }
        else
        {
            style.BackgroundColor = GetDepthColor(depth);

            // Folders get a different shape
            if (page.Children.Count > 0)
            {
                style.Shape = "rounded";
                style.Icon = "oi oi-folder";
            }
        }

        // Root node styling
        if (depth == 0)
        {
            style.FontSize = 18;
            style.Padding = 12;
            style.BorderWidth = 3;
        }

        return style;
    }

    private List<MindMapEdge> GenerateEdges(Dictionary<string, MindMapNode> nodeIndex)
    {
        var edges = new List<MindMapEdge>();

        foreach (var node in nodeIndex.Values)
        {
            // Create hierarchy edges (parent-child relationships)
            foreach (var child in node.Children)
            {
                edges.Add(new MindMapEdge
                {
                    SourceId = node.Id,
                    TargetId = child.Id,
                    EdgeType = "hierarchy",
                    Color = "#cccccc"
                });
            }

            // Create reference edges (cross-links)
            foreach (var link in node.Links)
            {
                var targetId = GenerateId(link);
                if (nodeIndex.ContainsKey(targetId))
                {
                    edges.Add(new MindMapEdge
                    {
                        SourceId = node.Id,
                        TargetId = targetId,
                        EdgeType = "reference",
                        Color = "#3498db",
                        Label = "ref"
                    });
                }
            }
        }

        return edges;
    }

    private static string GenerateId(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return "root";
        }

        // Create a stable ID from the path
        return "node_" + path
            .Replace("/", "_")
            .Replace("\\", "_")
            .Replace(".", "_")
            .Replace(" ", "_")
            .ToLowerInvariant();
    }

    private static string GetDepthColor(int depth)
    {
        var index = Math.Min(depth, DepthColors.Length - 1);
        return DepthColors[index];
    }
}
