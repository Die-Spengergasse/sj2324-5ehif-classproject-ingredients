using Ingredients.Database;
using Ingredients.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ingredients.Controller;

[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoriesController(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "The category was successfully added", Type = typeof(string))]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        var id = await _categoriesRepository.CreateCategory(category);
        return Ok(id);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetCategory(string id)
    {
        var res = await _categoriesRepository.GetCategory(id);
        return Ok(res);
    }

    [HttpGet("getByName/{name}")]
    public async Task<IActionResult> GetCategoriesWithMatchingName(string name)
    {
        var res = await _categoriesRepository.GetCategoriesWithMatchingName(name);
        return Ok(res);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllCategories()
    {
        var res = await _categoriesRepository.GetAllCategories();
        return Ok(res);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var res = await _categoriesRepository.DeleteCategory(id);
        return Ok(res);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory(string id, Category category)
    {
        await _categoriesRepository.UpdateCategory(id, category);
        return Ok();
    }

    [HttpPut("category/{id}/ingredients")]
    public async Task<IActionResult> GetIngredientsForCategory(string id)
    {
        var res = await _categoriesRepository.GetIngredientsForCategory(id);
        return Ok(res);
    }

    [HttpPost("addEdgeCategoryToCategory")]
    public async Task<IActionResult> AddEdgeCategoryToCategory(string idA, string idB)
    {
        try
        {
            await _categoriesRepository.AddEdgeCategoryToCategory(idA, idB);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    [HttpPost("addEdgeCategoryToIngredient")]
    public async Task<IActionResult> AddEdgeCategoryToIngredient(string idCategory, string idIngredient)
    {
        try
        {
            await _categoriesRepository.AddEdgeCategoryToIngredient(idCategory, idIngredient);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
}