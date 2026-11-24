using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpGet("latest/{id:int}")]
    public async Task<ActionResult<CartDto>> GetLatestCart(int id)
    {


        var cart = await CartDtoQuery(context.Koszyks)
                .Where(x => x.IdUzytkownika == id)
                .OrderByDescending(x => x.DataOstatniejModyfikacji)
                .FirstOrDefaultAsync();
        if (cart is null)
            return NotFound("No cart found for this user.");

        return Ok(cart);
    }

    // [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartDto>> GetCart(int id)
    {
        var cart = await CartDtoQuery(context.Koszyks)
            .FirstOrDefaultAsync(x => x.IdKoszyka == id);

        if (cart is null) return NotFound();


        return Ok(cart);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Koszyk>> CreateCart(CreateCartDto dto)
    {
        var cart = new Koszyk
        {
            IdUzytkownika = dto.IdUzytkownika,
            DataOstatniejModyfikacji = DateTime.Now,
            PozycjaKoszykas = new List<PozycjaKoszyka>()
        };
        var productIds = dto.Pozycje.Select(p => p.IdProduktu).ToList();
        var products = await context.Produkts
            .Where(p => productIds.Contains(p.IdProduktu))
            .ToDictionaryAsync(p => p.IdProduktu);

        foreach (var itemDto in dto.Pozycje)
        {
            if (!products.TryGetValue(itemDto.IdProduktu, out var product))
            {
                return BadRequest($"Product with ID {itemDto.IdProduktu} not found.");
            }

            var cartItem = new PozycjaKoszyka
            {
                IdProduktu = product.IdProduktu,
                Ilosc = itemDto.Ilosc,
                IdKoszykaNavigation = cart
            };

            cart.PozycjaKoszykas.Add(cartItem);
        }

        context.Koszyks.Add(cart);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateCart), new { id = cart.IdKoszyka }, cart);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> ClearKoszyk(int id)
    {
        var cart = await context.Koszyks
            .Include(x => x.PozycjaKoszykas)
            .FirstOrDefaultAsync(x => x.IdKoszyka == id);

        if (cart is null) return NotFound();

        context.RemoveRange(cart.PozycjaKoszykas);
        cart.DataOstatniejModyfikacji = DateTime.Now;
        await context.SaveChangesAsync();

        return NoContent();
    }
    private static IQueryable<CartDto> CartDtoQuery(IQueryable<Koszyk> query)
    {
        return query
            .AsNoTracking()
            .Select(cart => new CartDto
            {
                IdKoszyka = cart.IdKoszyka,
                IdUzytkownika = cart.IdUzytkownika,
                DataOstatniejModyfikacji = cart.DataOstatniejModyfikacji,
                Pozycje = cart.PozycjaKoszykas.Select(pk => new CartItemDto
                {
                    IdPozycji = pk.IdPozycji,
                    IdProduktu = pk.IdProduktu,
                    Ilosc = pk.Ilosc,

                    Product = new ProductDto
                    {
                        IdProduktu = pk.IdProduktuNavigation.IdProduktu,
                        NazwaProduktu = pk.IdProduktuNavigation.NazwaProduktu,
                        Cena = pk.IdProduktuNavigation.Cena,

                        PromocjaWProc = pk.IdProduktuNavigation.PromocjaWProc,

                        Opis = pk.IdProduktuNavigation.Opis,
                        Zdjecie = pk.IdProduktuNavigation.Zdjecie,
                        DataWydania = pk.IdProduktuNavigation.DataWydania,
                        StanMagazynowy = pk.IdProduktuNavigation.StanMagazynowy,

                        AutorImie = pk.IdProduktuNavigation.IdAutoraNavigation.Imie,
                        AutorNazwisko = pk.IdProduktuNavigation.IdAutoraNavigation.Nazwisko,

                        Kategorie = pk.IdProduktuNavigation.IdKategoriis
                            .Select(k => new CategoryDto
                            {
                                IdKategorii = k.IdKategorii,
                                NazwaKategorii = k.NazwaKategorii
                            })
                            .ToList(),
                    }
                }).ToList(),

                SumaKoszyka = cart.PozycjaKoszykas.Sum(p0 => (decimal)p0.Ilosc * Math.Round(p0.IdProduktuNavigation.PromocjaWProc > 0
                    ? p0.IdProduktuNavigation.Cena * (1 - (decimal)p0.IdProduktuNavigation.PromocjaWProc / 100)
                    : p0.IdProduktuNavigation.Cena, 2))
            });
    }

}