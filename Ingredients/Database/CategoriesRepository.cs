using Neo4j.Driver;
using Category = Ingredients.Model.Category;

namespace Ingredients.Database;

public interface ICategoriesRepository
{
    public Task<string> CreateCategory(Category category);
    
    public Task<Category?> GetCategory(string id);
    
    public Task<IEnumerable<Category>> GetCategoriesWithMatchingName(string name);
    
    public Task UpdateCategory(string id, Category category);
    
    public Task<Category?> DeleteCategory(string id);
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
        throw new NotImplementedException();

    }
    
    public async Task<Category?> GetCategory(string id)
    {
        throw new NotImplementedException();

    }
    
    public async Task<IEnumerable<Category>> GetCategoriesWithMatchingName(string name)
    {
        throw new NotImplementedException();

    }
    
    public async Task UpdateCategory(string id, Category category)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Category?> DeleteCategory(string id)
    {
        throw new NotImplementedException();

    }
}