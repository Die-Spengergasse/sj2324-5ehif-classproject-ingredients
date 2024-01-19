using Ingredients.Database;
using Ingredients.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ingredients.Controller;

[Route("[controller]")]
[ApiController]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientsRepository _ingredientsRepository;

    public IngredientsController(IIngredientsRepository ingredientsRepository)
    {
        _ingredientsRepository = ingredientsRepository;
    }

    /// <summary>
    /// Create the given <see cref="Ingredient"/> on the database.
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "The ingredient was successfully added", Type = typeof(string))]
    public async Task<IActionResult> CreateIngredient(Ingredient ingredient)
    {
        var id = await _ingredientsRepository.CreateIngredient(ingredient);
        return Ok(id);
    }

    /// <summary>
    /// Get the <see cref="Ingredient"/> with the given id. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetIngredient(string id)
    {
        var res = await _ingredientsRepository.GetIngredient(id);
        return Ok(res);
    }

    /// <summary>
    /// Get an <see cref="Ingredient"/> with given name. 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>List of <see cref="Ingredients"/></returns>
    [HttpGet("getByName/{name}")]
    public async Task<IActionResult> GetIngredientsWithMatchingName(string name)
    {
        var res = await _ingredientsRepository.GetIngredientsWithMatchingName(name);
        return Ok(res);
    }
    
    /// <summary>
    /// Update the <see cref="Ingredient"/> with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient(string id, [FromBody] Ingredient ingredient)
    {
        var res = await _ingredientsRepository.UpdateIngredient(id, ingredient);
        return Ok(res);
    }
    
    /// <summary>
    /// Delete the <see cref="Ingredient"/> with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(string id)
    {
        var res = await _ingredientsRepository.DeleteIngredient(id);
        return Ok(res);
    }
}