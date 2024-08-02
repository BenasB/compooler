using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.API.Types.Queries;

public class UserQueries : ObjectTypeExtension
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);

        descriptor
            .Field("users")
            .Type<NonNullType<ListType<NonNullType<ObjectType<User>>>>>()
            .Resolve(ctx =>
            {
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return dbContext.Users.AsNoTracking().ToListAsync(ctx.RequestAborted);
            });
    }
}
