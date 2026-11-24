using System;
using System.Collections.Generic;

namespace API.Models;

public partial class PozycjaKoszyka
{
    public int IdPozycji { get; set; }

    public int IdKoszyka { get; set; }

    public int IdProduktu { get; set; }

    public int Ilosc { get; set; }

    public virtual Koszyk IdKoszykaNavigation { get; set; } = null!;

    public virtual Produkt IdProduktuNavigation { get; set; } = null!;
}
