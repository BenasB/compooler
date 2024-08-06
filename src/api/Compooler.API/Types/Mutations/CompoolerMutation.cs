using Compooler.Application;
using HotChocolate.Resolvers;

namespace Compooler.API.Types.Mutations;

/// <summary>
/// <para>Provides mutation resolving pipeline for a typical Compooler mutation with explicit error typing (CLR and GraphQL)</para>
/// Get the input object -> Map input to command -> Call handler -> Return successful result or one of the errors
/// </summary>
public static class CompoolerMutation
{
    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<TInput, TCommand, TResult>(
        this IObjectFieldDescriptor descriptor,
        Func<TInput, TCommand> inputToCommand
    ) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    _ => throw UndeclaredErrorException
                };
            });

    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<
        TInput,
        TCommand,
        TResult,
        TError
    >(this IObjectFieldDescriptor descriptor, Func<TInput, TCommand> inputToCommand) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult, TError>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    TError e => e,
                    _ => throw UndeclaredErrorException
                };
            });

    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<
        TInput,
        TCommand,
        TResult,
        TError1,
        TError2
    >(this IObjectFieldDescriptor descriptor, Func<TInput, TCommand> inputToCommand) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult, TError1, TError2>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    TError1 e => e,
                    TError2 e => e,
                    _ => throw UndeclaredErrorException
                };
            });

    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<
        TInput,
        TCommand,
        TResult,
        TError1,
        TError2,
        TError3
    >(this IObjectFieldDescriptor descriptor, Func<TInput, TCommand> inputToCommand) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult, TError1, TError2, TError3>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    TError1 e => e,
                    TError2 e => e,
                    TError3 e => e,
                    _ => throw UndeclaredErrorException
                };
            });

    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<
        TInput,
        TCommand,
        TResult,
        TError1,
        TError2,
        TError3,
        TError4
    >(this IObjectFieldDescriptor descriptor, Func<TInput, TCommand> inputToCommand) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult, TError1, TError2, TError3, TError4>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    TError1 e => e,
                    TError2 e => e,
                    TError3 e => e,
                    TError4 e => e,
                    _ => throw UndeclaredErrorException
                };
            });

    /// <inheritdoc cref="CompoolerMutation"/>
    public static IObjectFieldDescriptor ResolveCompoolerMutation<
        TInput,
        TCommand,
        TResult,
        TError1,
        TError2,
        TError3,
        TError4,
        TError5
    >(this IObjectFieldDescriptor descriptor, Func<TInput, TCommand> inputToCommand) =>
        descriptor
            .CommonConfiguration<TInput>()
            .Resolve<FieldResult<TResult, TError1, TError2, TError3, TError4, TError5>>(async ctx =>
            {
                var result = await GetResult<TInput, TCommand, TResult>(inputToCommand, ctx);

                if (!result.IsFailed)
                    return result.Value;

                return result.Error switch
                {
                    TError1 e => e,
                    TError2 e => e,
                    TError3 e => e,
                    TError4 e => e,
                    TError5 e => e,
                    _ => throw UndeclaredErrorException
                };
            });

    private static Exception UndeclaredErrorException => new("Undeclared error");

    private static IObjectFieldDescriptor CommonConfiguration<TInput>(
        this IObjectFieldDescriptor descriptor
    ) => descriptor.Argument("input", x => x.Type<InputObjectType<TInput>>());

    private static Task<Domain.Result<TResult>> GetResult<TInput, TCommand, TResult>(
        Func<TInput, TCommand> inputToCommand,
        IResolverContext ctx
    )
    {
        var input = ctx.ArgumentValue<TInput>("input");
        var handler = ctx.Services.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        var command = inputToCommand.Invoke(input);
        return handler.HandleAsync(command, ctx.RequestAborted);
    }
}
