using System.Net.Mail;
using API.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/account")]
public class ForgotPasswordController(
    UserManager<AppUser> userManager,
    IConfiguration configuration) 
    : ControllerBase
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
        public required string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required(ErrorMessage = "Nowe hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        public required string NewPassword { get; set; }
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Ok(new { message = "Jeśli konto o podanym adresie e-mail istnieje, wysłano na nie dalsze instrukcje." });
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        var encodedEmail = WebUtility.UrlEncode(user.Email);
        var frontendUrl = configuration["FrontendUrl"];
        if (string.IsNullOrEmpty(frontendUrl))
        {
            Console.WriteLine("[BŁĄD KRYTYCZNY] Zmienna 'FrontendUrl' nie jest ustawiona w konfiguracji aplikacji.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd serwera. Skontaktuj się z administratorem.");
        }

        var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={encodedEmail}";
        var subject = "Sklep Kotika - Resetowanie hasła";
        
        var messageBody = $@"
            <div style=""background-color: #f9fafb; padding: 50px 20px; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">
                <div style=""max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);"">
                    <div style=""padding: 40px; text-align: center;"">
                        <h2 style=""color: #111827; font-size: 24px; font-weight: 700; margin: 0 0 20px 0;"">Reset hasła</h2>
                        <p style=""color: #4b5563; font-size: 16px; line-height: 1.6; margin: 0 0 30px 0; text-align: left;"">
                            Witaj,<br><br>
                            Otrzymaliśmy prośbę o zmianę hasła do Twojego konta w sklepie <strong>Kotika</strong>. Kliknij poniższy przycisk, aby ustawić nowe hasło.
                        </p>
                        <a href=""{resetLink}"" style=""display: inline-block; background-color: #3f509e; color: #ffffff; font-size: 16px; font-weight: 600; text-decoration: none; padding: 14px 32px; border-radius: 8px;"">Zresetuj hasło</a>
                        <p style=""color: #9ca3af; font-size: 14px; line-height: 1.5; margin: 30px 0 0 0; text-align: left; border-top: 1px solid #e5e7eb; padding-top: 20px;"">
                            Jeśli to nie Ty wysłałeś tę prośbę, zignoruj tę wiadomość. Twoje konto pozostanie bezpieczne.
                        </p>
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
                Console.WriteLine($"\n[BŁĄD KRYTYCZNY] Brak lub nieprawidłowa konfiguracja SMTP.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd serwera uniemożliwiający wysłanie wiadomości.");
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
                From = new MailAddress(smtpUser, "Sklep Kotika"),
                Subject = subject,
                Body = messageBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(dto.Email.Trim());

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"\n[SUKCES] Mail wysłany do: {dto.Email}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[BŁĄD WYSYŁANIA MAILA]: {ex.Message}");
            return BadRequest(new { message = "Nie udało się wysłać maila z instrukcjami resetowania hasła. Spróbuj ponownie." });
        }

        return Ok(new { message = "Jeśli konto o podanym adresie e-mail istnieje, wysłano na nie dalsze instrukcje." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return BadRequest(new { message = "Nie udało się zresetować hasła. Spróbuj ponownie." });
        }

        var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (result.Succeeded)
        {
            return Ok(new { message = "Hasło zostało pomyślnie zresetowane." });
        }

        foreach (var error in result.Errors)
        {
            if (error.Code == "InvalidToken")
            {
                return BadRequest(new { message = "Link do resetowania hasła jest nieprawidłowy lub wygasł. Spróbuj ponownie." });
            }
        }
        return BadRequest(new { message = "Wystąpił błąd podczas resetowania hasła." });
    }
}