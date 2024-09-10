using Compooler.API.Extensions;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Compooler.Persistence.DataLoaders.Entities;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
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
            .UsePaging()
            .Type<NonNullType<ListType<NonNullType<ObjectType<User>>>>>()
            .Resolve<Connection<User>>(async ctx =>
            {
                var pagingArguments = ctx.GetPagingArguments();
                var dbContext = ctx.Services.GetRequiredService<CompoolerDbContext>();
                return await dbContext
                    .Users.AsNoTracking()
                    .OrderBy(x => x.Id)
                    .ToPageAsync(pagingArguments, ctx.RequestAborted)
                    .ToConnectionAsync();
            });

        descriptor
            .Field("me")
            .Type<ObjectType<User>>()
            .Resolve(async ctx =>
            {
                var dataLoader = ctx.Services.GetRequiredService<UserByIdDataLoader>();
                var userId = ctx.GetRequiredUserId();

                return await dataLoader.LoadAsync(userId, ctx.RequestAborted);
            });
    }
}
