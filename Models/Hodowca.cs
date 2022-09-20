using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

using KanarkiHarcenskie.Data;

namespace KanarkiHarcenskie.Models
{
    public class Hodowca
    {
        [Key, StringLength(4), RegularExpression(@"^[A-Z][0-9]{3}$")]
        [Display(Name = "Sygnum Hodowcy (CIH)")]
        public string SygnumHodowcy { get; set; } = null!;

        [Display(Name = "Imię")]
        public string Imie { get; set; } = null!;

        public string Nazwisko { get; set; } = null!;

        public ICollection<Kolekcja> ZgloszoneKolekcje { get; set; } = new HashSet<Kolekcja>();

        public string Podsumowanie
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return Imie + " " + Nazwisko + " (CIH: " + SygnumHodowcy + ")";
        }

        // Funkcja generująca nowe Sygnum Hodowcy na podstawie jego nazwiska
        public async Task<string> WyznaczSygnumAsync(ApplicationDbContext context, string nazwisko)
        {
            var hodowcyTenSamInicjal = (await context.Hodowcy.ToListAsync())
                .Where(h => h.Nazwisko[0] == nazwisko[0]);

            if (!hodowcyTenSamInicjal.Any())
            {
                return nazwisko[0] + "001";
            }

            var poprzedniHodowca = hodowcyTenSamInicjal.OrderByDescending(h => h.SygnumHodowcy).First();
            int id = Int32.Parse(poprzedniHodowca.SygnumHodowcy.Substring(1)) + 1;

            if (id > 999)
                throw new Exception("COUNT(Hodowcy WHERE Nazwisko[0] == nazwisko[0]) == 999");

            return nazwisko[0] + String.Format("{0:D3}", id);
        }
    }
}
