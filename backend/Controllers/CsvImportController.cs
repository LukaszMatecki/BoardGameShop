using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsvImportController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            AppDbContext context,
            IConfiguration configuration
            ) : ControllerBase
    {
        //[Authorize]
        [HttpPost("import-user-csv")]
        public async Task<ActionResult<List<ImportUserResultDto>>> ImportUsersFromCsv(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("CSV file is required.");

            var results = new List<ImportUserResultDto>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);

            var header = await reader.ReadLineAsync();

            if (header is null)
                return BadRequest("CSV file is empty.");

            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(',');

                if (columns.Length < 3)
                {
                    results.Add(new ImportUserResultDto
                    {
                        Email = "unknown",
                        Success = false,
                        Message = "Invalid row format. Expected: email,password,role"
                    });

                    continue;
                }

                var email = columns[0].Trim();
                var password = columns[1].Trim();
                var role = columns[2].Trim();

                if (string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(role))
                {
                    results.Add(new ImportUserResultDto
                    {
                        Email = email,
                        Success = false,
                        Message = "Email, password and role are required."
                    });

                    continue;
                }

                var existingUser = await userManager.FindByEmailAsync(email);

                if (existingUser is not null)
                {
                    results.Add(new ImportUserResultDto
                    {
                        Email = email,
                        Success = false,
                        Message = "User already exists."
                    });

                    continue;
                }

                await using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(role));

                        if (!roleResult.Succeeded)
                        {
                            await transaction.RollbackAsync();

                            results.Add(new ImportUserResultDto
                            {
                                Email = email,
                                Success = false,
                                Message = string.Join(" ", roleResult.Errors.Select(e => e.Description))
                            });

                            continue;
                        }
                    }

                    var user = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = false
                    };

                    var createResult = await userManager.CreateAsync(user, password);

                    if (!createResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        results.Add(new ImportUserResultDto
                        {
                            Email = email,
                            Success = false,
                            Message = string.Join(" ", createResult.Errors.Select(e => e.Description))
                        });

                        continue;
                    }

                    var roleAssignResult = await userManager.AddToRoleAsync(user, role);

                    if (!roleAssignResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        results.Add(new ImportUserResultDto
                        {
                            Email = email,
                            Success = false,
                            Message = string.Join(" ", roleAssignResult.Errors.Select(e => e.Description))
                        });

                        continue;
                    }

                    context.UzytkownikDetails.Add(new UzytkownikDetail
                    {
                        IdUzytkownika = user.Id,
                        CzyUsuniety = false,
                        DataRejestracji = DateTime.Now
                    });

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    results.Add(new ImportUserResultDto
                    {
                        Email = email,
                        Success = true,
                        Message = "User imported successfully."
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    results.Add(new ImportUserResultDto
                    {
                        Email = email,
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            return Ok(results);
        }
    
        [Authorize]
        [HttpPost("import-product-csv")]
        //!!!WAŻNE!!! W tym momencie to po prostu działa ale w imortowanych wartościach nie możemy mieć ","!!!WAŻNE!!!
        public async Task<ActionResult<List<ImportProductResultDto>>> ImportProductsFromCsv(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("CSV file is required.");

            var results = new List<ImportProductResultDto>();

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);

            var header = await reader.ReadLineAsync();

            if (header is null)
                return BadRequest("CSV file is empty.");

            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(',');

                if (columns.Length < 9)
                {
                    results.Add(new ImportProductResultDto
                    {
                        ProductName = "unknown",
                        Success = false,
                        Message = "Invalid row format."
                    });

                    continue;
                }

                var productName = columns[0].Trim();

                try
                {
                    var idAutora = int.Parse(columns[1].Trim());
                    var cena = decimal.Parse(columns[2].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    var stanMagazynowy = int.Parse(columns[3].Trim());
                    var dataWydania = DateOnly.Parse(columns[4].Trim());
                    var opis = string.IsNullOrWhiteSpace(columns[5]) ? null : columns[5].Trim();
                    var czyUkryty = bool.Parse(columns[6].Trim());

                    var categoryIds = ParseIds(columns[7]);
                    var tagIds = ParseIds(columns[8]);

                    var autorExists = await context.Autors.FirstOrDefaultAsync(a => a.IdAutora == idAutora);

                    if (autorExists is null)
                    {
                        results.Add(new ImportProductResultDto
                        {
                            ProductName = productName,
                            Success = false,
                            Message = $"Author with ID {idAutora} does not exist."
                        });

                        continue;
                    }

                    var categories = await context.Kategoria
                        .Where(k => categoryIds.Contains(k.IdKategorii))
                        .ToListAsync();

                    if (categories.Count != categoryIds.Count)
                    {
                        results.Add(new ImportProductResultDto
                        {
                            ProductName = productName,
                            Success = false,
                            Message = "One or more categories do not exist."
                        });

                        continue;
                    }

                    var tags = await context.Tags
                        .Where(t => tagIds.Contains(t.IdTagu))
                        .ToListAsync();

                    if (tags.Count != tagIds.Count)
                    {
                        results.Add(new ImportProductResultDto
                        {
                            ProductName = productName,
                            Success = false,
                            Message = "One or more tags do not exist."
                        });

                        continue;
                    }

                    var product = new Produkt
                    {
                        NazwaProduktu = productName,
                        IdAutora = idAutora,
                        Cena = cena,
                        StanMagazynowy = stanMagazynowy,
                        DataWydania = dataWydania,
                        Opis = opis,
                        CzyUkryty = czyUkryty
                    };

                    foreach (var category in categories)
                        product.IdKategoriis.Add(category);

                    foreach (var tag in tags)
                        product.IdTagus.Add(tag);

                    context.Produkts.Add(product);
                    await context.SaveChangesAsync();

                    results.Add(new ImportProductResultDto
                    {
                        ProductName = productName,
                        Success = true,
                        Message = "Product imported successfully."
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new ImportProductResultDto
                    {
                        ProductName = productName,
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            return Ok(results);
        }

        private static List<int> ParseIds(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<int>();

            return value
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Trim()))
                .Distinct()
                .ToList();
        }
    
    }
}