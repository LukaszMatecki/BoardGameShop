using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(AppDbContext context, IMemoryCache cache) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            var trzydziesciDniTemu = DateTime.UtcNow.AddDays(-30);

            await context.Produkts
                .Where(p => p.DataNajnizszejCenyW30Dni == null || p.DataNajnizszejCenyW30Dni < trzydziesciDniTemu)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.NajnizszaCenaW30Dni, p => p.PromocjaWProc > 0
                        ? p.Cena * (1 - (decimal)p.PromocjaWProc / 100)
                        : p.Cena)
                    .SetProperty(p => p.DataNajnizszejCenyW30Dni, DateTime.UtcNow)
                );

            var products = await ProductDtoQuery(context.Produkts)
                .ToListAsync();

            return Ok(products);
            }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDto>>> GetFilteredProducts([FromQuery] string? param)
        {
            var query = context.Produkts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(param))
            {
                var term = $"%{param.Trim()}%";

                query = query.Where(p =>
                    EF.Functions.ILike(p.NazwaProduktu, term) ||
                    EF.Functions.ILike(p.IdAutoraNavigation.Imie, term) ||
                    EF.Functions.ILike(p.IdAutoraNavigation.Nazwisko, term));
            }

            var products = await ProductDtoQuery(query)
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var trzydziesciDniTemu = DateTime.UtcNow.AddDays(-30);

            await context.Produkts
                .Where(p => p.IdProduktu == id && (p.DataNajnizszejCenyW30Dni == null || p.DataNajnizszejCenyW30Dni < trzydziesciDniTemu))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.NajnizszaCenaW30Dni, p => Math.Round(p.PromocjaWProc > 0 
                        ? p.Cena * (1 - (decimal)p.PromocjaWProc / 100) 
                        : p.Cena,2))
                    .SetProperty(p => p.DataNajnizszejCenyW30Dni, DateTime.UtcNow)
                );

            var product = await ProductDtoQuery(context.Produkts)
                .FirstOrDefaultAsync(p => p.IdProduktu == id);

            if (product is null)
                return NotFound();

            product.Opinie = await GetProductReviews(id);
            product.RelatedProducts = await GetRelatedProducts(id);

            return Ok(product);
    }


    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            var validationResult = await ValidateCreateProductDto(dto);
            if (validationResult is not null)
                return validationResult;

            var categories = await context.Kategoria
                .Where(k => dto.KategorieIds.Contains(k.IdKategorii))
                .ToListAsync();

            var tags = await context.Tags
                .Where(t => dto.TagiIds.Contains(t.IdTagu))
                .ToListAsync();

            var product = new Produkt
            {
                NazwaProduktu = dto.NazwaProduktu,
                IdAutora = dto.IdAutora,
                Cena = dto.Cena,
                PromocjaWProc = 0,
                NajnizszaCenaW30Dni = dto.Cena,
                DataNajnizszejCenyW30Dni = DateTime.UtcNow,
                StanMagazynowy = dto.StanMagazynowy,
                DataWydania = dto.DataWydania,
                Opis = dto.Opis,
                Zdjecie = dto.Zdjecie,
                CzyUkryty = dto.CzyUkryty,
            };

            foreach (var category in categories)
                product.IdKategoriis.Add(category);

            foreach (var tag in tags)
                product.IdTagus.Add(tag);

            context.Produkts.Add(product);
            await context.SaveChangesAsync();

            var createdProduct = await ProductDtoQuery(context.Produkts)
                .FirstAsync(p => p.IdProduktu == product.IdProduktu);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.IdProduktu },
                createdProduct
            );
        }

        //[Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await context.Produkts
                .Include(p => p.IdKategoriis)
                .Include(p => p.IdTagus)
                .FirstOrDefaultAsync(p => p.IdProduktu == id);

            if (product is null)
                return NotFound();

            product.IdKategoriis.Clear();
            product.IdTagus.Clear();

            context.Produkts.Remove(product);
            await context.SaveChangesAsync();

            cache.Remove($"related_products_{id}");

            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            var product = await context.Produkts.FindAsync(id);
            
            if (product == null)
                return NotFound();

            if (dto.StanMagazynowy < 0)
                return BadRequest("Stock cannot be negative.");

            product.StanMagazynowy = dto.StanMagazynowy;
            await context.SaveChangesAsync();

            return NoContent();
        }

        private static IQueryable<ProductDto> ProductDtoQuery(IQueryable<Produkt> query)
        {
            return query
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    IdProduktu = p.IdProduktu,
                    NazwaProduktu = p.NazwaProduktu,
                    Cena = p.Cena,
                     PromocjaWProc = p.PromocjaWProc,
                    NajnizszaCenaW30Dni = p.NajnizszaCenaW30Dni,
                    // DataNajnizszejCenyW30Dni = p.DataNajnizszejCenyW30Dni,
                    Opis = p.Opis,
                    DataWydania = p.DataWydania,
                    IdAutora = p.IdAutora,
                    StanMagazynowy = p.StanMagazynowy,
                    CzyUkryty = p.CzyUkryty,
                    Zdjecie = p.Zdjecie,
                    AutorImie = p.IdAutoraNavigation.Imie,
                    AutorNazwisko = p.IdAutoraNavigation.Nazwisko,

                    LiczbaOpinii = p.OpiniaProduktus.Count(),


                    SredniaOcena = p.OpiniaProduktus.Any()
                        ? p.OpiniaProduktus.Average(o => o.Ocena)
                        : 0,

                    Kategorie = p.IdKategoriis
                        .Select(k => new CategoryDto
                        {
                            IdKategorii = k.IdKategorii,
                            NazwaKategorii = k.NazwaKategorii
                        })
                        .ToList(),

                    Tagi = p.IdTagus
                        .Select(t => new TagDto
                        {
                            IdTagu = t.IdTagu,
                            NazwaTagu = t.NazwaTagu
                        })
                        .ToList()
                });
        }

        private async Task<List<ReviewDto>> GetProductReviews(int productId)
        {
            return await context.OpiniaProduktus
                .AsNoTracking()
                .Where(o => o.IdProduktu == productId && o.CzyZatwierdzona)
                .Select(o => new ReviewDto
                {
                    IdOpinii = o.IdOpinii,
                    
                    UzytkownikLogin = o.IdUzytkownikaNavigation.Imie ?? 
                                    o.IdUzytkownikaNavigation.User.UserName!.Split('@', StringSplitOptions.None)[0],
                    Ocena = o.Ocena,
                    Komentarz = o.Komentarz,
                    DataWystawienia = o.DataWystawienia
                })
                .ToListAsync();
        }

        private async Task<List<ProductDto>> GetRelatedProducts(int productId)
        {
            string cacheKey = $"related_products_{productId}";

            if (cache.TryGetValue(cacheKey, out List<ProductDto>? cachedRelatedProducts))
            {
                return cachedRelatedProducts ?? [];
            }

            var tagIds = await context.Produkts
                .Where(p => p.IdProduktu == productId)
                .SelectMany(p => p.IdTagus.Select(t => t.IdTagu))
                .ToListAsync();

            if (tagIds.Count == 0)
                return [];

            var relatedProducts = await ProductDtoQuery(context.Produkts)
                .Where(p => p.IdProduktu != productId)
                .Where(p => context.Produkts
                    .Where(original => original.IdProduktu == p.IdProduktu)
                    .Any(original => original.IdTagus.Any(t => tagIds.Contains(t.IdTagu))))
                .Take(4)
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            cache.Set(cacheKey, relatedProducts, cacheOptions);

            return relatedProducts;

        }

        private async Task<ActionResult?> ValidateCreateProductDto(CreateProductDto dto)
        {
            var autorExists = await context.Autors
                .AnyAsync(a => a.IdAutora == dto.IdAutora);

            if (!autorExists)
                return BadRequest("Author does not exist.");

            var uniqueCategoryIds = dto.KategorieIds.Distinct().ToList();
            var uniqueTagIds = dto.TagiIds.Distinct().ToList();

            var existingCategoryCount = await context.Kategoria
                .CountAsync(k => uniqueCategoryIds.Contains(k.IdKategorii));

            if (existingCategoryCount != uniqueCategoryIds.Count)
                return BadRequest("One or more categories do not exist.");

            var existingTagCount = await context.Tags
                .CountAsync(t => uniqueTagIds.Contains(t.IdTagu));

            if (existingTagCount != uniqueTagIds.Count)
                return BadRequest("One or more tags do not exist.");

            return null;
        }
        
    [HttpPut("{id:int}/promotion")]
    public async Task<IActionResult> UpdatePromotion(int id, [FromQuery] int nowaPromocja)
    {
        if (nowaPromocja < 0 || nowaPromocja > int.MaxValue) 
            return BadRequest("Niepoprawna wartość promocji.");

        var product = await context.Produkts.FirstOrDefaultAsync(p => p.IdProduktu == id);
        if (product is null)
            return NotFound();

        decimal nowaCenaPoPromocji =Math.Round( nowaPromocja > 0 
            ? product.Cena * (1 - (decimal)nowaPromocja / 100) 
            : product.Cena,2);

        if (product.DataNajnizszejCenyW30Dni == null || 
            (DateTime.UtcNow - product.DataNajnizszejCenyW30Dni.Value).TotalDays > 30)
        {
            product.NajnizszaCenaW30Dni = nowaCenaPoPromocji;
            product.DataNajnizszejCenyW30Dni = DateTime.UtcNow;
        }
        else if (nowaCenaPoPromocji < product.NajnizszaCenaW30Dni)
        {
            product.NajnizszaCenaW30Dni = nowaCenaPoPromocji;
            product.DataNajnizszejCenyW30Dni = DateTime.UtcNow;
        }

        product.PromocjaWProc = nowaPromocja;

        await context.SaveChangesAsync();
        
        cache.Remove($"related_products_{id}");

        return NoContent();
    }
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, CreateProductDto dto)
        {
            var product = await context.Produkts
                .Include(p => p.IdKategoriis)
                .Include(p => p.IdTagus)
                .FirstOrDefaultAsync(p => p.IdProduktu == id);

            if (product is null)
                return NotFound();

            var validationResult = await ValidateCreateProductDto(dto);
            if (validationResult is not null)
                return validationResult;

            var categories = await context.Kategoria
                .Where(k => dto.KategorieIds.Contains(k.IdKategorii))
                .ToListAsync();

            var tags = await context.Tags
                .Where(t => dto.TagiIds.Contains(t.IdTagu))
                .ToListAsync();

            product.NazwaProduktu = dto.NazwaProduktu;
            product.IdAutora = dto.IdAutora;
            product.Cena = dto.Cena;
            product.StanMagazynowy = dto.StanMagazynowy;
            product.DataWydania = dto.DataWydania;
            product.Opis = dto.Opis;
            product.Zdjecie = dto.Zdjecie;
            product.CzyUkryty = dto.CzyUkryty;

            product.IdKategoriis.Clear();
            foreach (var category in categories)
                product.IdKategoriis.Add(category);

            product.IdTagus.Clear();
            foreach (var tag in tags)
                product.IdTagus.Add(tag);

            await context.SaveChangesAsync();
            
            cache.Remove($"related_products_{id}");

            return NoContent();
        }

    [HttpGet("tag/{tagName}")]
