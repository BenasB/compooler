using Compooler.Domain;
using Compooler.Domain.Entities.UserEntity;

namespace Compooler.Application.Commands;

public record CreateUserCommand(string FirstName, string LastName);

public class CreateUserCommandHandler(ICompoolerDbContext dbContext)
    : ICommandHandler<CreateUserCommand, User>
{
    public async Task<Result<User>> HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var newUser = User.Create(command.FirstName, command.LastName);
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync(ct);

        return Result<User>.Success(newUser);
    }
}
