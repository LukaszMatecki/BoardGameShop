namespace API.Dtos;

public class CreateOrderDto
{
    public int IdMetodyDostawy { get; set; }
    public int IdMetodyPlatnosci { get; set; }
    public List<CreateOrderItemDto> Pozycje { get; set; } = new();
    public decimal CalkowitaKwota { get; set; }
    public int? IdAdresu { get; set; }
    public AddressDto? NowyAdres { get; set; }
    public string? Email { get; set; }
    public string? Imie { get; set; }
    public string? Nazwisko { get; set; }
    public string? NumerTelefonu { get; set; }
    public int? IdKodu { get; set; }
}

public class CreateOrderItemDto
{
    public int IdProduktu { get; set; }
    public int Ilosc { get; set; }
}