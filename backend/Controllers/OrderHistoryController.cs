using API.Data;
using API.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderHistoryController(AppDbContext context) : ControllerBase
    {
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetUserOrders(int userId)
        {
            var orders = await context.Zamowienies
                .Where(z => z.IdUzytkownika == userId)
                .Select(z => new OrderHistoryDto
                {
                    IdZamowienia = z.IdZamowienia,
                    DataZamowienia = z.DataZamowienia,
                    CalkowitaKwota = z.CalkowitaKwota,

                    Status = z.IdStatusuNavigation.NazwaStatusu,

                    MetodaPlatnosci =
                        z.IdMetodyPlatnosciNavigation.NazwaPlatnosci,

                    MetodaDostawy =
                        z.IdMetodyDostawyNavigation.NazwaMetody,

                    Produkty = z.PozycjaZamowienia
                        .Select(p => new OrderProductDto
                        {
                            IdProduktu = p.IdProduktu,
                            NazwaProduktu = p.NazwaWMomencieZakupu,
                            Ilosc = p.Ilosc,
                            CenaZakupu = p.CenaZakupu
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.DataZamowienia)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPatch("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            UpdateOrderStatusDto dto)
        {
            var order = await context.Zamowienies
                .FirstOrDefaultAsync(x => x.IdZamowienia == orderId);

            if (order is null)
                return NotFound();

            var statusExists = await context.SlownikStatusows
                .AnyAsync(x => x.IdStatusu == dto.IdStatusu);

            if (!statusExists)
                return BadRequest("Status does not exist.");

            order.IdStatusu = dto.IdStatusu;

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}