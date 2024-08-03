using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types.Queries;

[PublicAPI]
public class UserQueries : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("users")
            .Type<NonNullType<ListType<NonNullType<ObjectType<User>>>>>()
            .Resolve<IReadOnlyList<User>>(async ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return await dbContext.Users.AsNoTracking().ToListAsync(ctx.RequestAborted);
            });
    }
}
