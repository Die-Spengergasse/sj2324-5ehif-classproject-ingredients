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
    /// Update an <see cref="Ingredient"/> with given Id. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ingredient"></param>
    /// <returns>Id of updated <see cref="Ingredient"/></returns>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateIngredient(string id, Ingredient ingredient)
    {
        await _ingredientsRepository.UpdateIngredient(id, ingredient);
        return Ok(id);
    }
    
    /// <summary>
    /// Delete an <see cref="Ingredient"/> with given Id. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Deleted <see cref="Ingredient"/></returns>
    [HttpDelete("deleteById/{id}")]
    public async Task<IActionResult> DeleteIngredient(string id)
    {
        var deletedIngredient = await _ingredientsRepository.DeleteIngredient(id);
        if (deletedIngredient == null)
        {
            return NotFound();
        }
        return Ok(deletedIngredient);
    } 
    
    
    
    /*
     * edges from here on down
     */

    [HttpPost("AddEdgeIngredientToIngredient")]
    public async Task<IActionResult> AddEdgeIngredientToIngredient(string idA, string idB)
    {
        try
        {
            await _ingredientsRepository.AddEdgeIngredientToIngredient(idA, idB);
        }
        catch (Exception e)
        {
            //Console.WriteLine(e);
            return BadRequest();
        }
        return Ok();
    }

    [HttpPost("AddEdgeIngredientToAllergen")]
    public async Task<IActionResult> AddEdgeIngredientToAllergen(string idAllergen, string idIngredient)
    {
        try
        {
            await _ingredientsRepository.AddEdgeIngredientToAllergen(idAllergen, idIngredient);
        }
        catch (Exception e)
        {
            //Console.WriteLine(e);
            return BadRequest();
        }
        return Ok();
    }
}