using Microsoft.EntityFrameworkCore;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Database;

public class OsuCzechiaBotDatabaseContext(DbContextOptions<OsuCzechiaBotDatabaseContext> options) : DbContext(options)
{
    public const string SchemaName = "osu_czechia";
    
    public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
    public DbSet<OneTimeJobLog> OneTimeJobLogs { get; set; }
    public DbSet<ReactionRole> ReactionRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        
        modelBuilder.Entity<ReactionRole>().HasIndex(r => new { r.EmojiId, r.EmojiName }).IsUnique();
        modelBuilder.Entity<ReactionRole>().Property(r => r.Id).ValueGeneratedOnAdd();
    }
}