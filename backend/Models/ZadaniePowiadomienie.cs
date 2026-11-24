using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ZadaniePowiadomienie
{
    public int IdZadania { get; set; }

    public string Tytul { get; set; } = null!;

    public string Tresc { get; set; } = null!;

    public int IdPrzypisanegoAdmina { get; set; }

    public string Status { get; set; } = null!;

    public DateTime DataUtworzenia { get; set; }

    public virtual UzytkownikDetail IdPrzypisanegoAdminaNavigation { get; set; } = null!;
}
