using Compooler.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCompoolerDbContext(builder.Configuration);

var app = builder.Build();

await CompoolerDbContextSetUp.InitializeAsync(app.Services, app.Environment.IsDevelopment());

app.MapGet("/", () => "Hello World!");

app.Run();
