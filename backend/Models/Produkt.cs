using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Produkt
{
    public int IdProduktu { get; set; }

    public string NazwaProduktu { get; set; } = null!;

    public int IdAutora { get; set; }

    public decimal Cena { get; set; }

    public int PromocjaWProc { get; set; } = 0;

    public decimal? NajnizszaCenaW30Dni { get; set; }

    public DateTime? DataNajnizszejCenyW30Dni { get; set; }


    public int StanMagazynowy { get; set; }

    public DateOnly DataWydania { get; set; }

    public string? Opis { get; set; }

    public string? Zdjecie { get; set; }

    public bool CzyUkryty { get; set; }

    public virtual Autor IdAutoraNavigation { get; set; } = null!;

    public virtual ICollection<OpiniaProduktu> OpiniaProduktus { get; set; } = new List<OpiniaProduktu>();

    public virtual ICollection<PozycjaKoszyka> PozycjaKoszykas { get; set; } = new List<PozycjaKoszyka>();

    public virtual ICollection<PozycjaZamowienium> PozycjaZamowienia { get; set; } = new List<PozycjaZamowienium>();

    public virtual ICollection<ZapytanieOProdukt> ZapytanieOProdukts { get; set; } = new List<ZapytanieOProdukt>();

    public virtual ICollection<Kategorium> IdKategoriis { get; set; } = new List<Kategorium>();

    public virtual ICollection<Tag> IdTagus { get; set; } = new List<Tag>();

    public virtual ICollection<UzytkownikDetail> IdUzytkownikas { get; set; } = new List<UzytkownikDetail>();

    public decimal CenaPoPromocji => PromocjaWProc > 0 
        ? Cena * (1 - (decimal)PromocjaWProc / 100) 
        : Cena;
}
