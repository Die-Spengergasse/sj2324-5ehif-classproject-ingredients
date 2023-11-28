using Ingredients.Database;
using Ingredients.Options;
using Microsoft.OpenApi.Models;
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

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ingredients API", Version = "v1" });

            var filePath = Path.Combine(AppContext.BaseDirectory, "Ingredients.xml");
            c.IncludeXmlComments(filePath);
        });
        
        services.AddControllers();
        services.AddSingleton<IDriver>(_ => GraphDatabase.Driver(neo4JSettings.Neo4JConnection, AuthTokens.None));
        services.AddSingleton<IIngredientsRepository, IngredientsRepository>();
    }

    public void ConfigureHost(ConfigureHostBuilder host, IConfiguration configuration)
    {
    }

    public Task Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UsePathBase("/api");
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Ingredients API V1");
            c.RoutePrefix = "swagger";
        });
        
        app.MapControllers();
        return Task.CompletedTask;
    }
}