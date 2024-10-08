using HotChocolate.Execution.Configuration;

namespace Compooler.API.Extensions;

public static class HotChocolateConventions
{
    public static IRequestExecutorBuilder AddCompoolerConventions(
        this IRequestExecutorBuilder builder
    ) =>
        builder
            .ModifyPagingOptions(options =>
            {
                options.AllowBackwardPagination = false;
                options.RequirePagingBoundaries = false;
            })
            .AddGlobalObjectIdentification()
            .AddAuthorization()
            .AddMutationConventions(applyToAllMutations: true);
}
