using Compooler.API.Extensions;
using Compooler.Domain;
using Compooler.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth();

builder
    .Services.AddCompoolerDbContext(builder.Configuration)
    .AddSingleton<IDateTimeOffsetProvider, CurrentDateTimeOffsetProvider>();

builder
    .Services.AddGraphQLServer()
    .AddCompoolerTypes()
    .AddCompoolerConventions()
    .InitializeOnStartup();

builder.Services.RegisterCommandHandlers();

var app = builder.Build();

await CompoolerDbContextSetUp.InitializeAsync(app.Services, app.Environment.IsDevelopment());

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();
