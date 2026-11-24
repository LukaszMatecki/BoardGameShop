namespace API.Dtos;

public class ImportProductResultDto
{
    public string ProductName { get; set; } = null!;
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}