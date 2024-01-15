﻿using Ingredients.Database;
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
    
}