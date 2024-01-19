using Ingredients;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services, builder.Environment);
startup.ConfigureHost(builder.Host, builder.Configuration);

var app = builder.Build();
var serviceProvider = app.Services;
await startup.Configure(app, builder.Environment, serviceProvider);

app.Run();