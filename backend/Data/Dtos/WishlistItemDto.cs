namespace API.Dtos;

public class WishlistItemDto
{
    public int ProductId { get; set; } 
    public string NazwaProduktu { get; set; } = null!;
    public string AutorImie { get; set; } = null!;
    public string AutorNazwisko { get; set; } = null!;
    public decimal Cena { get; set; }
    public string? Zdjecie { get; set; }
}