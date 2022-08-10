using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KanarkiHercenskie.Models
{
    public class Wynik
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Identyfikator klatki")]
        public int ID_Klatki { get; set; }

        [ForeignKey("ID_Klatki")]
        public Klatka PrzyznanoDla { get; set; } = null!;

        [Display(Name = "Nazwa cechy śpiewu")]
        public string NazwaCechySpiewu { get; set; } = null!;

        [ForeignKey("NazwaCechySpiewu")]
        public CechaSpiewuCOM PrzyznanoZa { get; set; } = null!;

        public int PrzyznanePunkty { get; set; }
    }
}
