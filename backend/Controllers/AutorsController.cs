using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutorsController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAutors()
    {
        var autors = await context.Autors
            .AsNoTracking()
            .Select(a => new 
            {
                a.IdAutora,
                a.Imie,
                a.Nazwisko
            })
            .ToListAsync();

        return Ok(autors);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Autor>> GetAutor(int id)
    {
        var autor = await context.Autors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAutora == id);

        if (autor is null)
            return NotFound();

        return Ok(autor);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Autor>> CreateAutor(AutorDto dto)
    {
        var autor = new Autor
        {
            Imie = dto.Imie,
            Nazwisko = dto.Nazwisko
        };

        context.Autors.Add(autor);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAutor),
            new { id = autor.IdAutora },
            autor
        );
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAutor(int id, AutorDto dto)
    {
        var autor = await context.Autors
            .FirstOrDefaultAsync(x => x.IdAutora == id);

        if (autor is null)
            return NotFound();

        autor.Imie = dto.Imie;
        autor.Nazwisko = dto.Nazwisko;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAutor(int id)
    {
        var autor = await context.Autors
            .FirstOrDefaultAsync(x => x.IdAutora == id);

        if (autor is null)
            return NotFound();

        context.Autors.Remove(autor);
        await context.SaveChangesAsync();

        return NoContent();
    }
}