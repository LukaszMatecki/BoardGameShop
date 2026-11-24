using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemsController(AppDbContext context) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCartItem(CreateCartItemDto dto)
    {
        var product = await context.Produkts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdProduktu == dto.IdProduktu);

        if (product == null)
            return NotFound("product not found");

        var cart = await context.Koszyks
            .Include(k => k.PozycjaKoszykas)
            .FirstOrDefaultAsync(k => k.IdKoszyka == dto.IdKoszyka);

        if (cart == null)
            return NotFound("cart not found");


        var newItem = new PozycjaKoszyka
        {
            IdKoszyka = cart.IdKoszyka,
            IdProduktu = dto.IdProduktu,
            Ilosc = dto.Ilosc,
        };
        context.PozycjaKoszykas.Add(newItem);

        cart.DataOstatniejModyfikacji = DateTime.Now;

        await context.SaveChangesAsync();
        return Ok();
    }
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateDeltaToItem(UpdateCartItemDto dto)
    {

        var item = await context.PozycjaKoszykas
            .Include(x => x.IdKoszykaNavigation)
            .FirstOrDefaultAsync(x => x.IdPozycji == dto.IdPozycji);
        var product = await context.Produkts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdProduktu == dto.IdProduktu);

        if (product == null) return NotFound("product");
        if (item is null) return NotFound("cartItem");


        int totalRequestedQuantity = dto.Ilosc + item.Ilosc;

        if (totalRequestedQuantity > product.StanMagazynowy)
        {
            return BadRequest($"Maximum available stock reached: {product.StanMagazynowy}. of {product.NazwaProduktu}");
        }
        if (totalRequestedQuantity <= 0)
            return BadRequest($"stock cannot be zero ");

        item.Ilosc = totalRequestedQuantity;

        UpdateCartDate(item);
        if (item.IdKoszykaNavigation != null)
        {
            item.IdKoszykaNavigation.DataOstatniejModyfikacji = DateTime.Now;
        }
        await context.SaveChangesAsync();
        return NoContent();
    }


    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCartItem(int id)
    {
        var item = await context.PozycjaKoszykas.FindAsync(id);

        if (item == null)
            return NotFound();

        UpdateCartDate(item);
        context.PozycjaKoszykas.Remove(item);
        await context.SaveChangesAsync();

        return NoContent();
    }
    private static void UpdateCartDate(PozycjaKoszyka item)
    {
        if (item.IdKoszykaNavigation != null)
        {
            item.IdKoszykaNavigation.DataOstatniejModyfikacji = DateTime.Now;
        }
    }
}