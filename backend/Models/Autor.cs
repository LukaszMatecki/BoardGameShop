using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Autor
{
    public int IdAutora { get; set; }

    public string Imie { get; set; } = null!;

    public string Nazwisko { get; set; } = null!;

    public virtual ICollection<Produkt> Produkts { get; set; } = new List<Produkt>();
}
