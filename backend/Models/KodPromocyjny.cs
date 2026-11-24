using System;
using System.Collections.Generic;

namespace API.Models;

public partial class KodPromocyjny
{
    public int IdKodu { get; set; }

    public string Kod { get; set; } = null!;

    public int ZnizkaProcentowa { get; set; }

    public virtual ICollection<UzytkownikDetail> Uzytkownicy { get; set; } = new List<UzytkownikDetail>();
}
