using Compooler.Domain;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record RemoveUserCommand(int Id);

public class RemoveUserCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<RemoveUserCommand, User>
{
    public async Task<Result<User>> HandleAsync(
        RemoveUserCommand command,
        CancellationToken ct = default
    )
    {
        var userToRemove = await dbContext.Users.FindAsync([command.Id], cancellationToken: ct);

        if (userToRemove is null)
            return new EntityNotFoundError<User>(command.Id);

        dbContext.Users.Remove(userToRemove);
        await dbContext.SaveChangesAsync(ct);

        return userToRemove;
    }
}
