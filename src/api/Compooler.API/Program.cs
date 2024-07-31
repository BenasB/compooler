using Compooler.Application;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddDbContext<CompoolerDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("CompoolerDb")
                ?? throw new InvalidOperationException(
                    "Could not find the database connection string"
                )
        )
    )
    .AddScoped<ICompoolerDbContext, CompoolerDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CompoolerDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();

    Compooler.Domain.Entities.UserEntity.User[] users =
    [
        Compooler.Domain.Entities.UserEntity.User.Create(firstName: "Benas", lastName: "Bud"),
        Compooler.Domain.Entities.UserEntity.User.Create(firstName: "John", lastName: "Doe"),
        Compooler.Domain.Entities.UserEntity.User.Create(firstName: "Jermaine", lastName: "Cole")
    ];
    dbContext.Users.AddRange(users);
    await dbContext.SaveChangesAsync();

    var group = Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup.Create(
        route: Compooler.Domain.Entities.CommuteGroupEntity.Route.Create(
            start: Compooler
                .Domain.Entities.CommuteGroupEntity.GeographicCoordinates.Create(-43, 112)
                .Value!,
            finish: Compooler
                .Domain.Entities.CommuteGroupEntity.GeographicCoordinates.Create(42, 42)
                .Value!
        ),
        driverId: users[0].Id,
        maxPassengers: 2
    );

    group.AddPassenger(users[1].Id);
    group.AddPassenger(users[2].Id);

    dbContext.CommuteGroups.Add(group);
    await dbContext.SaveChangesAsync();
}

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CompoolerDbContext>();
    var groups = await dbContext.CommuteGroups.ToListAsync();

    var groupsOnUser = await dbContext
        .CommuteGroupsPassengers.Where(x => x.UserId == 3)
        .Select(x => EF.Property<int>(x, "CommuteGroupId"))
        .ToListAsync();

    var fullGroupsOnUser = await dbContext
        .CommuteGroups.Where(x => groupsOnUser.Contains(x.Id))
        .ToListAsync();
}

app.Run();
