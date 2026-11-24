using System;
using System.Collections.Generic;

namespace API.Models;

public class UzytkownikKodPromocyjny
{
    public int IdUzytkownika { get; set; }
    public virtual UzytkownikDetail Uzytkownik { get; set; } = null!;
    public int IdKodu { get; set; }
    public virtual KodPromocyjny KodPromocyjny { get; set; } = null!;
    public bool CzyWykorzystany { get; set; }
}