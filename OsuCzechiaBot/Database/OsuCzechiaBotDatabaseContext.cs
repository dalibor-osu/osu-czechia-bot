using Microsoft.EntityFrameworkCore;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database;

public class OsuCzechiaBotDatabaseContext(DbContextOptions<OsuCzechiaBotDatabaseContext> options) : DbContext(options)
{
    public const string SchemaName = "osu_czechia";
    
    public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
    }
}