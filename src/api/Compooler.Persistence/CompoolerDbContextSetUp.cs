using Compooler.Application;
using Compooler.Domain;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence.Configurations;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NpgsqlTypes;

namespace Compooler.Persistence;

public static class CompoolerDbContextSetUp
{
    public static IServiceCollection AddCompoolerDbContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("CompoolerDb")
            ?? throw new InvalidOperationException("Could not find the database connection string");

        return services.AddDbContext<ICompoolerDbContext, CompoolerDbContext>(
            o => o.UseCompoolerDatabase(connectionString),
            ServiceLifetime.Scoped,
            ServiceLifetime.Singleton
        );
    }

    public static DbContextOptionsBuilder UseCompoolerDatabase(
        this DbContextOptionsBuilder builder,
        string connectionString
    )
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        return builder.UseNpgsql(
            dataSource,
            o => o.MigrationsAssembly("Compooler.Persistence").UseNetTopologySuite()
        );
    }

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
                    User.Create(
                        Guid.NewGuid().ToString("N")[..UserConfiguration.IdLength],
                        "Benas",
                        "Bud"
                    ),
                    User.Create(
                        Guid.NewGuid().ToString("N")[..UserConfiguration.IdLength],
                        "John",
                        "Doe"
                    ),
                    User.Create(
                        Guid.NewGuid().ToString("N")[..UserConfiguration.IdLength],
                        "Jermaine",
                        "Cole"
                    )
                ];
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync();

                var dateTimeOffsetProvider =
                    scope.ServiceProvider.GetRequiredService<IDateTimeOffsetProvider>();

                Ride[] rides =
                [
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(66.21321, 162.321).Value!,
                            GeographicCoordinates.Create(67.3232, 162.121).Value!
                        ),
                        users[0].Id,
                        2,
                        DateTimeOffset.Now.AddDays(1).ToUniversalTime(),
                        dateTimeOffsetProvider
                    ).Value!,
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(-65.5123, -2.421).Value!,
                            GeographicCoordinates.Create(-11.3, -112.441).Value!
                        ),
                        users[0].Id,
                        3,
                        DateTimeOffset.Now.AddDays(2).AddMinutes(14).ToUniversalTime(),
                        dateTimeOffsetProvider
                    ).Value!,
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(-12.5123, 32.421).Value!,
                            GeographicCoordinates.Create(-11.3, -112.441).Value!
                        ),
                        users[1].Id,
                        3,
                        DateTimeOffset.Now.AddDays(4).AddHours(3).AddMinutes(14).ToUniversalTime(),
                        dateTimeOffsetProvider
                    ).Value!,
                    Ride.Create(
                        Route.Create(
                            GeographicCoordinates.Create(52.252058, 20.946427).Value!,
                            GeographicCoordinates.Create(50.241338, 19.006077).Value!
                        ),
                        users[1].Id,
                        3,
                        DateTimeOffset.Now.AddDays(4).AddHours(3).AddMinutes(14).ToUniversalTime(),
                        dateTimeOffsetProvider
                    ).Value!
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

        const string query = $"""
            WITH PartitionedRides AS (
                SELECT
                    rp."{nameof(RidePassenger.UserId)}",
                    r."{nameof(Ride.Id)}",
                    ROW_NUMBER() OVER
                    (PARTITION BY rp."{nameof(RidePassenger.UserId)}"
                    ORDER BY r."{nameof(Ride.Id)}") AS "RowNumber"
                FROM "{nameof(CompoolerDbContext.Rides)}" r
                JOIN "{nameof(CompoolerDbContext.RidePassengers)}" rp
                    ON r."{nameof(Ride.Id)}" = rp."{RideConfiguration.RideIdColumnName}"
                WHERE rp."UserId" = ANY(@userIds) AND r."Id" > @after
            )
            SELECT *
            FROM PartitionedRides
            WHERE "RowNumber" <= @first;
            """;

        var userIds = new NpgsqlParameter(
            "userIds",
            new[] { "b39ca817b7b14cbb9f7ff7bc8a30", "19de0d3457144406b38a528268ee" }
        );
        var after = new NpgsqlParameter("after", value: 0);
        var first = new NpgsqlParameter("first", value: 1);

        var r = await dbContext
            .Database.SqlQueryRaw<PaginationResult>(query, userIds, after, first)
            .ToListAsync();
    }

    private record PaginationResult(string UserId, int RowNumber, int Id);
}
