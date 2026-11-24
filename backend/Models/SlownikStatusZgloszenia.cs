using System.ComponentModel.DataAnnotations;
using API.Models;

public partial class SlownikStatusZgloszenia
{
    [Key]
    public int IdStatusuZgloszenia { get; set; }
    public string Nazwa { get; set; } = null!;
    public virtual ICollection<ZwrotZamowienium> Zwroty { get; set; } = new List<ZwrotZamowienium>();
}