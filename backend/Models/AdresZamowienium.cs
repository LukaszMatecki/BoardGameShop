using System;
using System.Collections.Generic;

namespace API.Models;

public partial class AdresZamowienium
{
    public int IdAdresuZamowienia { get; set; }

    public string Miasto { get; set; } = null!;

    public string Ulica { get; set; } = null!;

    public string NumerBudynku { get; set; } = null!;

    public string? NumerLokalu { get; set; }

    public string KodPocztowy { get; set; } = null!;

    public string? ImieOdbiorcy { get; set; }

    public string? NazwiskoOdbiorcy { get; set; }

    // public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();
}
