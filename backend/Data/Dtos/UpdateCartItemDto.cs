using API.Dtos;

public class UpdateCartItemDto
{
    public int IdPozycji { get; set; }
    public int IdKoszyka { get; set; }
    public int IdProduktu { get; set; }
    public int Ilosc { get; set; }
}