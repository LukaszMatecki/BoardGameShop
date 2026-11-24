using System;
using System.Collections.Generic;

namespace API.Models;

public partial class OpiniaProduktu
{
    public int IdOpinii { get; set; }

    public int IdProduktu { get; set; }

    public int IdUzytkownika { get; set; }

    public int Ocena { get; set; }

    public string? Komentarz { get; set; }

    public DateTime DataWystawienia { get; set; }

    public bool CzyZatwierdzona { get; set; }

    public virtual Produkt IdProduktuNavigation { get; set; } = null!;

    public virtual UzytkownikDetail IdUzytkownikaNavigation { get; set; } = null!;
}
