using API.Dtos;

public class CartItemDto
{
    public int IdPozycji { get; set;}
    public int IdProduktu { get; set; }
    public ProductDto Product { get; set; } = new();
    public int Ilosc { get; set; }
}