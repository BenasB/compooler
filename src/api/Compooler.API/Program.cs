using Compooler.API.Extensions;
using Compooler.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCompoolerDbContext(builder.Configuration);
builder
    .Services.AddGraphQLServer()
    .AddCompoolerTypes()
    .AddCompoolerConventions()
    .InitializeOnStartup();

builder.Services.RegisterCommandHandlers();

var app = builder.Build();

await CompoolerDbContextSetUp.InitializeAsync(app.Services, app.Environment.IsDevelopment());

app.MapGraphQL();

app.Run();
