using Ingredients.Database;
using Ingredients.Model;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace Allergens.Database;

/// <summary>
///     Handles CRUD operations with <see cref="Allergen" />s to the database.
/// </summary>
public interface IAllergensRepository
{
    /// <summary>
    ///     Create an allergen in the database from the given <paramref name="allergen" />.
    /// </summary>
    /// <remarks>This will fail if an allergen with the same name already exists.</remarks>
    /// <param name="allergen">The allergen to create.</param>
    /// <returns></returns>
    public Task<string> CreateAllergen(Allergen allergen);

    /// <summary>
    ///     Get the allergen with the given <paramref name="id" />.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The <see cref="Allergen" />, null if not found</returns>
    public Task<Allergen?> GetAllergen(string id);

    /// <summary>
    ///     Get a list of <see cref="Allergen" />s that have a matching starting name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IEnumerable<Allergen>> GetAllergensWithMatchingTypeShort(string typeShort);

    /// <summary>
    ///     Replace the <see cref="Allergen" /> with the given <paramref name="id" /> with the passed new
    ///     <paramref name="allergen" />.
    /// </summary>
    /// <returns></returns>
    public Task UpdateAllergen(string id, Allergen allergen);

    /// <summary>
    ///     Delete the <see cref="Allergen" /> with the given <paramref name="id" />.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The removed <see cref="Allergen" />, null if not found</returns>
    public Task<Allergen?> DeleteAllergen(string id);

    /// <summary>
    ///     Get a list of <see cref="Ingredient" />s that have the allergen passed.
    /// </summary>
    /// <param allergenId="Id"></param>
    /// <returns></returns>
    public Task<IEnumerable<Ingredient>> GetIngredientsWithAllergen(string allergenId);
    
    /*
     * edges from here onwards
     */
    
    
    /// <summary>
    ///     adds a connection from node a to node b
    /// </summary>
    /// <param name="idA">the int id of the node the relation is supposed to start from</param>
    /// <param name="idB">the int id of the node the relation is supposed to end at</param>
    /// <returns></returns>
    public Task AddEdgeAllergenToAllergen(int idA, int idB);

    /// <summary>
    ///     add an edge from allergen to ingredient
    /// </summary>
    /// <param name="idAllergen">id of the allergen node (end of the edge)</param>
    /// <param name="idIngredient">id of the ingredient node (start of the edge)</param>
    /// <returns></returns>
    public Task AddEdgeAllergenToIngredient(int idAllergen, int idIngredient);
}

public class AllergensRepository : IAllergensRepository
{
    private readonly IDriver _driver;

    public AllergensRepository(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<string> CreateAllergen(Allergen allergen)
    {
        var parameters = ParameterSerializer.ToDictionary(allergen);
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "CREATE (i:Allergen $Object) " +
                    "RETURN id(i)",
                    new { Object = parameters });

                var single = await result.SingleAsync();
                return single[0].As<string>();
            });

        return res!;
    }

    public async Task<Allergen?> GetAllergen(string id)
    {
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async rx =>
            {
                var result = await rx.RunAsync(
                    "MATCH (n:Allergen) " +
                    $"WHERE id(n) = {id} " +
                    "RETURN n"
                );

                var single = await result.SingleAsync();
                return single[0];
            });

        // WOW! I never thought code this terrible could exist yet here we are. 
        var node = res as INode;
        var str = JsonConvert.SerializeObject(node.Properties);
        var allergen = JsonConvert.DeserializeObject<Allergen>(str);
        return allergen;
    }

    public async Task<IEnumerable<Allergen>> GetAllergensWithMatchingTypeShort(string typeShort)
    {
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async rx =>
            {
                var result = await rx.RunAsync(
                    //TODO 
                    "MATCH (n:Allergen) " +
                    $"WHERE n.TypeShort = \"{typeShort}\" " +
                    "RETURN n"
                    );

                var fetchAsync = await result.ToListAsync();
                return fetchAsync;
            });

        // WOW! I never thought code this terrible could exist yet here we are.
        //      => and yet if it works it works
        var nodes = res.Select(n => (res[0].Values.Values.ToList()[0] as INode).Properties);
        
        var ings = new List<Allergen>();
        foreach (var v in nodes)
        {
            var propertyStr =  JsonConvert.SerializeObject(v);
            ings.Add(JsonConvert.DeserializeObject<Allergen>(propertyStr));
        }
        return ings;
    }

    public async Task UpdateAllergen(string id, Allergen allergen)
    {
        var parameters = ParameterSerializer.ToDictionary(allergen);
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx =>
            {
                await tx.RunAsync(
                    "MATCH (n:Allergen) " +
                    $"WHERE n.Id = \"{id}\" " +
                    "SET n = $Object",
                    new { Object = parameters });
            });
    }
    
    public async Task<Allergen?> DeleteAllergen(string id)
        {
            await using var session = _driver.AsyncSession();
            var res = await session.ExecuteWriteAsync(
                async tx =>
                {
                    var result = await tx.RunAsync(
                        "MATCH (n:Allergen) " +
                        $"WHERE n.Id = \"{id}\" " +
                        "DETACH DELETE n " +
                        "RETURN n"
                    );
                    var single = await result.SingleAsync();
                    return single[0];
                });
            var str = JsonConvert.SerializeObject(res);
            var allergen = JsonConvert.DeserializeObject<Allergen>(str);
            return allergen;
        } 
    
    
    public Task<IEnumerable<Ingredient>> GetIngredientsWithAllergen(string allergenId)
    {
        throw new NotImplementedException();
    }

    public async Task AddEdgeAllergenToAllergen(int idA, int idB)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Allergen {{ Id: \"{idA}\" }}), " +
                $"(b:Allergen {{ Id: \"{idB}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }

    public async Task AddEdgeAllergenToIngredient(int idAllergen, int idIngredient)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Allergen {{ Id: \"{idAllergen}\" }}), " +
                $"(b:Ingredient {{ Id: \"{idIngredient}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }
}