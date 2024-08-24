using Compooler.API.Extensions;
using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence;
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
    }
}
