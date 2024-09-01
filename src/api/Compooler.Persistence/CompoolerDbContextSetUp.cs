using Compooler.Application;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
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

            if (!dbContext.Users.Any() && !dbContext.Rides.Any())
            {
                User[] users =
                [
                    User.Create("Benas", "Bud"),
                    User.Create("John", "Doe"),
                    User.Create("Jermaine", "Cole")
                ];
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync();

                Ride[] rides =
                [
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(66.21321, 162.321).Value!,
                            GeographicCoordinates.Create(67.3232, 162.121).Value!
                        ),
                        users[0].Id,
                        2,
                        DateTimeOffset.Now.AddDays(1).ToUniversalTime()
                    ),
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(-12.5123, 32.421).Value!,
                            GeographicCoordinates.Create(-11.3, -112.441).Value!
                        ),
                        users[0].Id,
                        3,
                        DateTimeOffset.Now.AddDays(2).AddMinutes(14).ToUniversalTime()
                    ),
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(-12.5123, 32.421).Value!,
                            GeographicCoordinates.Create(-11.3, -112.441).Value!
                        ),
                        users[1].Id,
                        3,
                        DateTimeOffset.Now.AddDays(4).AddHours(3).AddMinutes(14).ToUniversalTime()
                    )
                ];
                rides[0].AddPassenger(users[1].Id);
                rides[0].AddPassenger(users[2].Id);

                rides[1].AddPassenger(users[2].Id);

                dbContext.Rides.AddRange(rides);
                await dbContext.SaveChangesAsync();
            }
        }
        else
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                throw new InvalidOperationException("Not all migrations have been applied");
        }
    }
}
