using System;
using System.Collections.Generic;

namespace API.Models;

public partial class SlownikMetodDostawy
{
    public int IdMetodyDostawy { get; set; }

    public string NazwaMetody { get; set; } = null!;

    public decimal KosztBazowy { get; set; }

    public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();
}
