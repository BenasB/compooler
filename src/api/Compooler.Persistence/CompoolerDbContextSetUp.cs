using Compooler.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Compooler.Persistence;

public static class CompoolerDbContextSetUp
{
    public static IServiceCollection AddCompoolerDbContext(
        this IServiceCollection services,
        IConfiguration configuration
    ) =>
        services
            .AddDbContext<CompoolerDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("CompoolerDb")
                        ?? throw new InvalidOperationException(
                            "Could not find the database connection string"
                        ),
                    x => x.MigrationsAssembly("Compooler.Persistence")
                )
            )
            .AddScoped<ICompoolerDbContext, CompoolerDbContext>();

    public static async Task InitializeAsync(IServiceProvider services, bool isDevelopment)
    {
        using var scope = services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<CompoolerDbContext>();

        if (isDevelopment)
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                throw new InvalidOperationException("Not all migrations have been applied");
        }
    }
}
