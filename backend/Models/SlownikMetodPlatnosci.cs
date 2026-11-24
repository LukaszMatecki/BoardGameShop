using System;
using System.Collections.Generic;

namespace API.Models;

public partial class SlownikMetodPlatnosci
{
    public int IdMetodyPlatnosci { get; set; }

    public string NazwaPlatnosci { get; set; } = null!;

    public virtual ICollection<Zamowienie> Zamowienies { get; set; } = new List<Zamowienie>();
}
