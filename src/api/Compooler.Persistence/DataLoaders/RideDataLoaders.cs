using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence.Configurations;
using GreenDonut;
using HotChocolate.Pagination;
using HotChocolate.Pagination.Expressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Compooler.Persistence.DataLoaders;

public sealed class RideDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Ride>> GetRideByIdAsync(
        IReadOnlyList<int> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .Rides.AsNoTracking()
            .Where(cg => keys.Contains(cg.Id))
            .ToDictionaryAsync(cg => cg.Id, cancellationToken);

    [DataLoader]
    public static async Task<ILookup<string, int>> GetRideIdsByUserIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var ridePassengers = await dbContext
            .RidePassengers.AsNoTracking()
            .Where(rp => keys.Contains(rp.UserId))
            .Select(rp => new
            {
                rp.UserId,
                RideId = EF.Property<int>(rp, RideConfiguration.RideIdColumnName)
            })
            .ToListAsync(cancellationToken);

        return ridePassengers.ToLookup(x => x.UserId, x => x.RideId);
    }

    [DataLoader]
    public static async Task<
        IReadOnlyDictionary<string, Page<(DateTimeOffset TimeOfDeparture, int RideId)>>
    > GetTestRideIdsByUserIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken
    )
    {
        if (pagingArguments.Last != null || pagingArguments.Before != null)
            throw new ArgumentException(
                $"'{nameof(pagingArguments.Last)}' and '{nameof(pagingArguments.Before)}' not supported,"
                    + $"use '{nameof(pagingArguments.First)}' and '{nameof(pagingArguments.After)}'"
            );

        var tables = new
        {
            Rides = nameof(dbContext.Rides),
            RidePassengers = nameof(dbContext.RidePassengers)
        };

        var columns = new
        {
            Rides = new
            {
                Id = nameof(Ride.Id),
                DriverId = nameof(Ride.DriverId),
                TimeOfDeparture = nameof(Ride.TimeOfDeparture)
            },
            RidePassengers = new
            {
                RideId = RideConfiguration.RideIdColumnName,
                UserId = nameof(RidePassenger.UserId)
            }
        };

        var sqlParameters = new List<NpgsqlParameter>();
        var paginationFilter = """("TOD", "RideId") > (CURRENT_TIMESTAMP, 0)""";

        var cursor = GetCursor();
        if (cursor.HasValue)
        {
            paginationFilter = """("TOD", "RideId") > (@afterTimeOfDeparture, @afterId)""";
            sqlParameters.Add(
                new NpgsqlParameter(
                    "afterTimeOfDeparture",
                    value: cursor.Value.afterTimeOfDeparture
                )
            );
            sqlParameters.Add(new NpgsqlParameter("afterId", value: cursor.Value.afterId));
        }

        /*language=sql*/
        var query = $"""
            WITH CombinedRides AS (
                -- Rides where user is a passenger
                -- Index (UserId, RideId) utilized
                SELECT
                    rp."{columns.RidePassengers.UserId}" AS "UserId",
                    r."{columns.Rides.Id}" AS "RideId",
                    r."{columns.Rides.TimeOfDeparture}" AS "TOD",
                    'Passenger' AS "Role"
                FROM "{tables.Rides}" r
                    INNER JOIN "{tables.RidePassengers}" rp
                        ON r."{columns.Rides.Id}" = rp."{columns.RidePassengers.RideId}"
                WHERE rp."UserId" = ANY(@userIds)

                UNION ALL

                -- Rides where user is the driver
                -- Index (DriverId, TimeOfDeparture, Id) utilized (combined with PartitionedRides)
                SELECT
                    r."{columns.Rides.DriverId}" AS "UserId",
                    r."{columns.Rides.Id}" AS "RideId",
                    r."{columns.Rides.TimeOfDeparture}" AS "TOD",
                    'Driver' AS "Role"
                FROM "{tables.Rides}" r
                WHERE r."{columns.Rides.DriverId}" = ANY(@userIds)
            ),
            PartitionedRides AS (
                SELECT
                    "UserId",
                    "TOD",
                    "RideId",
                    ROW_NUMBER() OVER (PARTITION BY "UserId" ORDER BY "TOD", "RideId") AS "RowNumber"
                FROM CombinedRides
                WHERE {paginationFilter}
            )
            SELECT "UserId", "TOD" AS "TimeOfDeparture", "RideId"
            FROM PartitionedRides
            WHERE "RowNumber" <= @first;
            """;

        sqlParameters.Add(new NpgsqlParameter("userIds", keys));
        sqlParameters.Add(new NpgsqlParameter("first", value: pagingArguments.First));

        var rows = await dbContext
            .Database.SqlQueryRaw<UserRideRow>(query, parameters: sqlParameters.ToArray<object>())
            .ToListAsync(cancellationToken: cancellationToken);

        return rows.GroupBy(row => row.UserId, row => (row.TimeOfDeparture, row.RideId))
            .ToDictionary(
                x => x.Key,
                x => new Page<(DateTimeOffset TimeOfDeparture, int RideId)>(
                    items: [.. x],
                    hasNextPage: false, // TODO
                    hasPreviousPage: default,
                    createCursor: row => "" // TODO
                )
            );

        (DateTimeOffset afterTimeOfDeparture, int afterId)? GetCursor()
        {
            if (pagingArguments.After == null)
                return null;

            var cursorKeyParser = new CursorKeyParser();
            cursorKeyParser.Visit(
                dbContext.Rides.OrderBy(r => r.TimeOfDeparture).ThenBy(r => r.Id).Expression
            );
            var parsedCursor = CursorParser.Parse(pagingArguments.After, [.. cursorKeyParser.Keys]);
            if (parsedCursor is not [DateTimeOffset afterTimeOfDeparture, int afterId])
                throw new InvalidOperationException("Unexpected cursor format");

            return (afterTimeOfDeparture.ToUniversalTime(), afterId);
        }
    }

    private record UserRideRow(string UserId, DateTimeOffset TimeOfDeparture, int RideId);
}
