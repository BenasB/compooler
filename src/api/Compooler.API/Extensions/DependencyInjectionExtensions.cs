using System.Reflection;
using Compooler.Application;

namespace Compooler.API.Extensions;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
    {
        var assembly =
            Assembly.GetAssembly(typeof(ICommandHandler<,>))
            ?? throw new InvalidOperationException("Could not locate command handler assembly");

        var commandHandlerTypes = assembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                    )
            )
            .ToList();

        foreach (var handlerType in commandHandlerTypes)
        {
            var handlerInterface = handlerType
                .GetInterfaces()
                .First(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                );

            services.AddScoped(handlerInterface, handlerType);
        }

        return services;
    }
}
