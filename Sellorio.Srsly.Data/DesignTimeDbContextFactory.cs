using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sellorio.Srsly.Data;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var options =
            new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(string.Empty)
                .Options;

        return new DatabaseContext(options);
    }
}
