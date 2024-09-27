using Compooler.API.Extensions;
using Compooler.Domain;
using Compooler.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth();

builder.Services.AddCompoolerDbContext(builder.Configuration);

builder
    .Services.AddGraphQLServer()
    .AddCompoolerTypes()
    .AddCompoolerConventions()
    .InitializeOnStartup();

builder
    .Services.RegisterCommandHandlers()
    .AddSingleton<IDateTimeOffsetProvider, CurrentDateTimeOffsetProvider>();

var app = builder.Build();

if (!args.Contains("schema"))
    await CompoolerDbContextSetUp.InitializeAsync(app.Services, app.Environment.IsDevelopment());

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

await app.RunWithGraphQLCommandsAsync(args);
