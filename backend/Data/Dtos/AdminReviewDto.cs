namespace API.Dtos;

public class AdminReviewDto
{
    public int IdOpinii { get; set; }
    public int IdProduktu { get; set; }
    public string NazwaProduktu { get; set; } = null!;
    public string UzytkownikLogin { get; set; } = null!;
    public int Ocena { get; set; }
    public string? Komentarz { get; set; }
    public DateTime DataWystawienia { get; set; }
}