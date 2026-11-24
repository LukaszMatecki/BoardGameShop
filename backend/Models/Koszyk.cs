using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Koszyk
{
    public int IdKoszyka { get; set; }

    public int IdUzytkownika { get; set; }

    public DateTime DataOstatniejModyfikacji { get; set; }

    public virtual UzytkownikDetail IdUzytkownikaNavigation { get; set; } = null!;

    public virtual ICollection<PozycjaKoszyka> PozycjaKoszykas { get; set; } = new List<PozycjaKoszyka>();
}
