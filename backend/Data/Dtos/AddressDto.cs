using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class AddressDto
{
    public int IdAdresu { get; set; }

    [Required(ErrorMessage = "Miasto nie może być puste.")]
    [StringLength(100, MinimumLength = 2)]
    public required string Miasto { get; set; }

    [Required(ErrorMessage = "Ulica nie może być pusta.")]
    public required string Ulica { get; set; }

    [Required(ErrorMessage = "Numer budynku jest wymagany.")]
    public required string NumerBudynku { get; set; }

    public string? NumerLokalu { get; set; }

    [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
    [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi mieć format 00-000.")]
    public required string KodPocztowy { get; set; }
}