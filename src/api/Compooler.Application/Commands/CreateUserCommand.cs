using Compooler.Domain;
using Compooler.Domain.Entities.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Commands;

public record CreateUserCommand(string Id, string FirstName, string LastName);

public class CreateUserCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<CreateUserCommand, User>
{
    public async Task<Result<User>> HandleAsync(
        CreateUserCommand command,
        CancellationToken ct = default
    )
    {
        var existingUser = await dbContext.Users.FindAsync([command.Id], cancellationToken: ct);

        if (existingUser != null)
            return new EntityAlreadyExistsError<User, string>(command.Id);

        var newUser = User.Create(command.Id, command.FirstName, command.LastName);
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(ct);

        return newUser;
    }
}
