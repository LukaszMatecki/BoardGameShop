namespace API.Dtos;

public class ReturnDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int IdPozycji { get; set; }
    public DateTime DataZgloszenia { get; set; }
    public string? Email { get; set; }
    public string Powod { get; set; } = null!;
    public bool CzyZaakceptowany { get; set; }
    public string Typ { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public class CreateReturnDto
{
    public int IdZamowienia { get; set; }
    public int IdPozycji { get; set; }
    public string Powod { get; set; } = null!;
    public int IdTypu { get; set; }
}

public class ReturnTypesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class ReturnStatus
{
    public int Id { get; set; }
    public int IdStatusu { get; set; }
    public string Status { get; set; } = null!;
}
