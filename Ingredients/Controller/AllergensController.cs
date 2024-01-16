using Allergens.Database;
using Ingredients.Database;
using Ingredients.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Allergens.Controller;

[Route("[controller]")]
[ApiController]
public class AllergensController : ControllerBase
{
    private readonly IAllergensRepository _allergensRepository;

    public AllergensController(IAllergensRepository allergensRepository)
    {
        _allergensRepository = allergensRepository;
    }

    /// <summary>
    /// Create the given <see cref="Allergen"/> on the database.
    /// </summary>
    /// <param name="allergen"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "The allergen was successfully added", Type = typeof(string))]
    public async Task<IActionResult> CreateAllergen(Allergen allergen)
    {
        //TODO check if allergen is valid (e.g. not null & Typeshort is a valid char not string)
        var id = await _allergensRepository.CreateAllergen(allergen);
        return Ok(id);
    }

    /// <summary>
    /// Get the <see cref="Allergen"/> with the given id. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetAllergen(string id)
    {
        var res = await _allergensRepository.GetAllergen(id);
        return Ok(res);
    }

    /// <summary>
    /// Get an <see cref="Allergen"/> with given name. 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>List of <see cref="Allergens"/></returns>
    [HttpGet("getByName/{name}")]
    public async Task<IActionResult> GetAllergensWithMatchingTypeShort(string typeShort)
    {
        var res = await _allergensRepository.GetAllergensWithMatchingTypeShort(typeShort);
        return Ok(res);
    }

    /// <summary>
    /// Update an <see cref="Allergen"/> with given Id. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="allergen"></param>
    /// <returns>Id of updated <see cref="Allergen"/></returns>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAllergen(string id, Allergen allergen)
    {
        await _allergensRepository.UpdateAllergen(id, allergen);
        return Ok(id);
    }
    
    /// <summary>
    /// Delete an <see cref="Allergen"/> with given Id. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Deleted <see cref="Allergen"/></returns>
    [HttpDelete("deleteById/{id}")]
    public async Task<IActionResult> DeleteAllergen(string id)
    {
        var deletedAllergen = await _allergensRepository.DeleteAllergen(id);
        if (deletedAllergen == null)
        {
            return NotFound();
        }
        return Ok(deletedAllergen);
    } 
    
}