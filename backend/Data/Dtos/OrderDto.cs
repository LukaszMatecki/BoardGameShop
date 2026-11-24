using API.Data.Dtos;

namespace API.Dtos;

public class OrderDto
{
    public int IdZamowienia { get; set; }
    public int? IdUzytkownika { get; set; }
    public int IdStatusu { get; set; }
    public int IdMetodyDostawy { get; set; }
    public int IdMetodyPlatnosci { get; set; }
    public DateTime DataZamowienia { get; set; }
    public decimal CalkowitaKwota { get; set; }
    public decimal? KosztDostawy { get; set; }
    public int? IdAdresuZamowienia { get; set; }
    public AddressDto? AdresZamowienia { get; set; }
    public List<OrderPozycjaDto> Pozycje { get; set; } = new();

    public string? Email { get; set; }
    public string? StatusNazwa { get; set; }
    public string? MetodaDostawyNazwa { get; set; }
    public string? MetodaPlatnosciNazwa { get; set; }
}
public class OrderPozycjaDto
{
    public int IdPozycji { get; set; }
    public int IdProduktu { get; set; }
    public int? IdStatusuZwrotu { get; set; }
    public string? StatusZwrotu { get; set; }
    public string NazwaProduktu { get; set; } = null!;

    public int Ilosc { get; set; }

    public decimal Cena { get; set; }
}
