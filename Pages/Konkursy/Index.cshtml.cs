using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Konkursy
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Konkurs> Konkursy { get;set; } = default!;

        public Konkurs WybranyKonkurs = null;
        public IList<Wynik> WynikiKonkursu = null;

        public string SortujWedlugCIH = "CIH_rosnaco";
        public string SortujWedlugPrzesluchan = "NajwczesniejszePrzesluchanie";
        public string SortujWedlugWynikow = "NajlepszyWynik";

        public async Task OnGetAsync(int? id, string? sortuj)
        {
            if (_context.Konkursy != null)
            {
                Konkursy = await _context.Konkursy.AsNoTracking().ToListAsync();

                if (id != null)
                {
                    var wybranyKonkurs = _context.Konkursy
                        .Include(k => k.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Klatki)
                        .Include(k => k.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Wlasciciel)
                        .Include(k => k.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Przesluchanie)
                        .FirstOrDefault(konkurs => konkurs.ID == id);

                    if (wybranyKonkurs != null)
                    {
                        WybranyKonkurs = wybranyKonkurs;
                        WynikiKonkursu = await _context.Wyniki
                            .Include(w => w.PrzyznanoZa)
                            .Include(w => w.PrzyznanoDla)
                            .Where(w => w.PrzyznanoDla.Kolekcja.Konkurs.ID == id)
                            .AsNoTracking().ToListAsync();

                        if (sortuj != null)
                        {
                            switch (sortuj)
                            {
                                case "CIH_rosnaco":
                                    SortujWedlugCIH = "CIH_malejaco";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderBy(k => k.SygnumWlasciciela).ToList();
                                    break;
                                case "CIH_malejaco":
                                    SortujWedlugCIH = "CIH_rosnaco";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderByDescending(k => k.SygnumWlasciciela).ToList();
                                    break;
                                case "NajwczesniejszePrzesluchanie":
                                    SortujWedlugPrzesluchan = "NajpozniejszePrzesluchanie";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderBy(k => k.Przesluchanie.Data)
                                        .ThenBy(k => k.Przesluchanie.GodzinaOd).ToList();
                                    break;
                                case "NajpozniejszePrzesluchanie":
                                    SortujWedlugPrzesluchan = "NajwczesniejszePrzesluchanie";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderByDescending(k => k.Przesluchanie.Data)
                                        .ThenByDescending(k => k.Przesluchanie.GodzinaOd).ToList();
                                    break;
                                case "NajlepszyWynik":
                                    SortujWedlugWynikow = "NajgorszyWynik";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderByDescending(k => _context.Wyniki
                                            .Where(w => w.PrzyznanoDla.ID_Kolekcji == k.ID)
                                            .Sum(w => w.PrzyznanePunkty * (int)(w.PrzyznanoZa.WagaPunktow)))
                                        .ToList();
                                    break;
                                case "NajgorszyWynik":
                                    SortujWedlugWynikow = "NajlepszyWynik";
                                    WybranyKonkurs.ZgloszoneKolekcje = WybranyKonkurs.ZgloszoneKolekcje
                                        .OrderBy(k => _context.Wyniki
                                            .Where(w => w.PrzyznanoDla.ID_Kolekcji == k.ID)
                                            .Sum(w => w.PrzyznanePunkty * (int)(w.PrzyznanoZa.WagaPunktow)))
                                        .ToList();
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
