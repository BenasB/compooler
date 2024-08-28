using Compooler.Application;

namespace Compooler.API.Types.Mutations;

/// <summary>
/// <para>Provides mutation resolving pipeline for a typical Compooler mutation with explicit error typing (CLR and GraphQL)</para>
/// Get the input object -> Map input to command -> Call handler -> Return successful result or one of the errors
/// </summary>
public static class CompoolerMutation
{
    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<TInput, TCommand, TResult>(
        this IObjectFieldDescriptor descriptor
    )
        where TInput : IMappableTo<TCommand> =>
        descriptor
            .Argument("input", x => x.Type<InputObjectType<TInput>>())
            .Resolve<FieldResult<TResult>>(async ctx =>
            {
                var input = ctx.ArgumentValue<TInput>("input");
                var handler = ctx.Services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
                var command = input.Map();
                var result = await handler.HandleAsync(command, ctx.RequestAborted);

                return !result.IsFailed ? result.Value : new FieldResult<TResult>(result.Error);
            });
}
