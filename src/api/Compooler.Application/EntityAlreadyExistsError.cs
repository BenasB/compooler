using Compooler.Domain;

namespace Compooler.Application;

public record EntityAlreadyExistsError<TEntity, TId>(TId Id)
    : Error("AlreadyExists", "An entity with that Id already exists")
    where TEntity : IEntity<TId>;
