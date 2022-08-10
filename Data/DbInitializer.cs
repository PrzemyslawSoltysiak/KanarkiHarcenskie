using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            if (context == null)
                throw new Exception("context == null");

            if (!await context.CechySpiewuCOM.AnyAsync())
            {
                await DodajCechySpiewuCOM(context);
            }

            Konkurs konkurs = null;
            if (!await context.Konkursy.AnyAsync())
            {
                konkurs = await DodajPrzykladowyKonkurs(context);
            }

            Hodowca hodowca = null;
            if (!await context.Hodowcy.AnyAsync())
            {
                hodowca = await DodajPrzykladowegoHodowce(context);
            }

            if (konkurs != null && hodowca != null)
            {
                await DodajPrzykladowaKolekcje(context, hodowca, konkurs);
            }
        }

        public static async Task DodajCechySpiewuCOM(ApplicationDbContext context)
        {
            var cechySpiewuCOM = new CechaSpiewuCOM[]
            {
                // cechy śpiewu punktowane dodatnio
                new CechaSpiewuCOM() { Nazwa = "Turkot", MaksPunktow = 27, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Bas", MaksPunktow = 27, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Tura wodna", MaksPunktow = 27, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Dzwonek dęty", MaksPunktow = 18, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Flet", MaksPunktow = 18, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Szokiel", MaksPunktow = 18, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Tokowanie", MaksPunktow = 18, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Dzwonek perlisty", MaksPunktow = 3, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Dzwonek zwykły", MaksPunktow = 3, WagaPunktow = WagaPunktow.Dodatnie },
                new CechaSpiewuCOM() { Nazwa = "Wrażenie", MaksPunktow = 3, WagaPunktow = WagaPunktow.Dodatnie },
                // cechy śpiewu punktowane ujemnie
                new CechaSpiewuCOM() { Nazwa = "Wadliwa tura wodna", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Wadliwa tokowanie", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Wadliwe flety", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Wadliwe dzwonki", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Grzechotki", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Zgrzyt (udzier)", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne },
                new CechaSpiewuCOM() { Nazwa = "Tony nosowe", MaksPunktow = 3, WagaPunktow = WagaPunktow.Ujemne }
            };
            await context.CechySpiewuCOM.AddRangeAsync(cechySpiewuCOM);
            await context.SaveChangesAsync();
        }

        public static async Task<Konkurs> DodajPrzykladowyKonkurs(ApplicationDbContext context)
        {
            var konkurs = new Konkurs()
            {
                Miejscowosc = "Kocie Brody",
                Data = DateTime.Parse("2017-11-23"),
                Ranga = RangaKonkursu.Oddzialowy
            };
            await context.Konkursy.AddAsync(konkurs);
            await context.SaveChangesAsync();
            return konkurs;
        }

        public static async Task<Hodowca> DodajPrzykladowegoHodowce(ApplicationDbContext context)
        {
            var hodowca = new Hodowca()
            {
                SygnumHodowcy = "P172",
                Imie = "Bazyl",
                Nazwisko = "Ponury"
            };
            await context.Hodowcy.AddAsync(hodowca);
            await context.SaveChangesAsync();
            return hodowca;
        }

        public static async Task DodajPrzykladowaKolekcje(ApplicationDbContext context, Hodowca hodowca, Konkurs konkurs)
        {
            var kolekcja = new Kolekcja()
            {
                Wlasciciel = hodowca,
                Konkurs = konkurs,
                Przesluchanie = new Przesluchanie()
                {
                    Data = DateTime.Parse("2017-11-23"),
                    GodzinaOd = DateTime.Parse("11:30"),
                    GodzinaDo = DateTime.Parse("12:00"),
                },
                Klatki = new Klatka[]
                {
                    new Klatka() { NrKlatki = 1, NrObraczkiRodowej = 077 },
                    new Klatka() { NrKlatki = 2, NrObraczkiRodowej = 043 },
                    new Klatka() { NrKlatki = 3, NrObraczkiRodowej = 054 },
                    new Klatka() { NrKlatki = 4, NrObraczkiRodowej = 046 }
                }
            };
            await context.Kolekcje.AddAsync(kolekcja);
            await context.SaveChangesAsync();
        }
    }
}
