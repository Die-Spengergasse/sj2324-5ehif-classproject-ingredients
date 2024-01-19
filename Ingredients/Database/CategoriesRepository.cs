﻿using Neo4j.Driver;
using Category = Ingredients.Model.Category;
using Ingredient = Ingredients.Model.Ingredient;
using Newtonsoft.Json;

namespace Ingredients.Database;

public interface ICategoriesRepository
{
    public Task<string> CreateCategory(Category category);

    public Task<Category?> GetCategory(string id);

    public Task<IEnumerable<Category>> GetCategoriesWithMatchingName(string name);

    public Task<IEnumerable<Category>> GetAllCategories();

    public Task UpdateCategory(string id, Category category);

    public Task<Category?> DeleteCategory(string id);

    Task<IEnumerable<Ingredient>> GetIngredientsForCategory(string categoryId);

    Task AddEdgeCategoryToCategory(string idA, string idB);
    Task RemoveEdgeCategoryToCategory(string idA, string idB);
    Task UpdateEdgeCategoryToCategory(string oldIdA, string oldIdB, string newIdA, string newIdB);
    Task AddEdgeCategoryToIngredient(string idCategory, string idIngredient);
    Task RemoveEdgeCategoryToIngredient(string idCategory, string idIngredient);
    Task UpdateEdgeCategoryToIngredient(string oldIdCategory, string oldIdIngredient, string newIdCategory, string newIdIngredient);
}

public class CategoriesRepository : ICategoriesRepository
{
    private readonly IDriver _driver;

    public CategoriesRepository(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<string> CreateCategory(Category category)
    {
        var parameters = ParameterSerializer.ToDictionary(category);
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "CREATE (c:Category $params) " +
                    "RETURN id(c)",
                    new { Object = parameters });

                var single = await result.SingleAsync();
                return single[0].As<string>();
            });

        return res!;
    }


    public async Task<Category?> GetCategory(string id)
    {
        await using var session = _driver.AsyncSession();
        var res = await session.ExecuteWriteAsync(
            async rx =>
            {
                var result = await rx.RunAsync(
                    "MATCH (c:Category) " +
                    $"WHERE id(c) = {id} " +
                    "RETURN c");

                var single = await result.SingleAsync();
                return single[0];
            });

        if (res is not INode node)
        {
            return null; 
        }

        var str = JsonConvert.SerializeObject(node.Properties);
        var category = JsonConvert.DeserializeObject<Category>(str);
        return category;
    }


    public async Task<IEnumerable<Category>> GetCategoriesWithMatchingName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        }

        const string query = "MATCH (c:Category) WHERE c.Name STARTS WITH $name RETURN c";
        var parameters = new { name };

        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.ToListAsync();
        });

        return result.Select(record =>
        {
            var node = record["c"].As<INode>();
            var id = node.ElementId.As<string>();
            var categoryName = node.Properties["Name"].As<string>();

            return new Category(id, categoryName);
        });
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        const string query = "MATCH (c:Category) RETURN c";
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query);
            return await cursor.ToListAsync();
        });

        return result.Select(record =>
        {
            var node = record["c"].As<INode>();
            var id = node.ElementId.As<string>();
            var categoryName = node.Properties["Name"].As<string>();

            return new Category(id, categoryName);
        });
    }

    public async Task UpdateCategory(string id, Category category)
    {
        var parameters = new { id, category.Name };

        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx =>
            {
                await tx.RunAsync(
                    "MATCH (c:Category) " +
                    "WHERE id(c) = $id " +
                    "SET c.Name = $category.Name",
                    parameters);
            });
    }

    public async Task<Category?> DeleteCategory(string id)
    {
        var parameters = new { id };

        await using var session = _driver.AsyncSession();
        IResultCursor cursor = await session.WriteTransactionAsync(
            async tx =>
            {
                return await tx.RunAsync(
                    "MATCH (c:Category) " +
                    "WHERE id(c) = $id " +
                    "DELETE c " +
                    "RETURN c",
                    parameters);
            });

        var record = await cursor.SingleAsync();
        if (record == null)
        {
            return null;
        }

        var node = record[0].As<INode>();
        var str = JsonConvert.SerializeObject(node.Properties);
        var category = JsonConvert.DeserializeObject<Category>(str);
        return category;
    }

    public async Task<IEnumerable<Ingredient>> GetIngredientsForCategory(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            throw new ArgumentException("Category ID cannot be null or whitespace.", nameof(categoryId));
        }

        const string query =
            """
                    MATCH (c:Category)-[:CONTAINS]->(i:Ingredient)
                    WHERE id(c) = $categoryId
                    RETURN i
            """;

        var parameters = new { categoryId };

        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.ToListAsync();
        });

        return result.Select(record =>
        {
            var node = record["i"].As<INode>();
            var id = node.ElementId.As<string>();
            var name = node.Properties["Name"].As<string>();
            var carbs = node.Properties["CarbohydratesInGram"].As<double>();
            var fats = node.Properties["FatsInGram"].As<double>();
            var proteins = node.Properties["ProteinsInGram"].As<double>();

            return new Ingredient(id, name, carbs, fats, proteins);
        });
    }

    public async Task AddEdgeCategoryToCategory(string idA, string idB)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{idA}\" }}), " +
                $"(b:Category {{ Id: \"{idB}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }
    
    public async Task RemoveEdgeCategoryToCategory(string idA, string idB)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{idA}\" }})-[r:RELATED_TO]->(b:Category {{ Id: \"{idB}\" }})" +
                $"DELETE r"
            ));
    }
    
    public async Task UpdateEdgeCategoryToCategory(string oldIdA, string oldIdB, string newIdA, string newIdB)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{oldIdA}\" }})-[r:RELATED_TO]->(b:Category {{ Id: \"{oldIdB}\" }})" +
                $"DELETE r"
            ));
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{newIdA}\" }}), " +
                $"(b:Category {{ Id: \"{newIdB}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }

    public async Task AddEdgeCategoryToIngredient(string idCategory, string idIngredient)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{idCategory}\" }}), " +
                $"(b:Ingredient {{ Id: \"{idIngredient}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }
    
    public async Task RemoveEdgeCategoryToIngredient(string idCategory, string idIngredient)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{idCategory}\" }})-[r:RELATED_TO]->(b:Ingredient {{ Id: \"{idIngredient}\" }})" +
                $"DELETE r"
            ));
    }

    public async Task UpdateEdgeCategoryToIngredient(string oldIdCategory, string oldIdIngredient, string newIdCategory, string newIdIngredient)
    {
        await using var session = _driver.AsyncSession();
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{oldIdCategory}\" }})-[r:RELATED_TO]->(b:Ingredient {{ Id: \"{oldIdIngredient}\" }})" +
                $"DELETE r"
            ));
        await session.ExecuteWriteAsync(
            async tx => await tx.RunAsync(
                $"MATCH (a:Category {{ Id: \"{newIdCategory}\" }}), " +
                $"(b:Ingredient {{ Id: \"{newIdIngredient}\" }})" +
                $"CREATE (a)<-[:RELATED_TO]-(b)"
            ));
    }
}