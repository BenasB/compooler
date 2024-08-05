using Compooler.API.Extensions;
using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types.Queries;

[PublicAPI]
public class CommuteGroupQueries : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("commuteGroups")
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<CommuteGroup>>>>>()
            .Resolve<Connection<CommuteGroup>>(async ctx =>
            {
                var pagingArguments = ctx.GetPagingArguments();
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return await dbContext
                    .CommuteGroups.OrderBy(x => x.Id)
                    .AsNoTracking()
                    .ToPageAsync(pagingArguments, ctx.RequestAborted)
                    .ToConnectionAsync();
            });
    }
}
