using Microsoft.EntityFrameworkCore;
using Oqtane.Databases.Interfaces;
using Oqtane.Repository;
using AlaWiki.Shared.Models;

namespace AlaWiki.Server.Repository;

public class AlaWikiContext : DBContextBase, ITransientService, IMultiDatabase
{
    public AlaWikiContext(IDBContextDependencies DBContextDependencies)
        : base(DBContextDependencies) { }

    public virtual DbSet<WikiConnection> WikiConnections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WikiConnection>(entity =>
        {
            entity.ToTable("AlaWiki_WikiConnection");
            entity.HasKey(e => e.WikiConnectionId);
            entity.HasIndex(e => e.ModuleId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.GitUrl).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.DefaultBranch).HasMaxLength(100);
            entity.Property(e => e.PersonalAccessToken).HasMaxLength(500);
            entity.Property(e => e.LocalPath).HasMaxLength(1000);
        });
    }
}
