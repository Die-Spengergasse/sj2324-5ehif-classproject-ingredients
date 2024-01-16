﻿using Ingredients.Model;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace Ingredients.Database;

/// <summary>
///     Handles CRUD operations with <see cref="Ingredient" />s to the database.
/// </summary>
public interface IIngredientsRepository
{
    /// <summary>
    ///     Create an ingredient in the database from the given <paramref name="ingredient" />.
    /// </summary>
    /// <remarks>This will fail if an ingredient with the same name already exists.</remarks>
    /// <param name="ingredient">The ingredient to create.</param>
    /// <returns></returns>
    public Task<string> CreateIngredient(Ingredient ingredient);

    /// <summary>
    ///     Get the ingredient with the given <paramref name="id" />.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The <see cref="Ingredient" />, null if not found</returns>
    public Task<Ingredient?> GetIngredient(string id);

    /// <summary>
    ///     Get a list of <see cref="Ingredient" />s that have a matching starting name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<IEnumerable<Ingredient>> GetIngredientsWithMatchingName(string name);

    /// <summary>
    ///     Replace the <see cref="Ingredient" /> with the given <paramref name="id" /> with the passed new
    ///     <paramref name="ingredient" />.
    /// </summary>
    /// <returns></returns>
    public Task<Ingredient?> UpdateIngredient(string id, Ingredient ingredient);

    /// <summary>
    ///     Delete the <see cref="Ingredient" /> with the given <paramref name="id" />.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The removed <see cref="Ingredient" />, null if not found</returns>
    public Task<Ingredient?> DeleteIngredient(string id);
}

public class IngredientsRepository : IIngredientsRepository
{
    private readonly IDriver _driver;

    public IngredientsRepository(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<string> CreateIngredient(Ingredient ingredient)
    {
        var parameters = ParameterSerializer.ToDictionary(ingredient);
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "CREATE (i:Ingredient $Object) " +
                    "RETURN id(i)",
                    new { Object = parameters });

                var single = await result.SingleAsync();
                return single[0].As<string>();
            });

        return res!;
    }

    public async Task<Ingredient?> GetIngredient(string id)
    {
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async rx =>
            {
                var result = await rx.RunAsync(
                    "MATCH (n:Ingredient) " +
                    $"WHERE id(n) = {id} " +
                    "RETURN n"
                );

                var single = await result.SingleAsync();
                return single[0];
            });

        // WOW! I never thought code this terrible could exist yet here we are. 
        var node = res as INode;
        var str = JsonConvert.SerializeObject(node.Properties);
        var ingredient = JsonConvert.DeserializeObject<Ingredient>(str);
        return ingredient;
    }

    public async Task<IEnumerable<Ingredient>> GetIngredientsWithMatchingName(string name)
    {
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async rx =>
            {
                var result = await rx.RunAsync(
                    "MATCH (n:Ingredient) " +
                    $"WHERE n.Name = \"{name}\" " +
                    "RETURN n"
                    );

                var fetchAsync = await result.ToListAsync();
                return fetchAsync;
            });

        // WOW! I never thought code this terrible could exist yet here we are.
        //      => and yet if it works it works
        var nodes = res.Select(n => (res[0].Values.Values.ToList()[0] as INode).Properties);
        
        var ings = new List<Ingredient>();
        foreach (var v in nodes)
        {
            var propertyStr =  JsonConvert.SerializeObject(v);
            ings.Add(JsonConvert.DeserializeObject<Ingredient>(propertyStr));
        }
        return ings;
    }

    /// <summary>
    ///   Replace the <see cref="Ingredient" /> with the given <paramref name="id" /> with
    ///   the passed new <paramref name="ingredient" />.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public async Task<Ingredient?> UpdateIngredient(string id, Ingredient ingredient)
    {
        await using var session = _driver.AsyncSession();
        var updated = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "MATCH (n:Ingredient) " +
                    $"WHERE id(n) = {id} " +
                    "SET n = $Object " +
                    "RETURN n",
                    new { Object = ingredient });

                var single = await result.SingleAsync();
                return single[0];
            });

        return (Ingredient?)updated;
    }

    /// <summary>
    ///    Delete the <see cref="Ingredient" /> with the given <paramref name="id" />.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Ingredient?> DeleteIngredient(string id)
    {
        await using var session = _driver.AsyncSession();
        var deleted = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "MATCH (n:Ingredient) " +
                    $"WHERE id(n) = {id} " +
                    "DELETE n " +
                    "RETURN n");

                var single = await result.SingleAsync();
                return single[0];
            });
        
        return (Ingredient?)deleted;
    }
}