using System.ComponentModel.DataAnnotations;
using Oqtane.Models;

namespace AlaWiki.Shared.Models;

/// <summary>
/// Represents a connection to an Azure DevOps wiki Git repository.
/// Stored in Oqtane database per module instance.
/// </summary>
public class WikiConnection : IAuditable
{
    public int WikiConnectionId { get; set; }

    public int ModuleId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Git clone URL for the ADO wiki repository.
    /// Example: https://dev.azure.com/org/project/_git/project.wiki
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string GitUrl { get; set; } = string.Empty;

    /// <summary>
    /// Default branch to use (typically 'wikiMaster' for ADO wikis).
    /// </summary>
    [MaxLength(100)]
    public string DefaultBranch { get; set; } = "wikiMaster";

    /// <summary>
    /// Personal Access Token for authentication.
    /// Should be stored encrypted.
    /// </summary>
    [MaxLength(500)]
    public string? PersonalAccessToken { get; set; }

    /// <summary>
    /// Local path where the wiki is cloned.
    /// </summary>
    [MaxLength(1000)]
    public string? LocalPath { get; set; }

    /// <summary>
    /// Last time the wiki was synced from remote.
    /// </summary>
    public DateTime? LastSyncedOn { get; set; }

    public bool IsActive { get; set; } = true;

    // IAuditable
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
