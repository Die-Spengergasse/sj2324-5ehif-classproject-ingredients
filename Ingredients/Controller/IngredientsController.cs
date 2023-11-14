using Ingredients.Database;
using Microsoft.AspNetCore.Mvc;

namespace Ingredients.Controller;


[Route("api/[controller]")]
[ApiController]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientsRepository _ingredientsRepository;

    public IngredientsController(IIngredientsRepository ingredientsRepository)
    {
        _ingredientsRepository = ingredientsRepository;
    }
}