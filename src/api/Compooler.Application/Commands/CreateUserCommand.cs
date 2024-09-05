using Compooler.Domain;
using Compooler.Domain.Entities.UserEntity;

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
        var newUser = User.Create(command.Id, command.FirstName, command.LastName);
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(ct);

        return newUser;
    }
}
