using System.Text.Json;
using LibGit2Sharp;
using Oqtane.Infrastructure;
using AlaWiki.Module.MindMap.Shared.Models;
using AlaWiki.Module.MindMap.Server.Repository;
using Microsoft.Extensions.Logging;

namespace AlaWiki.Module.MindMap.Server.Services;

public interface IGitWikiService
{
    Task<bool> TestConnectionAsync(WikiConnection connection);
    Task SyncWikiAsync(WikiConnection connection);
    WikiPage GetWikiTree(WikiConnection connection);
    WikiPage? GetWikiPage(WikiConnection connection, string path);
    void UpdateMetadata(WikiConnection connection, string path, NodeMetadata metadata);
    Task CommitMetadataAsync(WikiConnection connection, string message);
}

public class GitWikiService : IGitWikiService
{
    private readonly ILogger<GitWikiService> _logger;
    private readonly IWikiConnectionRepository _repository;
    private readonly string _basePath;

    public GitWikiService(
        ILogger<GitWikiService> logger,
        IWikiConnectionRepository repository,
        IServerStateManager serverState)
    {
        _logger = logger;
        _repository = repository;
        _basePath = Path.Combine(serverState.ContentRootPath, "Content", "AlaWiki");

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<bool> TestConnectionAsync(WikiConnection connection)
    {
        try
        {
            var options = CreateFetchOptions(connection);
            var refs = await Task.Run(() =>
                LibGit2Sharp.Repository.ListRemoteReferences(connection.GitUrl, (url, fromUrl, types) =>
                {
                    if (!string.IsNullOrEmpty(connection.PersonalAccessToken))
                    {
                        return new UsernamePasswordCredentials
                        {
                            Username = "pat",
                            Password = connection.PersonalAccessToken
                        };
                    }
                    return new DefaultCredentials();
                }));

            return refs.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection to {GitUrl}", connection.GitUrl);
            return false;
        }
    }

    public async Task SyncWikiAsync(WikiConnection connection)
    {
        var localPath = GetLocalPath(connection);

        await Task.Run(() =>
        {
            if (Directory.Exists(localPath) && LibGit2Sharp.Repository.IsValid(localPath))
            {
                // Pull latest changes
                using var repo = new LibGit2Sharp.Repository(localPath);
                var options = new PullOptions
                {
                    FetchOptions = CreateFetchOptions(connection)
                };

                var signature = new Signature("AlaWiki", "alawiki@local", DateTimeOffset.Now);
                Commands.Pull(repo, signature, options);
            }
            else
            {
                // Clone repository
                if (Directory.Exists(localPath))
                {
                    Directory.Delete(localPath, true);
                }

                var options = new CloneOptions
                {
                    BranchName = connection.DefaultBranch,
                    FetchOptions = CreateFetchOptions(connection)
                };

                LibGit2Sharp.Repository.Clone(connection.GitUrl, localPath, options);
            }
        });

        // Update connection with local path and sync time
        connection.LocalPath = localPath;
        connection.LastSyncedOn = DateTime.UtcNow;
        _repository.UpdateWikiConnection(connection);
    }

    public WikiPage GetWikiTree(WikiConnection connection)
    {
        var localPath = GetLocalPath(connection);

        if (!Directory.Exists(localPath))
        {
            throw new InvalidOperationException("Wiki not synced. Please sync first.");
        }

        var root = new WikiPage
        {
            Path = "",
            Title = connection.Name,
            Order = 0
        };

        BuildTree(localPath, "", root);
        return root;
    }

    public WikiPage? GetWikiPage(WikiConnection connection, string path)
    {
        var localPath = GetLocalPath(connection);
        var fullPath = Path.Combine(localPath, path);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        var content = File.ReadAllText(fullPath);
        var title = ExtractTitle(content, path);
        var metadata = LoadMetadata(localPath, path);

        return new WikiPage
        {
            Path = path,
            Title = title,
            Content = content,
            ParentPath = Path.GetDirectoryName(path) ?? "",
            Metadata = metadata
        };
    }

    public void UpdateMetadata(WikiConnection connection, string pagePath, NodeMetadata metadata)
    {
        var localPath = GetLocalPath(connection);
        var directory = Path.GetDirectoryName(Path.Combine(localPath, pagePath)) ?? localPath;
        var metadataFile = Path.Combine(directory, ".ala-wiki.json");

        AlaWikiMetadataFile metadataContainer;

        if (File.Exists(metadataFile))
        {
            var json = File.ReadAllText(metadataFile);
            metadataContainer = JsonSerializer.Deserialize<AlaWikiMetadataFile>(json)
                                ?? new AlaWikiMetadataFile();
        }
        else
        {
            metadataContainer = new AlaWikiMetadataFile();
        }

        var pageKey = Path.GetFileNameWithoutExtension(pagePath);
        metadataContainer.Pages[pageKey] = metadata;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(metadataFile, JsonSerializer.Serialize(metadataContainer, options));
    }

    public async Task CommitMetadataAsync(WikiConnection connection, string message)
    {
        var localPath = GetLocalPath(connection);

        await Task.Run(() =>
        {
            using var repo = new LibGit2Sharp.Repository(localPath);

            // Stage all .ala-wiki.json files
            Commands.Stage(repo, "*.ala-wiki.json");

            var status = repo.RetrieveStatus();
            if (!status.Staged.Any())
            {
                return; // Nothing to commit
            }

            var signature = new Signature("AlaWiki", "alawiki@local", DateTimeOffset.Now);
            repo.Commit(message, signature, signature);

            // Push if we have credentials
            if (!string.IsNullOrEmpty(connection.PersonalAccessToken))
            {
                var remote = repo.Network.Remotes["origin"];
                var options = new PushOptions
                {
                    CredentialsProvider = (url, user, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = "pat",
                            Password = connection.PersonalAccessToken
                        }
                };

                repo.Network.Push(remote, @"refs/heads/" + connection.DefaultBranch, options);
            }
        });
    }

    private void BuildTree(string basePath, string relativePath, WikiPage parent)
    {
        var currentPath = string.IsNullOrEmpty(relativePath)
            ? basePath
            : Path.Combine(basePath, relativePath);

        // Load .order file if exists
        var orderFile = Path.Combine(currentPath, ".order");
        var order = new Dictionary<string, int>();
        if (File.Exists(orderFile))
        {
            var lines = File.ReadAllLines(orderFile);
            for (int i = 0; i < lines.Length; i++)
            {
                order[lines[i].Trim()] = i;
            }
        }

        // Load metadata for this folder
        var metadataFile = Path.Combine(currentPath, ".ala-wiki.json");
        AlaWikiMetadataFile? metadata = null;
        if (File.Exists(metadataFile))
        {
            try
            {
                var json = File.ReadAllText(metadataFile);
                metadata = JsonSerializer.Deserialize<AlaWikiMetadataFile>(json);
            }
            catch
            {
                // Ignore invalid metadata files
            }
        }

        // Process markdown files
        var mdFiles = Directory.GetFiles(currentPath, "*.md")
            .Where(f => !Path.GetFileName(f).StartsWith("."));

        foreach (var file in mdFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var fileRelativePath = string.IsNullOrEmpty(relativePath)
                ? Path.GetFileName(file)
                : Path.Combine(relativePath, Path.GetFileName(file));

            var content = File.ReadAllText(file);
            var title = ExtractTitle(content, fileName);

            var page = new WikiPage
            {
                Path = fileRelativePath,
                Title = title,
                ParentPath = relativePath,
                Order = order.TryGetValue(fileName, out var o) ? o : 999
            };

            // Apply metadata if available
            if (metadata?.Pages.TryGetValue(fileName, out var nodeMeta) == true)
            {
                page.Metadata = nodeMeta;
                if (!string.IsNullOrEmpty(nodeMeta.Label))
                {
                    page.Title = nodeMeta.Label;
                }
            }

            parent.Children.Add(page);
        }

        // Process subdirectories
        var dirs = Directory.GetDirectories(currentPath)
            .Where(d => !Path.GetFileName(d).StartsWith("."));

        foreach (var dir in dirs)
        {
            var dirName = Path.GetFileName(dir);
            var dirRelativePath = string.IsNullOrEmpty(relativePath)
                ? dirName
                : Path.Combine(relativePath, dirName);

            var folder = new WikiPage
            {
                Path = dirRelativePath,
                Title = dirName.Replace("-", " "),
                ParentPath = relativePath,
                Order = order.TryGetValue(dirName, out var o) ? o : 999
            };

            BuildTree(basePath, dirRelativePath, folder);
            parent.Children.Add(folder);
        }

        // Sort children by order
        parent.Children = parent.Children.OrderBy(c => c.Order).ThenBy(c => c.Title).ToList();
    }

    private NodeMetadata? LoadMetadata(string basePath, string pagePath)
    {
        var directory = Path.GetDirectoryName(Path.Combine(basePath, pagePath)) ?? basePath;
        var metadataFile = Path.Combine(directory, ".ala-wiki.json");

        if (!File.Exists(metadataFile))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(metadataFile);
            var container = JsonSerializer.Deserialize<AlaWikiMetadataFile>(json);
            var pageKey = Path.GetFileNameWithoutExtension(pagePath);

            return container?.Pages.TryGetValue(pageKey, out var metadata) == true
                ? metadata
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static string ExtractTitle(string content, string fallback)
    {
        // Try to extract first H1 heading
        var lines = content.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("# "))
            {
                return trimmed[2..].Trim();
            }
        }

        // Fall back to filename
        return fallback.Replace("-", " ").Replace("_", " ");
    }

    private string GetLocalPath(WikiConnection connection)
    {
        // Use existing path or generate new one
        if (!string.IsNullOrEmpty(connection.LocalPath) &&
            Directory.Exists(connection.LocalPath))
        {
            return connection.LocalPath;
        }

        return Path.Combine(_basePath, $"wiki_{connection.WikiConnectionId}");
    }

    private FetchOptions CreateFetchOptions(WikiConnection connection)
    {
        return new FetchOptions
        {
            CredentialsProvider = (url, user, types) =>
            {
                if (!string.IsNullOrEmpty(connection.PersonalAccessToken))
                {
                    return new UsernamePasswordCredentials
                    {
                        Username = "pat",
                        Password = connection.PersonalAccessToken
                    };
                }
                return new DefaultCredentials();
            }
        };
    }
}
