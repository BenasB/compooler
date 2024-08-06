using Compooler.Domain;

namespace Compooler.Application;

public record EntityNotFoundError<T>(int Id)
    : Error("NotFound", "Could not find the specified entity")
    where T : IEntity;
