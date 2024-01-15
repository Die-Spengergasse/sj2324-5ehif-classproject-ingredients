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
        services.AddSingleton<IAllergensRepository, AllergensRepository>();
    }

    public void ConfigureHost(ConfigureHostBuilder host, IConfiguration configuration)
    {
    }

    public async Task Configure(WebApplication app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        var cultureInfo = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        // TODO: Make sure to only use like this in development  
        var insertDataFromCsv = Configuration.GetValue<bool>("InsertDataFromCsv");
        if (insertDataFromCsv)
        {
            var driver = serviceProvider.GetRequiredService<IDriver>();
            var ingredientsRepository = serviceProvider.GetRequiredService<IIngredientsRepository>();
            var insertDataCSV = new InsertDataCSV(driver);
            await insertDataCSV.InsertDataFromCsv(ingredientsRepository, "ingredients.csv");
        }

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