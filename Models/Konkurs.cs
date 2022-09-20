using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KanarkiHarcenskie.Models
{
    public enum RangaKonkursu
    {
        [Display(Name = "Oddziałowy")] Oddzialowy,
        [Display(Name = "Wojewódzki")] Wojewodzki,
        Krajowy
    }

    public class Konkurs
    {
        [Key]
        public int ID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Display(Name = "Miejsowość")]
        public string Miejscowosc { get; set; } = null!;

        public RangaKonkursu Ranga { get; set; }

        public ICollection<Kolekcja> ZgloszoneKolekcje { get; set; } = new HashSet<Kolekcja>();


        public string Podsumowanie
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return Miejscowosc + ", " + Data.ToShortDateString() + " (" + Ranga + ")";
        }
    }
}
