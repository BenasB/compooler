using Compooler.Domain;
using Compooler.Domain.Entities.CommuteGroupEntity;

namespace Compooler.Application.Commands;

public class CreateCommuteGroupHandler : ICommandHandler<CreateCommuteGroupCommand, CommuteGroup>
{
    public Task<Result<CommuteGroup>> HandleAsync(CreateCommuteGroupCommand command)
    {
        throw new NotImplementedException();
    }
}
