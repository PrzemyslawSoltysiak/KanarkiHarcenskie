using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.KartaOceny
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public Konkurs Konkurs { get; set; } = default!;


        [BindProperty]
        public Hodowca Hodowca { get; set; } = default!;

        [BindProperty]
        public string ImieNazwiskoHodowcy { get; set; }


        [BindProperty]
        public Kolekcja Kolekcja { get; set; } = default!;

        [BindProperty]
        public int[] NumeryObraczekRodowych { get; set; } = new int[4];

        [BindProperty]
        public Przesluchanie Przesluchanie { get; set; } = default!;


        public IList<CechaSpiewuCOM> CechyDodatnie { get; set; } = default!;
        public IList<CechaSpiewuCOM> CechyUjemne { get; set; } = default!;


        [BindProperty]
        public WierszWynikow[] WynikiDodatnie { get; set; }

        [BindProperty]
        public WierszWynikow[] WynikiUjemne { get; set; }


        public bool BledneImieNazwisko { get; set; } = false;


        public async Task<IActionResult> OnGetAsync(
            int? idKonkursu = null, string? sygnumHodowcy = null)
        {
            // WIELKI TEST NULLÓW
            if (WielkiTestNullow())
            {
                return NotFound();
            }

            // pobierz informacje o cechach œpiewu
            CechyDodatnie = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Dodatnie)
                .OrderByDescending(c => c.MaksPunktow).ToList();
            CechyUjemne = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Ujemne)
                .OrderByDescending(c => c.MaksPunktow).ToList();

            // utwórz pola tabel do przechowywania wyników
            // za cechy punktowane dodatnio
            WynikiDodatnie = new WierszWynikow[CechyDodatnie.Count()];
            for (int i = 0; i < CechyDodatnie.Count(); ++i)
            {
                WynikiDodatnie[i] = new WierszWynikow(CechyDodatnie[i].Nazwa);
            }
            // za cechy puntkowane ujemnie
            WynikiUjemne = new WierszWynikow[CechyUjemne.Count()];
            for (int i = 0; i < CechyUjemne.Count(); ++i)
            {
                WynikiUjemne[i] = new WierszWynikow(CechyUjemne[i].Nazwa);
            }

            // jeœli zosta³ wskazany Konkurs, to sprawdŸ, czy znajduje siê on w BD
            if (idKonkursu != null)
            {
                if (!await _context.Konkursy.AnyAsync(k => k.ID == idKonkursu))
                {
                    return NotFound();
                }

                Konkurs = (await _context.Konkursy.FindAsync(idKonkursu))!;
            }
            // jeœli nie wskazano Konkursu, to utwórz nowy
            else
            {
                Konkurs = new Konkurs() { Data = DateTime.Now.Date };
            }

            // jeœli zosta³ wskazany Hodowca, to sprawdŸ, czy znajduje siê on w BD
            if (sygnumHodowcy != null)
            {
                if (!await _context.Hodowcy.AnyAsync(h => h.SygnumHodowcy == sygnumHodowcy))
                {
                    return NotFound();
                }

                Hodowca = (await _context.Hodowcy.FindAsync(sygnumHodowcy))!;
                ImieNazwiskoHodowcy = Hodowca.Imie + " " + Hodowca.Nazwisko;
            }
            // jeœli nie wskazano Hodowcy, to utwórz nowego
            else
            {
                Hodowca = new Hodowca();
            }
            
            // jeœli wskazano zarówno Konkurs jak i Hodowcê (i znajduj¹ siê one w BD),
            // to sprawdŸ, czy istnieje odpowiednia Kolekcja
            if (idKonkursu != null && sygnumHodowcy != null &&
                await _context.Kolekcje.AnyAsync(k => (k.ID_Konkursu == idKonkursu &&
                                                       k.SygnumWlasciciela == sygnumHodowcy)))
            {
                Kolekcja = await _context.Kolekcje.Include(k => k.Klatki).Include(k => k.Przesluchanie)
                    .FirstAsync(k => (k.ID_Konkursu == idKonkursu && k.SygnumWlasciciela == sygnumHodowcy));

                NumeryObraczekRodowych[0] = Kolekcja.Klatki[0].NrObraczkiRodowej;
                NumeryObraczekRodowych[1] = Kolekcja.Klatki[1].NrObraczkiRodowej;
                NumeryObraczekRodowych[2] = Kolekcja.Klatki[2].NrObraczkiRodowej;
                NumeryObraczekRodowych[3] = Kolekcja.Klatki[3].NrObraczkiRodowej;
            }
            // jeœli nie istnieje odpowiednia Kolekcja, to utwórz j¹
            else
            {
                Kolekcja = new Kolekcja() { Wlasciciel = Hodowca, Konkurs = this.Konkurs };
            }

            // jeœli Kolekcja nie posiada jeszcze przypisanego Przes³uchania,
            // to utwórz nowe Przes³uchanie i przypisz je do Kolekcji
            if (Kolekcja.Przesluchanie == null)
            {
                Przesluchanie = NowePrzesluchanie(Kolekcja);
                Kolekcja.Przesluchanie = Przesluchanie;
            }
            // jeœli Kolekcja posiada ju¿ Przes³uchanie, to pobierz informacje
            else
            {
                Przesluchanie = Kolekcja.Przesluchanie;
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // WIELKI TEST NULLÓW
            if (WielkiTestNullow())
                return NotFound();

            if (Konkurs.Miejscowosc == null || (ImieNazwiskoHodowcy == null && Hodowca.SygnumHodowcy == null))
            {
                return PobierzCechyPotemReturnPage();
            }

            bool nowyKonkursLubHodowca = false;

            // sprawdŸ, czy istnieje Konkurs o podanych w³aœciwoœciach
            var konkurs = await _context.Konkursy
                .Where(k => (k.Ranga == Konkurs.Ranga &&
                             k.Miejscowosc == Konkurs.Miejscowosc &&
                             k.Data.Date == Konkurs.Data.Date)).FirstOrDefaultAsync();

            // jeœli nie, utwórz nowy Konkurs i dodaj go do bazy danych
            if (konkurs == null)
            {
                nowyKonkursLubHodowca = true;

                konkurs = new Konkurs()
                {
                    Ranga = Konkurs.Ranga,
                    Miejscowosc = Konkurs.Miejscowosc,
                    Data = Konkurs.Data.Date
                };
                _context.Konkursy.Add(konkurs);
                await _context.SaveChangesAsync();
            }

            // sprawdŸ, czy istnieje Hodowca o podanym Sygnum
            var hodowca = await _context.Hodowcy
                .Where(h => h.SygnumHodowcy == Hodowca.SygnumHodowcy).FirstOrDefaultAsync();

            string imieHodowcy = ImieNazwiskoHodowcy.Split(' ')[0];
            string nazwiskoHodowcy = ImieNazwiskoHodowcy.Split(' ')[1];

            // jeœli Hodowca istnieje, sprawdŸ, czy jego dane siê zgadzaj¹
            if (hodowca != null)
            {
                if (hodowca.Imie != imieHodowcy || hodowca.Nazwisko != nazwiskoHodowcy)
                {
                    BledneImieNazwisko = true;
                    return PobierzCechyPotemReturnPage();
                }
                else
                {
                    BledneImieNazwisko = false;
                }

            }
            // jeœli Hodowca nie istnieje, utwórz nowego Hodowcê i dodaj go do bazy danych
            else
            {
                nowyKonkursLubHodowca = true;

                hodowca = new Hodowca()
                {
                    SygnumHodowcy = await Hodowca.WyznaczSygnumAsync(_context, nazwiskoHodowcy),
                    Imie = imieHodowcy,
                    Nazwisko = nazwiskoHodowcy
                };
                _context.Hodowcy.Add(hodowca);
                await _context.SaveChangesAsync();
            }

            // jeœli przez formularz nie zarejestrowano Konkursu ani Hodowcy,
            // sprawdŸ, czy Hodowca zarejestrowa³ Kolekcjê na dany Konkurs
            Kolekcja kolekcja = null;
            if (nowyKonkursLubHodowca == false)
            {
                kolekcja = await _context.Kolekcje.Include(k => k.Klatki).Include(k => k.Przesluchanie)
                    .Where(k => (k.SygnumWlasciciela == hodowca.SygnumHodowcy &&
                                 k.ID_Konkursu == konkurs.ID)).FirstOrDefaultAsync();

                // jeœli tak, sprawdŸ czy zgadzaj¹ siê Numery obr¹czek rodowych,
                // oraz czy Kolekcja nie zosta³a ju¿ oceniona
                if (kolekcja != null)
                {
                    SprawdzNumeryObraczekRodowych(kolekcja);

                    if (_context.Wyniki.Where(w => w.PrzyznanoDla.ID_Kolekcji == kolekcja.ID).Any())
                    {
                        throw new Exception("W BD istniej¹ ju¿ Wyniki przypisane do Klatek wchodz¹cych w sk³ad wskazanej Kolekcji.");
                    }
                }
            }

            // jeœli przez formularz zarejestrowano Konkurs lub Hodowcê,
            // lub Hodowca nie zarejestrowa³ uprzednio Kolekcji na dany Konkurs
            // to uwtórz now¹ Kolekcjê (wraz z Klatkami) i dodaj do bazy danych
            if (kolekcja == null)
            {
                kolekcja = new Kolekcja()
                {
                    SygnumWlasciciela = hodowca.SygnumHodowcy,
                    ID_Konkursu = konkurs.ID,
                };

                kolekcja.DodajKlatki(NumeryObraczekRodowych[0], NumeryObraczekRodowych[1],
                                     NumeryObraczekRodowych[2], NumeryObraczekRodowych[3]);

                await _context.Kolekcje.AddAsync(kolekcja);
                await _context.SaveChangesAsync();
            }

            // sprawdŸ, czy Kolekcja ma przypisane Przes³uchanie
            // jeœli tak, sprawdŸ czy pola Data i Godzina Od siê zgadzaj¹ (jeœli s¹ wype³nione)
            // jeœli nie, utwórz nowe Przes³uchanie
            if (kolekcja.Przesluchanie != null)
            {
                if ((kolekcja.Przesluchanie.Data != null && Przesluchanie.Data != kolekcja.Przesluchanie.Data) ||
                    (kolekcja.Przesluchanie.Data != null && Przesluchanie.GodzinaOd != kolekcja.Przesluchanie.GodzinaOd))
                {
                    throw new Exception("Data lub Godzina Od Przes³uchania nie pokrywaj¹ siê z danymi w BD.");
                }
                    

                if (kolekcja.Przesluchanie.Data != null && Przesluchanie.GodzinaDo != kolekcja.Przesluchanie.GodzinaDo)
                    kolekcja.Przesluchanie.GodzinaDo = Przesluchanie.GodzinaDo;
            }
            else
            {
                kolekcja.Przesluchanie = Przesluchanie;
                kolekcja.Przesluchanie.ID_Kolekcji = kolekcja.ID;

                await _context.Przesluchania.AddAsync(kolekcja.Przesluchanie);
                await _context.SaveChangesAsync();
            }

            // pobierz informacje o cechach œpiewu
            CechyDodatnie = await _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Dodatnie).ToListAsync();
            CechyUjemne = await _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Ujemne).ToListAsync();

            // sprawdŸ, czy zosta³y wstawione wyniki,
            // jeœli tak, dodaj je do bazy danych
            bool bledneWyniki = false;
            for (int i = 0; i < CechyDodatnie.Count(); ++i)
            {
                WynikiDodatnie[i].NazwaCechySpiewu = CechyDodatnie[i].Nazwa;
                if (!WynikiDodatnie[i].Waliduj(_context))
                {
                    WynikiDodatnie[i].DodajKolekcje(kolekcja);
                    await _context.AddRangeAsync(WynikiDodatnie[i][0], WynikiDodatnie[i][1], WynikiDodatnie[i][2], WynikiDodatnie[i][3]);
                }
                else
                {
                    bledneWyniki = true;
                }
            }
            for (int i = 0; i < CechyUjemne.Count(); ++i)
            {
                WynikiUjemne[i].NazwaCechySpiewu = CechyUjemne[i].Nazwa;
                if (!WynikiUjemne[i].Waliduj(_context))
                {
                    WynikiUjemne[i].DodajKolekcje(kolekcja);
                    await _context.AddRangeAsync(WynikiUjemne[i][0], WynikiUjemne[i][1], WynikiUjemne[i][2], WynikiUjemne[i][3]);
                }
                else
                {
                    bledneWyniki = true;
                }
            }

            if (bledneWyniki)
            {
                return Page();
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();

            return Page();
        }

        private bool WielkiTestNullow()
        {
            return (_context == null || _context.Konkursy == null || _context.Hodowcy == null ||
                    _context.Kolekcje == null || _context.Klatki == null || _context.Przesluchania == null ||
                    _context.CechySpiewuCOM == null || _context.Wyniki == null);
        }
        private void SprawdzNumeryObraczekRodowych(Kolekcja kolekcjaBD)
        {
            // Klatka nr 1
            if (kolekcjaBD.Klatki[0].NrObraczkiRodowej != NumeryObraczekRodowych[0])
            {
                throw new Exception("Nr obr¹czki rodowej ptaka w Klatce nr 1 " +
                                    "nie pokrywa siê z numerem w bazie danych.");
            }

            // Klatka nr 2
            if (kolekcjaBD.Klatki[1].NrObraczkiRodowej != NumeryObraczekRodowych[1])
            {
                throw new Exception("Nr obr¹czki rodowej ptaka w Klatce nr 2 " +
                                    "nie pokrywa siê z numerem w bazie danych.");
            }

            // Klatka nr 3
            if (kolekcjaBD.Klatki[2].NrObraczkiRodowej != NumeryObraczekRodowych[2])
            {
                throw new Exception("Nr obr¹czki rodowej ptaka w Klatce nr 3 " +
                                    "nie pokrywa siê z numerem w bazie danych.");
            }

            // Klatka nr 4
            if (kolekcjaBD.Klatki[3].NrObraczkiRodowej != NumeryObraczekRodowych[3])
            {
                throw new Exception("Nr obr¹czki rodowej ptaka w Klatce nr 4 " +
                                    "nie pokrywa siê z numerem w bazie danych.");
            }
        }

        private Przesluchanie NowePrzesluchanie(Kolekcja kolekcja)
        {
            return new Przesluchanie()
            {
                ID_Kolekcji = kolekcja.ID,
                Data = DateTime.Now.Date,
                GodzinaOd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                         DateTime.Now.Hour, DateTime.Now.Minute, 0),
                GodzinaDo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                         DateTime.Now.Hour, DateTime.Now.Minute, 0).AddMinutes(30)
            };
        }

        private IActionResult PobierzCechyPotemReturnPage()
        {
            // pobierz informacje o cechach œpiewu
            CechyDodatnie = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Dodatnie)
                .OrderByDescending(c => c.MaksPunktow).ToList();
            CechyUjemne = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Ujemne)
                .OrderByDescending(c => c.MaksPunktow).ToList();
            return Page();
        }
    }

}
