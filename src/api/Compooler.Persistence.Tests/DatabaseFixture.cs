using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Compooler.Persistence.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgis/postgis:16-3.4")
        .Build();

    public DbContextOptions DbContextOptions =>
        _contextOptions ?? throw new InvalidOperationException("Database not initialized");

    private DbContextOptions? _contextOptions;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _contextOptions = new DbContextOptionsBuilder()
            .UseCompoolerDatabase(_dbContainer.GetConnectionString())
            .LogTo(Console.WriteLine, LogLevel.Information)
            .Options;

        await using var context = new CompoolerDbContext(_contextOptions);
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
