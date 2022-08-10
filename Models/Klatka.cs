using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;

namespace KanarkiHercenskie.Models
{
    public class Klatka
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Numer Klatki w Kolekcji"), Range(1, 4)]
        public int NrKlatki { get; set; }

        public int ID_Kolekcji { get; set; }
        [ForeignKey("ID_Kolekcji")]
        public Kolekcja Kolekcja { get; set; } = null!;

        [Display(Name = "Numer obrączki rodowej")]
        [Range(1, 999), DisplayFormat(DataFormatString = "{0:D3}", ApplyFormatInEditMode = true)]
        public int NrObraczkiRodowej { get; set; }

        public ICollection<Wynik> Wyniki { get; set; } = new HashSet<Wynik>();

        public string? Uwagi { get; set; }
    }
}
