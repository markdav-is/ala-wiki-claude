using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using AlaWiki.Shared.Models;
using AlaWiki.Server.Repository;
using AlaWiki.Server.Services;

namespace AlaWiki.Server.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class MindMapController : ModuleControllerBase
{
    private readonly IWikiConnectionRepository _repository;
    private readonly IGitWikiService _gitService;
    private readonly IMindMapGenerator _mindMapGenerator;
    private readonly ILogManager _logger;

    public MindMapController(
        IWikiConnectionRepository repository,
        IGitWikiService gitService,
        IMindMapGenerator mindMapGenerator,
        ILogManager logger)
    {
        _repository = repository;
        _gitService = gitService;
        _mindMapGenerator = mindMapGenerator;
        _logger = logger;
    }

    [HttpGet("generate/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public IActionResult GenerateMindMap(int wikiConnectionId)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            var wikiTree = _gitService.GetWikiTree(connection);
            var mindMap = _mindMapGenerator.GenerateMindMap(wikiTree, connection);
            return Ok(mindMap);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                "Mind Map Generation Failed {WikiConnectionId} {Error}", wikiConnectionId, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("generate/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public IActionResult GenerateMindMapWithSettings(int wikiConnectionId, [FromBody] MindMapSettings settings)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            var wikiTree = _gitService.GetWikiTree(connection);
            var mindMap = _mindMapGenerator.GenerateMindMap(wikiTree, connection, settings);
            return Ok(mindMap);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                "Mind Map Generation Failed {WikiConnectionId} {Error}", wikiConnectionId, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("tree/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public IActionResult GetWikiTree(int wikiConnectionId)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            var wikiTree = _gitService.GetWikiTree(connection);
            return Ok(wikiTree);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                "Wiki Tree Failed {WikiConnectionId} {Error}", wikiConnectionId, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("page/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public IActionResult GetWikiPage(int wikiConnectionId, [FromQuery] string path)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            var page = _gitService.GetWikiPage(connection, path);
            if (page == null)
            {
                return NotFound();
            }
            return Ok(page);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                "Wiki Page Failed {WikiConnectionId} {Path} {Error}", wikiConnectionId, path, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut("metadata/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public IActionResult UpdateMetadata(int wikiConnectionId, [FromQuery] string path, [FromBody] NodeMetadata metadata)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            _gitService.UpdateMetadata(connection, path, metadata);
            _logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Metadata Updated {WikiConnectionId} {Path}", wikiConnectionId, path);
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Update,
                "Metadata Update Failed {WikiConnectionId} {Path} {Error}", wikiConnectionId, path, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("commit/{wikiConnectionId}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> CommitMetadata(int wikiConnectionId, [FromBody] CommitRequest request)
    {
        var connection = _repository.GetWikiConnection(wikiConnectionId);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            await _gitService.CommitMetadataAsync(connection, request.Message ?? "Update mind map metadata");
            _logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Metadata Committed {WikiConnectionId}", wikiConnectionId);
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Update,
                "Metadata Commit Failed {WikiConnectionId} {Error}", wikiConnectionId, ex.Message);
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    public class CommitRequest
    {
        public string? Message { get; set; }
    }
}
