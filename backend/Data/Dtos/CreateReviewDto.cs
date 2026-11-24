namespace API.Dtos;

public class CreateReviewDto
{
    public int IdProduktu { get; set; }
    public int Ocena { get; set; } 
    public string? Komentarz { get; set; }
}