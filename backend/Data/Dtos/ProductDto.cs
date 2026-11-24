namespace API.Dtos;

public class ProductDto
{
    public int IdProduktu { get; set; }

    public string NazwaProduktu { get; set; } = null!;
    public int IdAutora { get; set; }
    public decimal Cena { get; set; }
    public int PromocjaWProc { get; set; }
    public decimal? NajnizszaCenaW30Dni { get; set; }

    // public DateTime? DataNajnizszejCenyW30Dni { get; set; }

    public decimal CenaPoPromocji => Math.Round(PromocjaWProc > 0
        ? Cena * (1 - (decimal)PromocjaWProc / 100)
        : Cena,2);
    public string? Opis { get; set; }
    public string? Zdjecie { get; set; }
    public int StanMagazynowy { get; set; }
    public bool CzyUkryty { get; set; }
    public DateOnly DataWydania { get; set; }

    public string AutorImie { get; set; } = null!;
    public string AutorNazwisko { get; set; } = null!;

    public double SredniaOcena { get; set; }
    public int LiczbaOpinii { get; set; }

    public List<CategoryDto> Kategorie { get; set; } = new();
    public List<TagDto> Tagi { get; set; } = new();

    public List<ReviewDto> Opinie { get; set; } = new();
    public List<ProductDto> RelatedProducts { get; set; } = new();

}