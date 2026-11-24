using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Zamowienie
{
    public int IdZamowienia { get; set; }

    public int? IdUzytkownika { get; set; }

    public int IdStatusu { get; set; }

    public int IdMetodyDostawy { get; set; }

    public int IdMetodyPlatnosci { get; set; }

    public DateTime DataZamowienia { get; set; }

    public decimal CalkowitaKwota { get; set; }
    public int? IdAdresuZamowienia { get; set; }

    public string? Imie { get; set; }

    public string? Nazwisko { get; set; }

    public string? Email { get; set; }

    public string? NumerTelefonu { get; set; }

    public virtual Adre? IdAdresuZamowieniaNavigation { get; set; }

    public virtual SlownikMetodDostawy IdMetodyDostawyNavigation { get; set; } = null!;

    public virtual SlownikMetodPlatnosci IdMetodyPlatnosciNavigation { get; set; } = null!;

    public virtual SlownikStatusow IdStatusuNavigation { get; set; } = null!;

    public virtual UzytkownikDetail? IdUzytkownikaNavigation { get; set; }

    public virtual ICollection<PozycjaZamowienium> PozycjaZamowienia { get; set; } = new List<PozycjaZamowienium>();

    public virtual ZwrotZamowienium? ZwrotZamowienium { get; set; }
}
