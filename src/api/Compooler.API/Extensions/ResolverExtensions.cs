using HotChocolate.Pagination;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;

namespace Compooler.API.Extensions;

public static class ResolverExtensions
{
    public static PagingArguments GetPagingArguments(this IResolverContext context)
    {
        var contextData = context.LocalContextData[WellKnownContextData.PagingArguments];

        if (contextData is not CursorPagingArguments cursorPagingArguments)
            throw new InvalidOperationException("Could not locate paging arguments");

        return new PagingArguments
        {
            After = cursorPagingArguments.After,
            Before = cursorPagingArguments.Before,
            First = cursorPagingArguments.First,
            Last = cursorPagingArguments.Last
        };
    }

    public static string GetRequiredUserId(this IResolverContext context) =>
        context.GetUser()?.Identity?.Name
        ?? throw new InvalidOperationException(
            "User id was not found. Are you sure the user is authenticated?"
        );
}
