using Compooler.API.Extensions;
using Compooler.API.Types.Queries.Inputs;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence;
using Compooler.Persistence.Queries;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types.Queries;

[PublicAPI]
public class RideQueries : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("rides")
            .Authorize()
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
            .Field("relevantRides")
            .Description("Returns rides relevant to given criteria (e.g. route location)")
            .Authorize()
            .Type<NonNullType<ListType<NonNullType<ObjectType<Ride>>>>>()
            .Argument("input", a => a.Type<NonNullType<InputObjectType<RideRelevanceInput>>>())
            .Resolve<List<Ride>>(async ctx =>
            {
                // TODO: Paging? Possible to implement with in memory caching of ride IDs, since we'll need to compute all of the scores anyway

                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                var input = ctx.ArgumentValue<RideRelevanceInput>("input");

                var startResult = GeographicCoordinates.Create(
                    latitude: input.StartLatitude,
                    longitude: input.StartLongitude
                );

                if (startResult.IsFailed)
                    throw new Exception(startResult.Error.Message);

                var finishResult = GeographicCoordinates.Create(
                    latitude: input.FinishLatitude,
                    longitude: input.FinishLongitude
                );

                if (finishResult.IsFailed)
                    throw new Exception(finishResult.Error.Message);

                return await dbContext
                    .Rides.AsNoTracking()
                    .FilterAndOrderByRelevance(
                        startResult.Value.Latitude,
                        startResult.Value.Longitude,
                        finishResult.Value.Latitude,
                        finishResult.Value.Longitude
                    )
                    .ToListAsync();
            });
    }
}
