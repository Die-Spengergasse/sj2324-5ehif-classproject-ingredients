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

    public Task UpdateAllergen(string id, Allergen allergen)
    {
        throw new NotImplementedException();
    }

    public Task<Allergen?> DeleteAllergen(string id)
    {
        throw new NotImplementedException();
    }
}