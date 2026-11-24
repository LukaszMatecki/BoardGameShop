namespace API.Dtos;

public class ReviewDto
{
    public int IdOpinii { get; set; }
    public string UzytkownikLogin { get; set; } = null!;
    public int Ocena { get; set; }
    public string? Komentarz { get; set; }
    public DateTime DataWystawienia { get; set; }
}