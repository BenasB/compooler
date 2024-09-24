using System.Collections.Immutable;
using System.Linq.Expressions;
using Compooler.Domain;
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
    public static Task<
        IReadOnlyDictionary<string, Page<(DateTimeOffset TimeOfDeparture, int RideId)>>
    > GetUpcomingRideIdsByUserIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        IDateTimeOffsetProvider dateTimeOffsetProvider,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken
    ) =>
        GetRideIdsByUserIdAsync(
            defaultPaginationFilter: """ "TOD" > @currentTime""",
            cursorPaginationFilter: """("TOD", "RideId") > (@afterTimeOfDeparture, @afterId)""",
            partitionOrder: """ "TOD", "RideId" """,
            keys,
            dbContext,
            dateTimeOffsetProvider,
            pagingArguments,
            cancellationToken
        );

    [DataLoader]
    public static Task<
        IReadOnlyDictionary<string, Page<(DateTimeOffset TimeOfDeparture, int RideId)>>
    > GetHistoricalRideIdsByUserIdAsync(
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        IDateTimeOffsetProvider dateTimeOffsetProvider,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken
    ) =>
        GetRideIdsByUserIdAsync(
            defaultPaginationFilter: """ "TOD" < @currentTime""",
            cursorPaginationFilter: """
             "TOD" < @afterTimeOfDeparture OR
             ("TOD" = @afterTimeOfDeparture AND "RideId" > @afterId)
            """,
            partitionOrder: """ "TOD" DESC, "RideId" """,
            keys,
            dbContext,
            dateTimeOffsetProvider,
            pagingArguments,
            cancellationToken
        );

    private static async Task<
        IReadOnlyDictionary<string, Page<(DateTimeOffset TimeOfDeparture, int RideId)>>
    > GetRideIdsByUserIdAsync(
        string defaultPaginationFilter,
        string cursorPaginationFilter,
        string partitionOrder,
        IReadOnlyList<string> keys,
        CompoolerDbContext dbContext,
        IDateTimeOffsetProvider dateTimeOffsetProvider,
        PagingArguments pagingArguments,
        CancellationToken cancellationToken
    )
    {
        if (pagingArguments.First == null)
            throw new ArgumentException(
                $"'{nameof(pagingArguments.First)}' Argument must be specified",
                nameof(pagingArguments)
            );

        if (pagingArguments.Last != null || pagingArguments.Before != null)
            throw new ArgumentException(
                $"'{nameof(pagingArguments.Last)}' and '{nameof(pagingArguments.Before)}' not supported,"
                    + $"use '{nameof(pagingArguments.First)}' and '{nameof(pagingArguments.After)}'",
                nameof(pagingArguments)
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
        string paginationFilter;

        var cursorKeys = GetCursorKeys(
            dbContext
                .Database.SqlQuery<(DateTimeOffset TimeOfDeparture, int RideId)>($"")
                .OrderBy(r => r.TimeOfDeparture)
                .ThenBy(r => r.RideId)
                .Expression
        );

        if (pagingArguments.After != null)
        {
            var parsedCursor = CursorParser.Parse(pagingArguments.After, cursorKeys);
            if (parsedCursor is not [DateTimeOffset afterTimeOfDeparture, int afterId])
                throw new InvalidOperationException("Unexpected cursor format");

            paginationFilter = cursorPaginationFilter;
            sqlParameters.Add(
                new NpgsqlParameter(
                    "afterTimeOfDeparture",
                    value: afterTimeOfDeparture.ToUniversalTime()
                )
            );
            sqlParameters.Add(new NpgsqlParameter("afterId", value: afterId));
        }
        else
        {
            paginationFilter = defaultPaginationFilter;
            sqlParameters.Add(
                new NpgsqlParameter(
                    "currentTime",
                    value: dateTimeOffsetProvider.Now.ToUniversalTime()
                )
            );
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
                    ROW_NUMBER() OVER (PARTITION BY "UserId" ORDER BY {partitionOrder}) AS "RowNumber"
                FROM CombinedRides
                WHERE {paginationFilter}
            )
            SELECT "UserId", "TOD" AS "TimeOfDeparture", "RideId"
            FROM PartitionedRides
            WHERE "RowNumber" <= @first + 1;
            """;

        sqlParameters.Add(new NpgsqlParameter("userIds", keys));
        sqlParameters.Add(new NpgsqlParameter("first", value: pagingArguments.First.Value));

        var rows = await dbContext
            .Database.SqlQueryRaw<UserOrderedRideRow>(
                query,
                parameters: sqlParameters.ToArray<object>()
            )
            .ToListAsync(cancellationToken: cancellationToken);

        return rows.GroupBy(row => row.UserId, row => (row.TimeOfDeparture, row.RideId))
            .Select(x => (x.Key, Items: x.ToImmutableArray()))
            .ToDictionary(
                x => x.Key,
                x => new Page<(DateTimeOffset TimeOfDeparture, int RideId)>(
                    items: x.Items.Length > pagingArguments.First.Value
                        ? x.Items[..pagingArguments.First.Value]
                        : x.Items,
                    hasNextPage: x.Items.Length > pagingArguments.First.Value,
                    hasPreviousPage: default,
                    createCursor: value => CursorFormatter.Format(value, cursorKeys)
                )
            );
    }

    private record UserOrderedRideRow(string UserId, DateTimeOffset TimeOfDeparture, int RideId);

    private static CursorKey[] GetCursorKeys(Expression expression)
    {
        var cursorKeyParser = new CursorKeyParser();
        cursorKeyParser.Visit(expression);
        return [.. cursorKeyParser.Keys];
    }
}
