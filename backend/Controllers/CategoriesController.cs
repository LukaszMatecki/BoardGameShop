using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await context.Kategoria
            .AsNoTracking()
            .Select(k => new 
            {
                k.IdKategorii,
                k.NazwaKategorii
            })
            .ToListAsync();

        return Ok(categories);
    }
}