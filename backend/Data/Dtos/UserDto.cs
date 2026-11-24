namespace API.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Name {get; set;}
    public string? Surname {get;set;}
    public bool CzyUsuniety { get; set; }
    public DateTime? DataRejestracji { get; set; }
}