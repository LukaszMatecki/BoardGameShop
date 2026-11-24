using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Data.Dtos;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole<int>> roleManager,
    AppDbContext context,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        const string roleName = "Customer";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(roleName));

            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, roleName);

        if (!addRoleResult.Succeeded)
            return BadRequest(addRoleResult.Errors);

        var details = new UzytkownikDetail
        {
            IdUzytkownika = user.Id,
            CzyUsuniety = false,
            DataRejestracji = DateTime.Now
        };

        context.UzytkownikDetails.Add(details);
        await context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Email,
            role = roleName,
            message = "User registered successfully"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return Unauthorized("Invalid email or password.");

        var passwordValid = await userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordValid)
            return Unauthorized("Invalid email or password.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        var roles = await userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            int.Parse(configuration["Jwt:ExpiresMinutes"] ?? "120"));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            expires,
            user = new
            {
                user.Id,
                user.Email,
                user.UserName,
                Roles = roles
            }
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdmin(LoginDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
            return BadRequest("User with this email already exists.");

        const string roleName = "Admin";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(roleName));

            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var createResult = await userManager.CreateAsync(user, dto.Password);

        if (!createResult.Succeeded)
            return BadRequest(createResult.Errors);

        var roleAssignResult = await userManager.AddToRoleAsync(user, roleName);

        if (!roleAssignResult.Succeeded)
            return BadRequest(roleAssignResult.Errors);

        context.UzytkownikDetails.Add(new UzytkownikDetail
        {
            IdUzytkownika = user.Id,
            CzyUsuniety = false,
            DataRejestracji = DateTime.Now
        });

        await context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Email,
            role = roleName,
            message = "Admin account created successfully."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await context.Users
            .AsNoTracking()
            .Include(u => u.Details)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                    Name = u.Details != null
                    ? u.Details.Imie
                    : null,
                Surname = u.Details != null
                    ? u.Details.Nazwisko
                    : null,
                CzyUsuniety = u.Details != null && u.Details.CzyUsuniety,
                DataRejestracji = u.Details != null
                    ? u.Details.DataRejestracji
                    : null
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}/user-name-details")]
    public async Task<ActionResult> GetUserName(int id)
    {
            var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Details)
            .Where(u => u.Id == id)
            .Select(u => new UserNameDetails
            {
                UserId = u.Id,
                Name = u.Details != null
                    ? u.Details.Imie
                    : null,
                Surname = u.Details != null
                    ? u.Details.Nazwisko
                    : null
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Details)
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                CzyUsuniety = u.Details != null && u.Details.CzyUsuniety,
                DataRejestracji = u.Details != null
                    ? u.Details.DataRejestracji
                    : null
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpPatch("{id:int}/soft-delete")]
    public async Task<IActionResult> SoftDeleteUser(int id)
    {
        var details = await context.UzytkownikDetails
            .FirstOrDefaultAsync(u => u.IdUzytkownika == id);

        if (details is null)
            return NotFound();

        if (details.CzyUsuniety)
            return NoContent();

        details.CzyUsuniety = true;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:int}/restore")]
    public async Task<IActionResult> RestoreUser(int id)
    {
        var details = await context.UzytkownikDetails
            .FirstOrDefaultAsync(u => u.IdUzytkownika == id);

        if (details is null)
            return NotFound();

        details.CzyUsuniety = false;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPut("details")]
    public async Task<IActionResult> UpdateDetails(UpdateUserDetailsDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var details = await context.UzytkownikDetails
            .FirstOrDefaultAsync(u => u.IdUzytkownika == userId);

        if (details == null)
            return NotFound();

        details.Imie = dto.Imie;
        details.Nazwisko = dto.Nazwisko;
        details.NumerTelefonu = dto.NumerTelefonu;

        await context.SaveChangesAsync();

        return Ok(details);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var roles = await userManager.GetRolesAsync(user);

        var ud = await context.UzytkownikDetails
            .FirstOrDefaultAsync(u => u.IdUzytkownika == userId);

        return Ok(new
        {
            Email = user.Email,
            UserName = user.UserName,
            Imie = ud?.Imie,
            Nazwisko = ud?.Nazwisko,
            Telefon = ud?.NumerTelefonu,
            Roles = roles
        });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userId== null) return Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found.");

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password changed successfully" });    
    
    }


    [Authorize]
    [HttpGet("addresses")]
    public async Task<IActionResult> GetUserAddresses()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var addresses = await context.Adres
            .Where(a => a.IdUzytkownika == userId)
            .Select(a => new AddressDto {
                IdAdresu = a.IdAdresu,
                Miasto = a.Miasto,
                Ulica = a.Ulica,
                NumerBudynku = a.NumerBudynku,
                NumerLokalu = a.NumerLokalu,
                KodPocztowy = a.KodPocztowy
            }).ToListAsync();

        return Ok(addresses);
    }

    [Authorize]
    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress(AddressDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var newAddress = new Adre 
        {
            IdUzytkownika = userId,
            Miasto = dto.Miasto,
            Ulica = dto.Ulica,
            NumerBudynku = dto.NumerBudynku,
            NumerLokalu = dto.NumerLokalu,
            KodPocztowy = dto.KodPocztowy
        };

        context.Adres.Add(newAddress);
        await context.SaveChangesAsync();

        return Ok(newAddress);
    }

    [Authorize]
    [HttpDelete("addresses/{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var address = await context.Adres.FirstOrDefaultAsync(a => a.IdAdresu == id && a.IdUzytkownika == userId);

        if (address == null) return NotFound();

        context.Adres.Remove(address);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpPut("addresses/{id:int}")]
    public async Task<IActionResult> UpdateAddress(int id, AddressDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var address = await context.Adres.FirstOrDefaultAsync(a => a.IdAdresu == id && a.IdUzytkownika == userId);

        if (address == null) return NotFound();

        address.Miasto = dto.Miasto;
        address.Ulica = dto.Ulica;
        address.NumerBudynku = dto.NumerBudynku;
        address.NumerLokalu = dto.NumerLokalu;
        address.KodPocztowy = dto.KodPocztowy;

        await context.SaveChangesAsync();
        return Ok(address);
    }
}