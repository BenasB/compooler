using HotChocolate.Execution.Configuration;

namespace Compooler.API.Extensions;

public static class HotChocolateConventions
{
    public static IRequestExecutorBuilder AddCompoolerConventions(
        this IRequestExecutorBuilder builder
    ) =>
        builder.ModifyOptions(options =>
        {
            options.DefaultBindingBehavior = BindingBehavior.Explicit;
        });
}
