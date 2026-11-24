using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class AppUser : IdentityUser<int>
{
    public UzytkownikDetail? Details { get; set; }
}