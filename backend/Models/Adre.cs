using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Adre
{
    public int IdAdresu { get; set; }

    public int? IdUzytkownika { get; set; }

    public string Miasto { get; set; } = null!;

    public string Ulica { get; set; } = null!;

    public string NumerBudynku { get; set; } = null!;

    public string? NumerLokalu { get; set; }

    public string KodPocztowy { get; set; } = null!;

    public virtual UzytkownikDetail? IdUzytkownikaNavigation { get; set; }
    public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();
}
