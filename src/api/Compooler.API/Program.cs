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

    var user = new Compooler.Domain.Entities.UserEntity.User
    {
        Id = 0,
        FirstName = "Benas",
        LastName = "Bud"
    };
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    var group = new Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup
    {
        Id = 0,
        Route = new Compooler.Domain.Entities.CommuteGroupEntity.Route
        {
            Start = Compooler
                .Domain.Entities.CommuteGroupEntity.GeographicCoordinates.Create(-43, 112)
                .Value!,
            Finish = Compooler
                .Domain.Entities.CommuteGroupEntity.GeographicCoordinates.Create(42, 42)
                .Value!
        },
        DriverId = user.Id,
        MaxPassengers = 2
    };

    dbContext.CommuteGroups.Add(group);
    await dbContext.SaveChangesAsync();
}

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ICompoolerDbContext>();
    var groups = await dbContext.CommuteGroups.ToListAsync();
}

app.Run();
