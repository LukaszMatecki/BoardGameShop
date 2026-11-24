using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountCodesController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateDiscountCodes([FromBody] List<AddDiscountCodeDto> dtos)
    {
        if (dtos == null || dtos.Count == 0)
            return BadRequest("Payload cannot be empty.");

        var groupedByCode = dtos.GroupBy(d => d.DiscountCode.Trim().ToUpper());

        foreach (var group in groupedByCode)
        {
            var codeString = group.Key;
            var percentage = group.First().DiscountPercentage;

            var discountCode = await context.KodyPromocyjne
                .Include(k => k.Uzytkownicy)
                .FirstOrDefaultAsync(k => k.Kod.ToUpper() == codeString);

            if (discountCode == null)
            {
                discountCode = new KodPromocyjny
                {
                    Kod = group.First().DiscountCode.Trim(),
                    ZnizkaProcentowa = percentage,
                };
                context.KodyPromocyjne.Add(discountCode);
            }

            foreach (var item in group)
            {
                var userDetail = await context.Set<UzytkownikDetail>()
                    .FirstOrDefaultAsync(u => u.IdUzytkownika == item.UserId);

                if (userDetail != null)
                {
                    if (!discountCode.Uzytkownicy.Any(u => u.IdUzytkownika == item.UserId))
                    {
                        discountCode.Uzytkownicy.Add(userDetail);
                    }
                }
            }
        }

        await context.SaveChangesAsync();
        return Ok(new { message = "code was succesfully saved." });
    }

    [Authorize]
    [HttpGet("validate/{code}")]
    public async Task<ActionResult<DiscountCodeDto>> ValidateDiscountCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("Code cannot be empty.");

        var normalizedCode = code.Trim().ToUpper();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var discountCode = await context.KodyPromocyjne
            .Where(k => k.Kod.ToUpper().Equals(normalizedCode))
            .Where(k => k.Uzytkownicy.Any(u => u.IdUzytkownika == userId))
            .FirstOrDefaultAsync();

        if (discountCode == null)
            return BadRequest("Code is invalid.");


        var result = new DiscountCodeDto
        {
            IdKodu = discountCode.IdKodu,
            Kod = discountCode.Kod,
            ZnizkaProcentowa = discountCode.ZnizkaProcentowa
        };

        return Ok(result);
    }

    [Authorize]
    [HttpGet("my-codes")]
    public async Task<ActionResult<IEnumerable<DiscountCodeDto>>> GetMyDiscountCodes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var codes = await context.KodyPromocyjne
            .Where(k => k.Uzytkownicy.Any(u => u.IdUzytkownika == userId))
            .Select(k => new DiscountCodeDto
            {
                IdKodu = k.IdKodu,
                Kod = k.Kod,
                ZnizkaProcentowa = k.ZnizkaProcentowa
            })
            .ToListAsync();

        return Ok(codes);
    }
}