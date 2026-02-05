using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using AlaWiki.Shared.Models;

namespace AlaWiki.Server.Repository;

public interface IWikiConnectionRepository
{
    IEnumerable<WikiConnection> GetWikiConnections(int moduleId);
    WikiConnection? GetWikiConnection(int wikiConnectionId);
    WikiConnection AddWikiConnection(WikiConnection wikiConnection);
    WikiConnection UpdateWikiConnection(WikiConnection wikiConnection);
    void DeleteWikiConnection(int wikiConnectionId);
}

public class WikiConnectionRepository : IWikiConnectionRepository, ITransientService
{
    private readonly AlaWikiContext _db;

    public WikiConnectionRepository(AlaWikiContext context)
    {
        _db = context;
    }

    public IEnumerable<WikiConnection> GetWikiConnections(int moduleId)
    {
        return _db.WikiConnections
            .Where(c => c.ModuleId == moduleId)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public WikiConnection? GetWikiConnection(int wikiConnectionId)
    {
        return _db.WikiConnections.Find(wikiConnectionId);
    }

    public WikiConnection AddWikiConnection(WikiConnection wikiConnection)
    {
        _db.WikiConnections.Add(wikiConnection);
        _db.SaveChanges();
        return wikiConnection;
    }

    public WikiConnection UpdateWikiConnection(WikiConnection wikiConnection)
    {
        _db.Entry(wikiConnection).State = EntityState.Modified;
        _db.SaveChanges();
        return wikiConnection;
    }

    public void DeleteWikiConnection(int wikiConnectionId)
    {
        var connection = _db.WikiConnections.Find(wikiConnectionId);
        if (connection != null)
        {
            _db.WikiConnections.Remove(connection);
            _db.SaveChanges();
        }
    }
}
