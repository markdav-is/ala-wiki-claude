using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using AlaWiki.Module.MindMap.Shared.Models;
using AlaWiki.Module.MindMap.Server.Repository;
using AlaWiki.Module.MindMap.Server.Services;

namespace AlaWiki.Module.MindMap.Server.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class WikiConnectionController : ModuleControllerBase
{
    private readonly IWikiConnectionRepository _repository;
    private readonly IGitWikiService _gitService;
    private readonly ILogManager _logger;

    public WikiConnectionController(
        IWikiConnectionRepository repository,
        IGitWikiService gitService,
        ILogManager logger)
    {
        _repository = repository;
        _gitService = gitService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public IEnumerable<WikiConnection> Get(int moduleid)
    {
        return _repository.GetWikiConnections(moduleid);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public WikiConnection? Get(int id)
    {
        return _repository.GetWikiConnection(id);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    public WikiConnection Post([FromBody] WikiConnection connection)
    {
        if (ModelState.IsValid)
        {
            connection.CreatedBy = User.Identity?.Name;
            connection.CreatedOn = DateTime.UtcNow;
            connection.ModifiedBy = User.Identity?.Name;
            connection.ModifiedOn = DateTime.UtcNow;

            connection = _repository.AddWikiConnection(connection);
            _logger.Log(LogLevel.Information, this, LogFunction.Create,
                "Wiki Connection Added {WikiConnection}", connection);
        }
        return connection;
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public WikiConnection Put(int id, [FromBody] WikiConnection connection)
    {
        if (ModelState.IsValid && connection.WikiConnectionId == id)
        {
            connection.ModifiedBy = User.Identity?.Name;
            connection.ModifiedOn = DateTime.UtcNow;

            connection = _repository.UpdateWikiConnection(connection);
            _logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Wiki Connection Updated {WikiConnection}", connection);
        }
        return connection;
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public void Delete(int id)
    {
        _repository.DeleteWikiConnection(id);
        _logger.Log(LogLevel.Information, this, LogFunction.Delete,
            "Wiki Connection Deleted {WikiConnectionId}", id);
    }

    [HttpGet("{id}/test")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> TestConnection(int id)
    {
        var connection = _repository.GetWikiConnection(id);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            var success = await _gitService.TestConnectionAsync(connection);
            return Ok(new { Success = success, Message = success ? "Connection successful" : "Connection failed" });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Read,
                "Wiki Connection Test Failed {WikiConnectionId} {Error}", id, ex.Message);
            return Ok(new { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{id}/sync")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<IActionResult> Sync(int id)
    {
        var connection = _repository.GetWikiConnection(id);
        if (connection == null)
        {
            return NotFound();
        }

        try
        {
            await _gitService.SyncWikiAsync(connection);
            _logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Wiki Synced {WikiConnectionId}", id);
            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Update,
                "Wiki Sync Failed {WikiConnectionId} {Error}", id, ex.Message);
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }
}
