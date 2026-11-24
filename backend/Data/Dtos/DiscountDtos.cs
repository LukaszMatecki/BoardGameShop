namespace API.Dtos;

public class AddDiscountCodeDto
{
    public int UserId { get; set; }
    public string DiscountCode { get; set; } = null!;
    public int DiscountPercentage { get; set; }
}

public class DiscountCodeDto
{
    public int IdKodu { get; set; }
    public string Kod { get; set; } = null!;
    public int ZnizkaProcentowa { get; set; }
    public bool CzyWykorzystany { get; set; }
}