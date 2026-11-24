using API.Models;

namespace API.Dtos;

public class CartDto
{
    public int IdKoszyka { get; set; }
    public int IdUzytkownika { get; set; }
    public DateTime DataOstatniejModyfikacji { get; set; }
    
    public List<CartItemDto> Pozycje { get; set; } = new();
    
    public decimal SumaKoszyka { get; set; }

   
}