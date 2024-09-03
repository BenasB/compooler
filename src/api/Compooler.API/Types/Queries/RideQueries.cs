using Compooler.API.Extensions;
using Compooler.API.Types.Queries.Inputs;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence;
using Compooler.Persistence.Configurations;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Compooler.API.Types.Queries;

[PublicAPI]
public class RideQueries : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("rides")
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Resolve<Connection<Ride>>(async ctx =>
            {
                var pagingArguments = ctx.GetPagingArguments();
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return await dbContext
                    .Rides.OrderBy(x => x.Id)
                    .AsNoTracking()
                    .ToPageAsync(pagingArguments, ctx.RequestAborted)
                    .ToConnectionAsync();
            });

        descriptor
            .Field("rankedRides")
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Argument("input", a => a.Type<NonNullType<InputObjectType<RideRankingInput>>>())
            .Resolve<List<Ride>>(async ctx =>
            {
                // TODO: Paging? Possible to implement with in memory caching of ride IDs, since we'll need to compute all of the scores anyway

                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                var input = ctx.ArgumentValue<RideRankingInput>("input");

                var startPoint = new Point(input.StartLongitude, input.StartLatitude); // TODO: validate input?
                var finishPoint = new Point(input.FinishLongitude, input.FinishLatitude);

                const int maxProximityMeters = 15000;
                const double startDistanceWeight = 0.5;
                const double finishDistanceWeight = 0.5;

                return await dbContext
                    .Rides.AsNoTracking()
                    .Where(ride =>
                        EF.Property<Point>(ride.Route.Start, RideConfiguration.PointPropertyName)
                            .IsWithinDistance(startPoint, maxProximityMeters)
                        && EF.Property<Point>(
                                ride.Route.Finish,
                                RideConfiguration.PointPropertyName
                            )
                            .IsWithinDistance(finishPoint, maxProximityMeters)
                    )
                    .Select(ride => new
                    {
                        Ride = ride,
                        StartDistance = EF.Property<Point>(
                                ride.Route.Start,
                                RideConfiguration.PointPropertyName
                            )
                            .Distance(startPoint),
                        FinishDistance = EF.Property<Point>(
                                ride.Route.Finish,
                                RideConfiguration.PointPropertyName
                            )
                            .Distance(finishPoint)
                    })
                    .Select(x => new
                    {
                        x.Ride,
                        NormalizedStartDistance = 1 - x.StartDistance / maxProximityMeters,
                        NormalizedFinishDistance = 1 - x.FinishDistance / maxProximityMeters
                    })
                    .Select(x => new
                    {
                        x.Ride,
                        Score = x.NormalizedStartDistance * startDistanceWeight
                            + x.NormalizedFinishDistance * finishDistanceWeight
                    })
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Ride)
                    .ToListAsync();
            });
    }
}
