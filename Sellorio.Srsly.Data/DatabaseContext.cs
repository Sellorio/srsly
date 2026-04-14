using Microsoft.EntityFrameworkCore;
using Sellorio.Srsly.Data.Users;

namespace Sellorio.Srsly.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<UserData> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userEntity = modelBuilder.Entity<UserData>();
        userEntity.Property(x => x.Email).UseCollation("NOCASE");
        userEntity.Property(x => x.Username).UseCollation("NOCASE");
    }
}
