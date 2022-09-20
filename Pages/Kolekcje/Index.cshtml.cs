using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Kolekcje
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Kolekcja> Kolekcje { get; set; } = default!;

        public string[] OcenyKolekcji { get; set; } = null!;

        public async Task OnGetAsync()
        {
            if (_context.Kolekcje != null)
            {
                Kolekcje = await _context.Kolekcje
                .Include(k => k.Konkurs)
                .Include(k => k.Wlasciciel)
                .Include(k => k.Klatki)
                .AsNoTracking().ToListAsync();

                OcenyKolekcji = new string[Kolekcje.Count()];

                var wyniki = await _context.Wyniki.Include(w => w.PrzyznanoZa).Include(w => w.PrzyznanoDla).ToListAsync();

                for (int i = 0; i < Kolekcje.Count(); ++i)
                {
                    if (!wyniki.Where(w => w.PrzyznanoDla.ID_Kolekcji == Kolekcje[i].ID).Any())
                    {
                        OcenyKolekcji[i] = "-";
                    }
                    else
                    {
                        var wynikiKolekcji = wyniki.Where(w => w.PrzyznanoDla.ID_Kolekcji == Kolekcje[i].ID);

                        int dodatnieKolekcji = wynikiKolekcji.Where(w => w.PrzyznanoZa.WagaPunktow == WagaPunktow.Dodatnie)
                            .Sum(w => w.PrzyznanePunkty);

                        int ujemneKolekcji = wynikiKolekcji.Where(w => w.PrzyznanoZa.WagaPunktow == WagaPunktow.Ujemne)
                            .Sum(w => w.PrzyznanePunkty);

                        OcenyKolekcji[i] = (dodatnieKolekcji - ujemneKolekcji).ToString();
                    }
                }
            }
        }
    }
}
