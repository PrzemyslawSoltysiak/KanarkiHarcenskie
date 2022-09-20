using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;

namespace KanarkiHarcenskie.Models
{
    public class Wynik : IValidatableObject
    {
        private readonly ApplicationDbContext _context;

        public Wynik(ApplicationDbContext context)
        {
            _context = context;
        }

        public Wynik() { }


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
        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var wynik = _context.Wyniki.Include(w => w.PrzyznanoZa).Where(w => w.ID == ID).First();

            if (wynik.PrzyznanePunkty > wynik.PrzyznanoZa.MaksPunktow)
            {
                yield return new ValidationResult(
                    "Liczba punktów nie może przekraczać maksymalnej liczby punków za daną Cechę Śpiewu.",
                    new[] { nameof(PrzyznanePunkty) });
            }
        }
    }
}
