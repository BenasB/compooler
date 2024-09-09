using Compooler.Domain;

namespace Compooler.Application;

public record EntityNotFoundError<TEntity, TId>(TId Id)
    : Error("NotFound", "Could not find the specified entity")
    where TEntity : IEntity<TId>;
