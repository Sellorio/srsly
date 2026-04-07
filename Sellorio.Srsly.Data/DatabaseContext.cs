using Microsoft.EntityFrameworkCore;
using Sellorio.Srsly.Data.Users;

namespace Sellorio.Srsly.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public required DbSet<UserData> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserData>().Property(x => x.Email).UseCollation("NOCASE");
        modelBuilder.Entity<UserData>().Property(x => x.Username).UseCollation("NOCASE");
    }
}
