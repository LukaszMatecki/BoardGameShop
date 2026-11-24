using System.Net;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ContactController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public class ContactDto
    {
        [Required] public required string Name { get; set; }
        [Required] [EmailAddress] public required string Email { get; set; }
        [Required] public required string Subject { get; set; }
        [Required] public required string Message { get; set; }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendContactMessage([FromBody] ContactDto dto)
    {
        var subject = $"[Formularz Kontaktowy] {dto.Subject}";
        
        var messageBody = $@"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #fbfbfe; padding: 30px; color: #150e24;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #eae8f5;'>
                    
                    <h2 style='color: #151875; margin-top: 0; border-bottom: 2px solid #f6f5ff; padding-bottom: 15px;'>
                        Nowa wiadomość - Sklep Kotika
                    </h2>
                    
                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                        <tr>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0; width: 100px; color: #8a8fb9;'><strong>Od:</strong></td>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0; color: #150e24;'>{dto.Name}</td>
                        </tr>
                        <tr>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0; color: #8a8fb9;'><strong>Email:</strong></td>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0;'>
                                <a href='mailto:{dto.Email}' style='color: #3f509e; text-decoration: none;'>{dto.Email}</a>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0; color: #8a8fb9;'><strong>Temat:</strong></td>
                            <td style='padding: 10px 0; border-bottom: 1px solid #f0f0f0; color: #150e24;'>{dto.Subject}</td>
                        </tr>
                    </table>

                    <h4 style='color: #151875; margin-bottom: 10px; font-size: 16px;'>Treść wiadomości:</h4>
                    
                    <div style='background-color: #fafafc; padding: 20px; border-left: 4px solid #3f509e; border-radius: 6px; line-height: 1.6; color: #4a405c;'>
                        {dto.Message.Replace("\n", "<br>")}
                    </div>
                    
                    <div style='margin-top: 35px; padding-top: 15px; border-top: 1px solid #eae8f5; text-align: center; color: #a0a4c0; font-size: 12px;'>
                        Wiadomość została wygenerowana automatycznie przez formularz kontaktowy na stronie Sklepu Kotika.<br>
                        Aby odpowiedzieć nadawcy, po prostu kliknij ""Odpowiedz"" w swoim programie pocztowym.
                    </div>

                </div>
            </div>
        ";

        try
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPortString = _configuration["Smtp:Port"];
            var smtpUser = _configuration["Smtp:User"];
            var smtpPassword = _configuration["Smtp:Password"];

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
                From = new MailAddress(smtpUser, "Formularz Kotika"),
                Subject = subject,
                Body = messageBody,
                IsBodyHtml = true,
            };
            
            mailMessage.To.Add(smtpUser);
            mailMessage.ReplyToList.Add(new MailAddress(dto.Email, dto.Name));

            await smtpClient.SendMailAsync(mailMessage);
            
            return Ok(new { message = "Wiadomość została wysłana pomyślnie!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BŁĄD FORMULARZA KONTAKTOWEGO]: {ex.Message}");
            return BadRequest(new { message = "Wystąpił błąd podczas wysyłania wiadomości. Spróbuj ponownie później." });
        }
    }
}