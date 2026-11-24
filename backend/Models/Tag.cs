using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Tag
{
    public int IdTagu { get; set; }

    public string NazwaTagu { get; set; } = null!;

    public virtual ICollection<Produkt> IdProduktus { get; set; } = new List<Produkt>();
}
