namespace API.Dtos;

public class ImportUserResultDto
{
    public string Email { get; set; } = null!;
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}