public async Task<ActionResult<List<ProductDto>>> GetProductsByTag(string tagName)
{
    var query = context.Produkts
        .AsNoTracking()
        .Where(p => p.IdTagus.Any(t => EF.Functions.ILike(t.NazwaTagu, tagName)));

    var products = await ProductDtoQuery(query).ToListAsync();

    return Ok(products);
}

[HttpPost("tag/{tagId:int}/promotion")]
public async Task<IActionResult> SetTagPromotion(int tagId, [FromQuery] int discountPercentage)
{
    if (discountPercentage < 0 || discountPercentage > 100)
        return BadRequest("Invalid discount percentage.");

    var products = await context.Produkts
        .Where(p => p.IdTagus.Any(t => t.IdTagu == tagId))
        .ToListAsync();

    if (!products.Any())
        return NotFound("No products found for this tag.");

    foreach (var product in products)
    {
        product.PromocjaWProc = discountPercentage;
    }

    await context.SaveChangesAsync();

    // Wyczyść cache dla powiązanych produktów
    foreach (var product in products)
        cache.Remove($"related_products_{product.IdProduktu}");

    return NoContent();
}

[HttpDelete("tag/{tagId:int}/promotion")]
public async Task<IActionResult> ClearTagPromotion(int tagId)
{
    // Pobieramy produkty, które mają ten tag oraz aktualnie posiadają jakąkolwiek promocję
    var products = await context.Produkts
        .Where(p => p.IdTagus.Any(t => t.IdTagu == tagId))
        .ToListAsync();

    if (!products.Any())
        return NotFound("No products found for this tag.");

    foreach (var product in products)
    {
        // Resetujemy promocję procentową do zera
        product.PromocjaWProc = 0;
        
        // Opcjonalnie: jeśli chcesz zresetować najniższą cenę z 30 dni do ceny standardowej przy usuwaniu promocji:
        product.NajnizszaCenaW30Dni = product.Cena;
        product.DataNajnizszejCenyW30Dni = DateTime.UtcNow;
    }

    await context.SaveChangesAsync();

    // Czyszczenie cache dla zmodyfikowanych produktów
    foreach (var product in products)
        cache.Remove($"related_products_{product.IdProduktu}");

    return NoContent();
}

}