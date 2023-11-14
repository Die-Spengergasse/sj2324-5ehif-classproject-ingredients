using Ingredients.Database;
using Ingredients.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace Ingredients;

public class Startup
{
    public Startup(IConfigurationRoot configuration)
    {
        Configuration = configuration;
    }

    private IConfigurationRoot Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services, IWebHostEnvironment environment)
    {
        var neo4JSettings = new Neo4JOptions();
        Configuration.GetSection("Neo4JOptions").Bind(neo4JSettings);

        services.AddControllers();
        services.AddSingleton<IDriver>(_ => GraphDatabase
            .Driver(neo4JSettings.Neo4JConnection, AuthTokens.Basic(neo4JSettings.Neo4JUser, neo4JSettings.Neo4JPassword)));
        
        services.AddScoped<INeo4JDataAccess, Neo4JDataAccess>();

        services.AddTransient<IIngredientsRepository, IngredientsRepository>();
    }

    public void ConfigureHost(ConfigureHostBuilder host, IConfiguration configuration)
    {
    }

    public Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UsePathBase("/api");
        
        app.MapControllers();
        return Task.CompletedTask;
    }
}