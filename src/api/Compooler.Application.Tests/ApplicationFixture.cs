using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Compooler.Application.Tests;

public class ApplicationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public DbContextOptions<CompoolerDbContext> DbContextOptions =>
        _contextOptions ?? throw new InvalidOperationException("Database not initialized");

    private DbContextOptions<CompoolerDbContext>? _contextOptions;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _contextOptions = new DbContextOptionsBuilder<CompoolerDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
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
