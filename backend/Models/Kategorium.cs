using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Kategorium
{
    public int IdKategorii { get; set; }

    public string NazwaKategorii { get; set; } = null!;

    public virtual ICollection<Produkt> IdProduktus { get; set; } = new List<Produkt>();
}
