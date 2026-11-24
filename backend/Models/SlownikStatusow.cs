using System;
using System.Collections.Generic;

namespace API.Models;

public partial class SlownikStatusow
{
    public int IdStatusu { get; set; }

    public string NazwaStatusu { get; set; } = null!;

    public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();
}
