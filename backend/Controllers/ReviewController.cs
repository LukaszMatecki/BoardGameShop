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
public class ReviewsController(AppDbContext context) : ControllerBase
{
    // POST: /api/reviews - Dodawanie nowej opinii przez zalogowanego użytkownika
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return Unauthorized();

        var productExists = await context.Produkts.AnyAsync(p => p.IdProduktu == dto.IdProduktu);
        if (!productExists) return NotFound("Product not found.");

        if (dto.Ocena < 1 || dto.Ocena > 5)
            return BadRequest("Rating must be between 1 and 5.");

        var review = new OpiniaProduktu
        {
            IdProduktu = dto.IdProduktu,
            IdUzytkownika = userId,
            Ocena = dto.Ocena,
            Komentarz = dto.Komentarz,
            DataWystawienia = DateTime.Now,
            // Zakładam, że chcemy, aby opinia była od razu widoczna. 
            // Jeśli wolisz moderację przed publikacją, zmień na false.
            CzyZatwierdzona = true 
        };

        context.OpiniaProduktus.Add(review);
        await context.SaveChangesAsync();

        return Ok(new { message = "Review added successfully" });
    }

    // GET: /api/reviews/all - Pobieranie wszystkich opinii dla Panelu Admina
    // Odkomentuj [Authorize(Roles = "Administrator")] jeśli masz poprawnie skonfigurowane role
    [Authorize] 
    [HttpGet("all")]
    public async Task<ActionResult<List<AdminReviewDto>>> GetAllReviews()
    {
        var reviews = await context.OpiniaProduktus
            .AsNoTracking()
            .Include(o => o.IdProduktuNavigation)
            .OrderByDescending(o => o.DataWystawienia)
            .Select(o => new AdminReviewDto
            {
                IdOpinii = o.IdOpinii,
                IdProduktu = o.IdProduktu,
                NazwaProduktu = o.IdProduktuNavigation.NazwaProduktu,
                // Ta sama logika co wyżej
                UzytkownikLogin = o.IdUzytkownikaNavigation.Imie ?? 
                                o.IdUzytkownikaNavigation.User.UserName!.Split('@', StringSplitOptions.None)[0],
                Ocena = o.Ocena,
                Komentarz = o.Komentarz,
                DataWystawienia = o.DataWystawienia
            })
            .ToListAsync();

        return Ok(reviews);
    }

    // DELETE: /api/reviews/{id} - Usuwanie opinii przez Panel Admina
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await context.OpiniaProduktus.FindAsync(id);
        if (review is null) return NotFound();

        context.OpiniaProduktus.Remove(review);
        await context.SaveChangesAsync();

        return NoContent();
    }
}