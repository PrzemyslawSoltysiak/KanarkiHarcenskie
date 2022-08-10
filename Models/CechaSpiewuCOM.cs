using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KanarkiHercenskie.Models
{
    public enum WagaPunktow
    {
        Dodatnie = 1,
        Ujemne = -1
    }

    public class CechaSpiewuCOM
    {
        [Key]
        public string Nazwa { get; set; } = null!;

        [Display(Name = "Maks. liczba punktów")]
        public uint MaksPunktow { get; set; }

        [Display(Name = "Waga punktów")]
        public WagaPunktow WagaPunktow { get; set; }

        public ICollection<Wynik> Wyniki { get; set; } = new HashSet<Wynik>();
    }
}
