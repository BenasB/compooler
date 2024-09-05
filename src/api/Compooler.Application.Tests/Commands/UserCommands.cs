using Compooler.Application.Commands;
using Compooler.Domain.Entities.UserEntity;
using Compooler.Persistence;
using Compooler.Tests.Utilities;
using Compooler.Tests.Utilities.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Compooler.Application.Tests.Commands;

[Collection(ApplicationTestsCollection.Name)]
public class UserCommandsTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private readonly CompoolerDbContext _dbContext = new(fixture.DbContextOptions);

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbContext.DisposeAsync().AsTask();

    [Fact]
    public async Task CreateUser_Succeeds()
    {
        var command = new CreateUserCommand(
            Id: TestEntityFactory.CreateUserId(),
            FirstName: TestEntityFactory.CreateUserId(),
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
        var id = TestEntityFactory.CreateUserId();
        var firstName = TestEntityFactory.CreateUserId();
        const string lastName = "Test";
        var newUser = _dbContext.Users.Add(User.Create(id, firstName, lastName));
        await _dbContext.SaveChangesAsync();
        var command = new RemoveUserCommand(Id: id);
        var handler = new RemoveUserCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsFailed);
        Assert.Null(await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == newUser.Entity.Id));
    }

    [Fact]
    public async Task RemoveUser_UserDoesNotExist_Fails()
    {
        var nonExistentId = TestEntityFactory.CreateUserId();
        var command = new RemoveUserCommand(Id: nonExistentId);
        var handler = new RemoveUserCommandHandler(_dbContext);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsFailed);
        Assert.Equal(new EntityNotFoundError<User, string>(nonExistentId), result.Error);
    }
}
