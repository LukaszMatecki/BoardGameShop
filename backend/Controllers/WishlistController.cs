using System.Security.Claims;
using API.Data;
using API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class WishlistController(AppDbContext context) : ControllerBase
{
    [HttpGet("wishlist")]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userDetails = await context.UzytkownikDetails
            .Include(ud => ud.IdProduktus)
            .ThenInclude(p => p.IdAutoraNavigation)
            .FirstOrDefaultAsync(ud => ud.IdUzytkownika == userId);

        if (userDetails == null)
            return NotFound("Nie znaleziono szczegółów użytkownika.");

        var wishlistItems = userDetails.IdProduktus
            .Select(p => new WishlistItemDto
            {
                ProductId = p.IdProduktu,
                NazwaProduktu = p.NazwaProduktu,
                Zdjecie = p.Zdjecie,
                AutorImie = p.IdAutoraNavigation?.Imie ?? "Nieznany",
                AutorNazwisko = p.IdAutoraNavigation?.Nazwisko ?? "Autor",
                Cena = p.Cena
            })
            .ToList();

        return Ok(wishlistItems);
    }

    [HttpPost("wishlist/{productId:int}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userDetails = await context.UzytkownikDetails
            .Include(ud => ud.IdProduktus)
            .FirstOrDefaultAsync(ud => ud.IdUzytkownika == userId);

        if (userDetails == null)
            return NotFound("Nie znaleziono szczegółów użytkownika.");

        var product = await context.Produkts.FindAsync(productId);
        if (product == null) return NotFound("Nie znaleziono produktu.");

        if (!userDetails.IdProduktus.Any(p => p.IdProduktu == productId))
        {
            userDetails.IdProduktus.Add(product);
            await context.SaveChangesAsync();
        }
        return Ok(new { message = "Dodano do ulubionych" });
    }

    [HttpDelete("wishlist/{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var userDetails = await context.UzytkownikDetails
            .Include(ud => ud.IdProduktus)
            .FirstOrDefaultAsync(ud => ud.IdUzytkownika == userId);

        if (userDetails == null)
            return NotFound("Nie znaleziono szczegółów użytkownika.");

        var productToRemove = userDetails.IdProduktus.FirstOrDefault(p => p.IdProduktu == productId);

        if (productToRemove != null)
        {
            userDetails.IdProduktus.Remove(productToRemove);
            await context.SaveChangesAsync();
        }
        return NoContent();
    }
}