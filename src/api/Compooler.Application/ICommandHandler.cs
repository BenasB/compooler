using Compooler.Domain;

namespace Compooler.Application;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command);
}
