using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/login")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AuthenticateUser([FromBody] UserCredentials creds)
    {
        if (creds == null)
        {
            return BadRequest();
        }

        bool isUserValid = ValidateInDatabase(creds.Email, creds.Password);

        if (isUserValid)
        {
            return Ok();
        }

        return Unauthorized();
    }

    private bool ValidateInDatabase(string userEmail, string userPassword)
    {
        return true;
    }
}

public class UserCredentials
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}