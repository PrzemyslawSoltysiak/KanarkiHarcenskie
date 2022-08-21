using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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


        public bool BrakMiejscowosci = false;
        public bool BrakDanychHodowcy = false;
        public bool NiepoprawnyFormatDanychHodowcy = false;
        public bool BledneImieNazwisko = false;
        public bool NieZnalezionoHodowcy = false;
        public bool[] BlednyNumerObraczkiRodowej = new bool[4];
        public bool BlednaDataPrzesluchania = false;
        public bool BlednaGodzinaRozpoczeciaPrzesluchania = false;
        public bool KolekcjaJuzOceniona = false;
        public bool[] PrzekroczonoMaksPunktowDodatnich = new bool[4];

        public int[] RazemPunktyDodatnie = new int[4];
        public int[] RazemPunktyUjemne = new int[4];
        public int[] OcenaKoncowa = new int[4];
        public int OcenaKolekcji;

        public string SygnumHodowcy { get; set; }

        public async Task<IActionResult> OnGetAsync(
            int? idKonkursu = null, string? sygnumHodowcy = null)
        {
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
                SygnumHodowcy = Hodowca.SygnumHodowcy;
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
            if (WielkiTestNullow())
                return NotFound();

            BrakMiejscowosci = Konkurs.Miejscowosc == null ? true : false;
            BrakDanychHodowcy = ImieNazwiskoHodowcy == null ? true : false;
            if (BrakMiejscowosci || BrakDanychHodowcy)
            {
                PobierzCechySpiewu();
                return Page();
            }

            NiepoprawnyFormatDanychHodowcy = !Regex.IsMatch(ImieNazwiskoHodowcy, @"^[A-Z][a-z]+\s[A-Z][a-z]+$");
            if (NiepoprawnyFormatDanychHodowcy)
            {
                PobierzCechySpiewu();
                return Page();
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

            NieZnalezionoHodowcy = Hodowca.SygnumHodowcy != null && hodowca == null;
            if (NieZnalezionoHodowcy)
            {
                PobierzCechySpiewu();
                return Page();
            }

            string imieHodowcy = ImieNazwiskoHodowcy.Split(' ')[0];
            string nazwiskoHodowcy = ImieNazwiskoHodowcy.Split(' ')[1];

            // jeœli Hodowca istnieje, sprawdŸ, czy jego dane siê zgadzaj¹
            if (hodowca != null)
            {
                if (hodowca.Imie != imieHodowcy || hodowca.Nazwisko != nazwiskoHodowcy)
                {
                    BledneImieNazwisko = true;
                    PobierzCechySpiewu();
                    return Page();
                }
                else
                {
                    BledneImieNazwisko = false;
                    SygnumHodowcy = hodowca.SygnumHodowcy;
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

                SygnumHodowcy = hodowca.SygnumHodowcy;
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
                    BlednyNumerObraczkiRodowej = SprawdzNumeryObraczekRodowych(kolekcja);

                    for (int i = 0; i < 4; ++i)
                    {
                        if (BlednyNumerObraczkiRodowej[i])
                        {
                            PobierzCechySpiewu();
                            return Page();
                        }
                    }

                    if (_context.Wyniki.Where(w => w.PrzyznanoDla.ID_Kolekcji == kolekcja.ID).Any())
                    {
                        KolekcjaJuzOceniona = true;
                        PobierzCechySpiewu();
                        return Page();
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
                BlednaDataPrzesluchania = kolekcja.Przesluchanie.Data != null && 
                                          Przesluchanie.Data != kolekcja.Przesluchanie.Data 
                    ? true : false;

                BlednaGodzinaRozpoczeciaPrzesluchania = kolekcja.Przesluchanie.GodzinaOd != null && 
                                                        Przesluchanie.GodzinaOd != kolekcja.Przesluchanie.GodzinaOd
                    ? true : false;

                if (BlednaDataPrzesluchania || BlednaGodzinaRozpoczeciaPrzesluchania)
                {
                    PobierzCechySpiewu();
                    return Page();
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
            CechyDodatnie = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Dodatnie)
                .OrderByDescending(c => c.MaksPunktow).ToList();
            CechyUjemne = _context.CechySpiewuCOM.Where(c => c.WagaPunktow == WagaPunktow.Ujemne)
                .OrderByDescending(c => c.MaksPunktow).ToList();

            // sprawdŸ, czy zosta³y wstawione wyniki,
            // jeœli tak, dodaj je do bazy danych
            bool bledneWyniki = false;
            for (int i = 0; i < CechyDodatnie.Count(); ++i)
            {
                WynikiDodatnie[i].NazwaCechySpiewu = CechyDodatnie[i].Nazwa;
                if (!WynikiDodatnie[i].Waliduj(_context))
                {
                    WynikiDodatnie[i].DodajKolekcje(kolekcja);
                    for (int j = 0; j < 4; ++j)
                    {
                        _context.Add(WynikiDodatnie[i][j]);
                    }
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
                    for (int j = 0; j < 4; ++j)
                    {
                        _context.Add(WynikiUjemne[i][j]);
                    }
                }
                else
                {
                    bledneWyniki = true;
                }
            }

            for (int i = 0; i < 4; ++i)
            {
                RazemPunktyDodatnie[i] = WynikiDodatnie.Sum(w => w.PrzyznanePunkty[i]);
                PrzekroczonoMaksPunktowDodatnich[i] = RazemPunktyDodatnie[i] > 90;
                RazemPunktyUjemne[i] = WynikiUjemne.Sum(w => w.PrzyznanePunkty[i]);
                OcenaKoncowa[i] = RazemPunktyDodatnie[i] - RazemPunktyUjemne[i];
            }
            OcenaKolekcji = OcenaKoncowa.Sum();

            if (bledneWyniki || PrzekroczonoMaksPunktowDodatnich.Contains(true))
            {
                PobierzCechySpiewu();
                return Page();
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            PobierzCechySpiewu();
            return Page();
        }

        private bool WielkiTestNullow()
        {
            return (_context == null || _context.Konkursy == null || _context.Hodowcy == null ||
                    _context.Kolekcje == null || _context.Klatki == null || _context.Przesluchania == null ||
                    _context.CechySpiewuCOM == null || _context.Wyniki == null);
        }
        private bool[] SprawdzNumeryObraczekRodowych(Kolekcja kolekcjaBD)
        {
            bool[] rezultat = new bool[4];

            for (int i = 0; i < 4; ++i)
            {
                rezultat[i] = kolekcjaBD.Klatki[i].NrObraczkiRodowej != NumeryObraczekRodowych[i];
            }

            return rezultat;
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

        private async void PobierzCechySpiewu()
		{
            CechyDodatnie = await _context.CechySpiewuCOM
                .Where(c => c.WagaPunktow == WagaPunktow.Dodatnie)
                .OrderByDescending(c => c.MaksPunktow)
                .ToListAsync();

            CechyUjemne = await _context.CechySpiewuCOM
                .Where(c => c.WagaPunktow == WagaPunktow.Ujemne)
                .OrderByDescending(c => c.MaksPunktow)
                .ToListAsync();
        }
    }

}
