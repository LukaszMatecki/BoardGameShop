using System.Security.Claims;
using API.Models;
using System.Net;
using System.Net.Mail;
using API.Data;
using API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data.Dtos;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class OrderController(AppDbContext context, IConfiguration configuration) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var orders = await context.Zamowienies
            .AsNoTracking()
            .Where(z => z.IdUzytkownika == userId)
            .OrderByDescending(z => z.DataZamowienia)
            .Select(z => new OrderDto
            {
                IdZamowienia = z.IdZamowienia,
                IdUzytkownika = z.IdUzytkownika,
                IdStatusu = z.IdStatusu,
                IdMetodyDostawy = z.IdMetodyDostawy,
                IdMetodyPlatnosci = z.IdMetodyPlatnosci,
                DataZamowienia = z.DataZamowienia,
                CalkowitaKwota = z.CalkowitaKwota,
                IdAdresuZamowienia = z.IdAdresuZamowienia,
                AdresZamowienia = null,
                Email = z.Email,
                Pozycje = new()
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("orders/{orderId:int}")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var order = await context.Zamowienies
            .AsNoTracking()
            .Where(z => z.IdUzytkownika == userId && z.IdZamowienia == orderId)
            .Select(z => new OrderDto
            {
                IdZamowienia = z.IdZamowienia,
                IdUzytkownika = z.IdUzytkownika,
                IdStatusu = z.IdStatusu,
                IdMetodyDostawy = z.IdMetodyDostawy,
                IdMetodyPlatnosci = z.IdMetodyPlatnosci,
                DataZamowienia = z.DataZamowienia,
                CalkowitaKwota = z.CalkowitaKwota,
                IdAdresuZamowienia = z.IdAdresuZamowienia,

                StatusNazwa = z.IdStatusuNavigation != null ? z.IdStatusuNavigation.NazwaStatusu : "Brak",
                MetodaDostawyNazwa = z.IdMetodyDostawyNavigation != null ? z.IdMetodyDostawyNavigation.NazwaMetody : "Brak",
                MetodaPlatnosciNazwa = z.IdMetodyPlatnosciNavigation != null ? z.IdMetodyPlatnosciNavigation.NazwaPlatnosci : "Brak",

                AdresZamowienia = z.IdAdresuZamowieniaNavigation != null ? new AddressDto
                {
                    IdAdresu = z.IdAdresuZamowieniaNavigation.IdAdresu,
                    Ulica = z.IdAdresuZamowieniaNavigation.Ulica,
                    Miasto = z.IdAdresuZamowieniaNavigation.Miasto,
                    KodPocztowy = z.IdAdresuZamowieniaNavigation.KodPocztowy,
                    NumerBudynku = z.IdAdresuZamowieniaNavigation.NumerBudynku,
                    NumerLokalu = z.IdAdresuZamowieniaNavigation.NumerLokalu
                } : null,
                Email = z.Email,
                Pozycje = z.PozycjaZamowienia.Select(p => new OrderPozycjaDto
                {
                    IdPozycji = p.IdPozycji,
                    IdProduktu = p.IdProduktu,
                    IdStatusuZwrotu = p.ZwrotZamowienium != null ? p.ZwrotZamowienium.IdStatusuZgloszenia : null,
                    StatusZwrotu = p.ZwrotZamowienium != null ? p.ZwrotZamowienium.IdStatusuZgloszeniaNavigation.Nazwa : null,
                    NazwaProduktu = p.NazwaWMomencieZakupu ?? "Nieznany produkt",
                    Ilosc = p.Ilosc,
                    Cena = p.CenaZakupu
                }).ToList()



            })
            .FirstOrDefaultAsync();

        if (order == null) return NotFound("Nie znaleziono zamówienia.");
        return Ok(order);
    }


    [Authorize(Roles = "Customer")]
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            int finalAddressId;

            if (dto.IdAdresu.HasValue)
            {
                var addressLinkedToUser = await context.UzytkownikDetails
                    .Where(u => u.IdUzytkownika == userId)
                    .AnyAsync(u => u.Adres.Any(a => a.IdAdresu == dto.IdAdresu.Value));

                if (!addressLinkedToUser)
                    return BadRequest("The specified address does not belong to this user.");

                finalAddressId = dto.IdAdresu.Value;
            }
            else if (dto.NowyAdres != null)
            {
                var newAddress = new Adre
                {
                    Ulica = dto.NowyAdres.Ulica,
                    Miasto = dto.NowyAdres.Miasto,
                    KodPocztowy = dto.NowyAdres.KodPocztowy,
                    NumerBudynku = dto.NowyAdres.NumerBudynku,
                    NumerLokalu = dto.NowyAdres.NumerLokalu
                };

                var userDetail = await context.UzytkownikDetails.FirstOrDefaultAsync(u => u.IdUzytkownika == userId);
                if (userDetail == null) return BadRequest("User profile not found.");

                userDetail.Adres.Add(newAddress);
                await context.SaveChangesAsync();

                finalAddressId = newAddress.IdAdresu;
            }
            else
            {
                return BadRequest("You must either select an existing address or provide a new one.");
            }

            var actionResult = await ProcessOrderCreation(dto, userId, finalAddressId);

            if (actionResult is BadRequestObjectResult || actionResult is NotFoundObjectResult)
            {
                await transaction.RollbackAsync();
                return actionResult;
            }

            var koszyk = await context.Koszyks
                .FirstOrDefaultAsync(k => k.IdUzytkownika == userId);

            if (koszyk != null)
            {
                var pozycjeKoszyka = await context.PozycjaKoszykas.Where(p => p.IdKoszyka == koszyk.IdKoszyka).ToListAsync();
                context.PozycjaKoszykas.RemoveRange(pozycjeKoszyka);

                context.Koszyks.Remove(koszyk);

                await context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return actionResult;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Internal server error. The order has been canceled, and your cart remains untouched.");
        }
    }

    [AllowAnonymous]
    [HttpPost("orders/guest")]
    public async Task<IActionResult> CreateGuestOrder(CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Imie) ||
            string.IsNullOrWhiteSpace(dto.Nazwisko) ||
            string.IsNullOrWhiteSpace(dto.NumerTelefonu))
        {
            return BadRequest("First name, last name, and email address are required for guest orders.");
        }

        if (dto.NowyAdres == null)
            return BadRequest("A delivery address is required for guest orders.");

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var newAddress = new Adre
            {
                Ulica = dto.NowyAdres.Ulica,
                Miasto = dto.NowyAdres.Miasto,
                KodPocztowy = dto.NowyAdres.KodPocztowy,
                NumerBudynku = dto.NowyAdres.NumerBudynku,
                NumerLokalu = dto.NowyAdres.NumerLokalu
            };

            context.Adres.Add(newAddress);
            await context.SaveChangesAsync();

            var actionResult = await ProcessOrderCreation(dto, userId: null, newAddress.IdAdresu);

            if (actionResult is BadRequestObjectResult || actionResult is NotFoundObjectResult)
            {
                await transaction.RollbackAsync();
                return actionResult;
            }

            await transaction.CommitAsync();
            return actionResult;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Internal server error occurred while processing the guest order.");
        }
    }

    private async Task<IActionResult> ProcessOrderCreation(CreateOrderDto dto, int? userId, int addressId)
    {
        if (dto.Pozycje == null || !dto.Pozycje.Any())
            return BadRequest("The order must contain at least one product.");

        var productIds = dto.Pozycje.Select(p => p.IdProduktu).Distinct().ToList();

        var productsFromDb = await context.Produkts
            .Where(p => productIds.Contains(p.IdProduktu))
            .ToListAsync();

        if (productsFromDb.Count != productIds.Count)
            return BadRequest("One or more selected products do not exist.");

        if (dto.IdKodu is int kodId && userId is int uId)
        {
            var userWithCodes = await context.UzytkownikDetails
                .Include(u => u.KodyPromocyjne)
                .FirstOrDefaultAsync(u => u.IdUzytkownika == uId);

            if (userWithCodes != null)
            {
                var codeToRemove = userWithCodes.KodyPromocyjne.FirstOrDefault(k => k.IdKodu == kodId);
                if (codeToRemove != null)
                {
                    userWithCodes.KodyPromocyjne.Remove(codeToRemove);
                }
            }
        }

        var order = new Zamowienie
        {
            IdUzytkownika = userId,
            IdStatusu = 1,
            IdMetodyDostawy = dto.IdMetodyDostawy,
            IdMetodyPlatnosci = dto.IdMetodyPlatnosci,
            IdAdresuZamowienia = addressId,
            DataZamowienia = DateTime.Now,
            CalkowitaKwota = dto.CalkowitaKwota,

            Email = dto.Email,
            Imie = dto.Imie,
            Nazwisko = dto.Nazwisko,
            NumerTelefonu = dto.NumerTelefonu
        };

        foreach (var itemDto in dto.Pozycje)
        {
            var product = productsFromDb.First(p => p.IdProduktu == itemDto.IdProduktu);

            if (product.StanMagazynowy < itemDto.Ilosc)
            {
                return BadRequest($"Insufficient stock for product '{product.NazwaProduktu}' (Available: {product.StanMagazynowy}).");
            }

            product.StanMagazynowy -= itemDto.Ilosc;

            var orderItem = new PozycjaZamowienium
            {
                IdProduktu = product.IdProduktu,
                Ilosc = itemDto.Ilosc,
                CenaZakupu = product.CenaPoPromocji,
                NazwaWMomencieZakupu = product.NazwaProduktu
            };

            order.PozycjaZamowienia.Add(orderItem);
        }

        context.Zamowienies.Add(order);
        await context.SaveChangesAsync();
        if (dto.Email != null)
        {
            try
            {
                bool isBankTransfer = dto.IdMetodyPlatnosci == 3;
                _ = SendOrderConfirmationEmailAsync(order, dto.Email, isBankTransfer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BŁĄD WYSYŁKI MAILA - ZAMÓWIENIE]: {ex.Message}");
            }
        }

        if (userId.HasValue)
        {
            return CreatedAtAction(
                nameof(GetOrderDetails),
                new { orderId = order.IdZamowienia },
                new { order.IdZamowienia, Message = "Order placed successfully, cart has been cleared." }
            );
        }

        return Ok(new { order.IdZamowienia, Message = "Guest order has been successfully placed." });
    }

    private async Task SendOrderConfirmationEmailAsync(Zamowienie order, string toEmail, bool isBankTransfer)
    {
        var subject = $"Order #{order.IdZamowienia} confirmation - Kotika Store";

        var itemsHtml = string.Join("", order.PozycjaZamowienia.Select(p =>
            $@"<tr>
                <td style='padding:10px; border-bottom:1px solid #eae8f5;'>{p.NazwaWMomencieZakupu}</td>
                <td style='padding:10px; border-bottom:1px solid #eae8f5; text-align:center;'>{p.Ilosc}</td>
                <td style='padding:10px; border-bottom:1px solid #eae8f5; text-align:right;'>{Math.Round(p.CenaZakupu, 2)} PLN</td>
            </tr>"
        ));

        var bankTransferHtml = isBankTransfer
            ? $@"<div style='background-color: #fafafc; padding: 20px; border-left: 4px solid #3f509e; border-radius: 6px; margin-top: 25px; line-height: 1.6; color: #4a405c;'>
                    <h4 style='color: #151875; margin-top: 0; margin-bottom: 10px; font-size: 16px;'>Payment Details (Bank Transfer)</h4>
                    <p style='margin-bottom: 10px;'>To complete your order, please transfer the total amount to the following bank account:</p>
                    <table style='width: 100%;'>
                        <tr><td style='width: 120px; color: #8a8fb9;'><strong>Account Number:</strong></td><td style='font-weight: bold; color: #150e24;'>12 3456 7890 0000 0000 0000 0000</td></tr>
                        <tr><td style='color: #8a8fb9;'><strong>Transfer Title:</strong></td><td style='font-weight: bold; color: #150e24;'>Order #{order.IdZamowienia}</td></tr>
                        <tr><td style='color: #8a8fb9;'><strong>Total Amount:</strong></td><td style='font-weight: bold; color: #150e24;'>{order.CalkowitaKwota} PLN</td></tr>
                    </table>
                </div>"
            : "";

        var messageBody = $@"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #fbfbfe; padding: 30px; color: #150e24;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #eae8f5;'>
                    
                    <h2 style='color: #151875; margin-top: 0; border-bottom: 2px solid #f6f5ff; padding-bottom: 15px;'>
                        Thank you for your order!
                    </h2>
                    
                    <p style='color: #4a405c; line-height: 1.6;'>Hello,</p>
                    <p style='color: #4a405c; line-height: 1.6;'>
                        We wanted to let you know that we have successfully received your order <strong>#{order.IdZamowienia}</strong> and we have just started processing it. 
                        Below you will find the summary of your purchase.
                    </p>

                    <h4 style='color: #151875; margin-bottom: 10px; margin-top: 25px; font-size: 16px;'>Ordered Items:</h4>
                    
                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 10px;'>
                        <thead>
                            <tr style='background-color: #f6f5ff; color: #151875;'>
                                <th style='padding: 10px; border-bottom: 2px solid #eae8f5; text-align:left;'>Product</th>
                                <th style='padding: 10px; border-bottom: 2px solid #eae8f5; text-align:center;'>Qty</th>
                                <th style='padding: 10px; border-bottom: 2px solid #eae8f5; text-align:right;'>Price</th>
                            </tr>
                        </thead>
                        <tbody>
                            {itemsHtml}
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan='2' style='padding: 15px 10px; text-align: right; font-weight: bold; color: #8a8fb9;'>Total:</td>
                                <td style='padding: 15px 10px; text-align: right; font-weight: bold; color: #151875; font-size: 16px;'>{order.CalkowitaKwota} PLN</td>
                            </tr>
                        </tfoot>
                    </table>

                    {bankTransferHtml}

                    <div style='margin-top: 35px; padding-top: 15px; border-top: 1px solid #eae8f5; text-align: center; color: #a0a4c0; font-size: 12px;'>
                        This message was generated automatically by Kotika Store.<br>
                        If you have any questions, please contact us by replying to this email.
                    </div>
                </div>
            </div>
        ";

        try
        {
            var smtpHost = configuration["Smtp:Host"];
            var smtpPortString = configuration["Smtp:Port"];
            var smtpUser = configuration["Smtp:User"];
            var smtpPassword = configuration["Smtp:Password"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortString) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword) || !int.TryParse(smtpPortString, out var smtpPort))
            {
                Console.WriteLine($"\n[BŁĄD KRYTYCZNY] Brak lub nieprawidłowa konfiguracja SMTP w OrderController.");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Kotika Store"),
                Subject = subject,
                Body = messageBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BŁĄD WYSYŁANIA POTWIERDZENIA ZAMÓWIENIA]: {ex.Message}");
        }
    }
}