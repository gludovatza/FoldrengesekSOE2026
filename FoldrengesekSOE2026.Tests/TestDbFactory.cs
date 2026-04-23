using FoldrengesekSOE2026.Data;
using Microsoft.EntityFrameworkCore;

namespace FoldrengesekSOE2026.Tests;

public class TestDbFactory
{
    public static FoldrengesContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<FoldrengesContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new FoldrengesContext(options);
    }

}
