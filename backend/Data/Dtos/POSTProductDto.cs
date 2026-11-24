namespace API.Dtos;

public class CreateProductDto
{
    public string NazwaProduktu { get; set; } = null!;
    public int IdAutora { get; set; }
    public decimal Cena { get; set; }
    public int StanMagazynowy { get; set; }
    public DateOnly DataWydania { get; set; }
    public string? Opis { get; set; }
    public string? Zdjecie { get; set; }
    public bool CzyUkryty { get; set; }

    public List<int> KategorieIds { get; set; } = new();
    public List<int> TagiIds { get; set; } = new();
}