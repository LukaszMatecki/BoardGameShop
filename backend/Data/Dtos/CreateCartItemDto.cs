namespace API.Dtos;

public class CreateCartItemDto
{
    public int IdKoszyka { get; set; }
    public int IdProduktu { get; set; }
    public int Ilosc { get; set; }
}