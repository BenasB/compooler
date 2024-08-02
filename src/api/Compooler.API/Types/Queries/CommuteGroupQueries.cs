using Compooler.Domain.Entities.CommuteGroupEntity;
using Compooler.Persistence;
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
            .Type<NonNullType<ListType<NonNullType<ObjectType<CommuteGroup>>>>>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return dbContext.CommuteGroups.AsNoTracking().ToListAsync(ctx.RequestAborted);
            });
    }
}
