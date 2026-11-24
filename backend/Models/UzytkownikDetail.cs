using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UzytkownikDetail
{
    public int IdUzytkownika { get; set; }
    public string? Imie { get; set; }
    public string? Nazwisko { get; set; }
    public string? NumerTelefonu { get; set; }
    public bool CzyUsuniety { get; set; }
    public DateTime DataRejestracji { get; set; }

    public AppUser User { get; set; } = null!;

    public virtual ICollection<Adre> Adres { get; set; } = new List<Adre>();

    public virtual Koszyk? Koszyk { get; set; }

    public virtual ICollection<OpiniaProduktu> OpiniaProduktus { get; set; } = new List<OpiniaProduktu>();

    public virtual ICollection<ZadaniePowiadomienie> ZadaniePowiadomienies { get; set; } = new List<ZadaniePowiadomienie>();

    public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();

    public virtual ICollection<Produkt> IdProduktus { get; set; } = new List<Produkt>();

    public virtual ICollection<KodPromocyjny> KodyPromocyjne { get; set; } = new List<KodPromocyjny>();
}
