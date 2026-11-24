using System;
using System.Collections.Generic;

namespace API.Models;

public partial class ZapytanieOProdukt
{
    public int IdZapytania { get; set; }

    public int? IdProduktu { get; set; }

    public string EmailPytajacego { get; set; } = null!;

    public string TrescZapytania { get; set; } = null!;

    public DateTime DataZapytania { get; set; }

    public bool CzyObsluzone { get; set; }

    public virtual Produkt? IdProduktuNavigation { get; set; }
}
