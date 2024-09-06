using Compooler.API.Extensions;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;
using JetBrains.Annotations;

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
                    .Users.OrderBy(x => x.Id)
                    .ToPageAsync(pagingArguments, ctx.RequestAborted)
                    .ToConnectionAsync();
            });

        descriptor
            .Field("me")
            .Authorize()
            .Resolve(ctx =>
            {
                var claimsPrincipal = ctx.GetUser();

                return claimsPrincipal!.Identity!.Name;
            });
    }
}
