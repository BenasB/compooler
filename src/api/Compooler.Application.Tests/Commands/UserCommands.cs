using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Tests.Commands;

[Collection(ApplicationTestsCollection.Name)]
public class UserCommandsTests(ApplicationFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    [Fact]
    public async Task CreateUser_Succeeds()
    {
        var command = new CreateUserCommand(
            FirstName: Guid.NewGuid().ToString("N"),
            LastName: "Test"
        );
        var handler = new CreateUserCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.NotNull(await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == result.Value.Id));
    }

    [Fact]
    public async Task RemoveUser_UserExists_Succeeds()
    {
        var firstName = Guid.NewGuid().ToString("N");
        const string lastName = "Test";
        var newUser = _dbContext.Users.Add(User.Create(firstName, lastName));
        await _dbContext.SaveChangesAsync();
        var command = new RemoveUserCommand(Id: newUser.Entity.Id);
        var handler = new RemoveUserCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.Null(await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == newUser.Entity.Id));
    }

    [Fact]
    public async Task RemoveUser_UserDoesNotExist_Fails()
    {
        var command = new RemoveUserCommand(Id: -1);
        var handler = new RemoveUserCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
    }
}
