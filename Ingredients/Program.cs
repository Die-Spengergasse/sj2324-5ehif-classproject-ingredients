using Ingredients;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services, builder.Environment);
startup.ConfigureHost(builder.Host, builder.Configuration);

var app = builder.Build();
await startup.Configure(app, builder.Environment);

app.Run();