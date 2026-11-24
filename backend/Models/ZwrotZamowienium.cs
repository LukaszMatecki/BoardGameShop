using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Dodaj ten using!

namespace API.Models;

public partial class ZwrotZamowienium
{
    public int IdZwrotu { get; set; }

    public int IdZamowienia { get; set; }

    public DateTime DataZgloszenia { get; set; }

    public string Powod { get; set; } = null!;

    public bool CzyZaakceptowany { get; set; }

    public int IdTypuZgloszenia { get; set; }
    public int IdStatusuZgloszenia { get; set; }
    public int IdPozycji { get; set; }

    public virtual Zamowienie IdZamowieniaNavigation { get; set; } = null!;


    [ForeignKey("IdPozycji")]
    public virtual PozycjaZamowienium IdPozycjiZamowienia { get; set; } = null!;

    [ForeignKey("IdTypuZgloszenia")]
    public virtual SlownikTypZgloszenia IdTypuZgloszeniaNavigation { get; set; } = null!;

    [ForeignKey("IdStatusuZgloszenia")]
    public virtual SlownikStatusZgloszenia IdStatusuZgloszeniaNavigation { get; set; } = null!;
}