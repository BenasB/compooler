using HotChocolate.Resolvers;

namespace Compooler.API;

public interface IMappableTo<out T>
{
    T Map(IResolverContext context);
}
