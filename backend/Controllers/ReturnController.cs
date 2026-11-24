using API.Data;
using System.Security.Claims;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReturnsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ReturnDto>>> GetReturns()
    {
        var returns = await ReturnDtoQuery(context.ZwrotZamowienia)
            .ToListAsync();

        return Ok(returns);
    }
    [HttpGet("Types")]
    public async Task<ActionResult<List<ReturnTypesDto>>> GetTypes()
    {
        var returns = await context.TypZgloszenia
            .AsNoTracking()
            .Select(t => new ReturnTypesDto
            {
                Id = t.IdTypuZgloszenia,
                Name = t.Nazwa
            })
            .ToListAsync();

        return Ok(returns);
    }
    [HttpGet("Statuses")]
    public async Task<ActionResult<List<ReturnTypesDto>>> GetStatuses()
    {
        var returns = await context.StatusZgloszenia
            .AsNoTracking()
            .Select(t => new ReturnTypesDto
            {
                Id = t.IdStatusuZgloszenia,
                Name = t.Nazwa
            })
            .ToListAsync();

        return Ok(returns);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnDto>> GetReturn(int id)
    {
        var returnItem = await ReturnDtoQuery(context.ZwrotZamowienia)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (returnItem is null)
            return NotFound();

        return Ok(returnItem);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReturnDto>> CreateReturn(CreateReturnDto dto)
    {
        var validationResult = await ValidateCreateReturnDto(dto);
        if (validationResult is not null)
            return validationResult;

        var newReturn = new ZwrotZamowienium
        {
            IdZamowienia = dto.IdZamowienia,
            IdPozycji = dto.IdPozycji,
            Powod = dto.Powod,
            DataZgloszenia = DateTime.Now,
            CzyZaakceptowany = false,
            IdTypuZgloszenia = dto.IdTypu,
            IdStatusuZgloszenia = 1
        };

        context.ZwrotZamowienia.Add(newReturn);
        await context.SaveChangesAsync();

        var createdReturn = await ReturnDtoQuery(context.ZwrotZamowienia)
            .FirstAsync(r => r.Id == newReturn.IdZwrotu);

        return CreatedAtAction(
            nameof(GetReturn),
            new { id = newReturn.IdZwrotu },
            createdReturn
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int statusId)
    {
        var returnItem = await context.ZwrotZamowienia
            .FirstOrDefaultAsync(z => z.IdZwrotu == id);

        if (returnItem is null)
            return NotFound();

        var statusExists = await context.StatusZgloszenia
            .AnyAsync(s => s.IdStatusuZgloszenia == statusId);

        if (!statusExists)
            return BadRequest("The specified status does not exist.");

        returnItem.IdStatusuZgloszenia = statusId;
        returnItem.CzyZaakceptowany = (statusId == 3);

        await context.SaveChangesAsync();

        return NoContent();
    }

    private static IQueryable<ReturnDto> ReturnDtoQuery(IQueryable<ZwrotZamowienium> query)
    {
        return query
            .AsNoTracking()
            .Select(z => new ReturnDto
            {
                Id = z.IdZwrotu,
                OrderId = z.IdZamowienia,
                IdPozycji = z.IdPozycji,
                DataZgloszenia = z.DataZgloszenia,
                Powod = z.Powod,
                Email = z.IdZamowieniaNavigation.Email,
                CzyZaakceptowany = z.CzyZaakceptowany,
                Typ = z.IdTypuZgloszeniaNavigation.Nazwa,
                Status = z.IdStatusuZgloszeniaNavigation.Nazwa
            });
    }

    private async Task<ActionResult?> ValidateCreateReturnDto(CreateReturnDto dto)
    {
        var itemExists = await context.PozycjaZamowienia
            .AnyAsync(p => p.IdPozycji == dto.IdPozycji && p.IdZamowienia == dto.IdZamowienia);

        if (!itemExists)
            return BadRequest("The specified product item does not exist in this order.");

        var typeExists = await context.TypZgloszenia
            .AnyAsync(t => t.IdTypuZgloszenia == dto.IdTypu);

        if (!typeExists)
            return BadRequest($"The specified return type does not exist{dto.IdTypu}.");

        return null;
    }
}