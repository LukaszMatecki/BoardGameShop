using System;
using System.Collections.Generic;

namespace API.Models;

public partial class PozycjaZamowienium
{
    public int IdPozycji { get; set; }

    public int IdZamowienia { get; set; }

    public int IdProduktu { get; set; }

    public int Ilosc { get; set; }

    public decimal CenaZakupu { get; set; }

    public string NazwaWMomencieZakupu { get; set; } = null!;

    public virtual Produkt IdProduktuNavigation { get; set; } = null!;

    public virtual Zamowienie IdZamowieniaNavigation { get; set; } = null!;
    public virtual ZwrotZamowienium? ZwrotZamowienium { get; set; }

}
