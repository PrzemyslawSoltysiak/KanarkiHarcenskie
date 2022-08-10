using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KanarkiHercenskie.Models
{
    public class Kolekcja
    {
        [Key]
        public int ID { get; set; }

        public string SygnumWlasciciela { get; set; } = null!;
        [ForeignKey("SygnumWlasciciela")]
        public Hodowca Wlasciciel { get; set; } = null!;

        public int ID_Konkursu { get; set; }
        [ForeignKey("ID_Konkursu")]
        public Konkurs Konkurs { get; set; } = null!;

        public Przesluchanie? Przesluchanie { get; set; }

        public IList<Klatka> Klatki { get; set; } = new List<Klatka>(4);


        public void DodajKlatki(int[] numeryObraczekRodowych)
        {
            if (numeryObraczekRodowych.Length < 4)
                throw new Exception("Należy podać 4 numery obrączek rodowych.");

            DodajKlatki(numeryObraczekRodowych[0], numeryObraczekRodowych[1],
                        numeryObraczekRodowych[2], numeryObraczekRodowych[3]);
        }

        public void DodajKlatki(int nrObraczkiRodowej1, int nrObraczkiRodowej2,
                                int nrObraczkiRodowej3, int nrObraczkiRodowej4)
        {
            if (Klatki.Count() != 0)
                throw new Exception("Do Kolekcji są już przypisane Klatki");

            Klatki.Add(new Klatka()
            {
                NrKlatki = 1,
                NrObraczkiRodowej = nrObraczkiRodowej1
            });

            Klatki.Add(new Klatka()
            {
                NrKlatki = 2,
                NrObraczkiRodowej = nrObraczkiRodowej2
            });

            Klatki.Add(new Klatka()
            {
                NrKlatki = 3,
                NrObraczkiRodowej = nrObraczkiRodowej3
            });

            Klatki.Add(new Klatka()
            {
                NrKlatki = 4,
                NrObraczkiRodowej = nrObraczkiRodowej4
            });
        }

        public void AktualizujNumeryObraczekRodowych(int nrObraczkiRodowej1, int nrObraczkiRodowej2,
                                                     int nrObraczkiRodowej3, int nrObraczkiRodowej4)
        {
            Klatki[0].NrObraczkiRodowej = nrObraczkiRodowej1;
            Klatki[1].NrObraczkiRodowej = nrObraczkiRodowej2;
            Klatki[2].NrObraczkiRodowej = nrObraczkiRodowej3;
            Klatki[3].NrObraczkiRodowej = nrObraczkiRodowej4;
        }
    }
}
