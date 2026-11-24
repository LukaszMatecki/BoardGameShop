using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var tags = await context.Tags
            .AsNoTracking()
            .Select(t => new 
            {
                t.IdTagu,
                t.NazwaTagu
            })
            .ToListAsync();

        return Ok(tags);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tag>> GetTag(int id)
    {
        var tag = await context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTagu == id);

        if (tag is null)
            return NotFound();

        return Ok(tag);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(CreateTagDto dto)
    {
        var tag = new Tag
        {
            NazwaTagu = dto.NazwaTagu
        };

        context.Tags.Add(tag);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTag),
            new { id = tag.IdTagu },
            tag
        );
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTag(int id, CreateTagDto dto)
    {
        var tag = await context.Tags
            .FirstOrDefaultAsync(x => x.IdTagu == id);

        if (tag is null)
            return NotFound();

        tag.NazwaTagu = dto.NazwaTagu;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await context.Tags
            .FirstOrDefaultAsync(x => x.IdTagu == id);

        if (tag is null)
            return NotFound();

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        return NoContent();
    }
}