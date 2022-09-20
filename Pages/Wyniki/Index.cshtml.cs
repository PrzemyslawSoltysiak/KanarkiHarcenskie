using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Klatki
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Klatka> Klatki { get; set; } = default!;

        public IList<string> RazemDodatnie { get; set; } = new List<string>();
        public IList<string> RazemUjemne { get; set; } = new List<string>();
        public IList<string> OcenaOgolna { get; set; } = new List<string>();

        public IList<string> OcenyKolekcji { get; set; } = new List<string>();


        public async Task OnGetAsync()
        {
            if (_context.Klatki != null)
            {
                Klatki = await _context.Klatki
                    .Include(k => k.Kolekcja)
                    .Include(k => k.Kolekcja.Wlasciciel)
                    .Include(k => k.Kolekcja.Konkurs)
                    .Include(k => k.Wyniki)
                    .AsNoTracking().ToListAsync();

                var cechySpiewu = await _context.CechySpiewuCOM.ToListAsync();

                for (int i = 0; i < Klatki.Count(); ++i)
                {
                    // razem punkty dodatnie
                    if (!Klatki[i].Wyniki.ToList().Where(w =>
                        (cechySpiewu.Where(c => (c.Nazwa == w.NazwaCechySpiewu &&
                                           c.WagaPunktow == WagaPunktow.Dodatnie)))
                        .Any()).Any())
                    {
                        RazemDodatnie.Add("-");
                    }
                    else
                    {
                        int razemDodatnie = Klatki[i].Wyniki.ToList().Where(w =>
                        (cechySpiewu.Where(c => (c.Nazwa == w.NazwaCechySpiewu &&
                                                 c.WagaPunktow == WagaPunktow.Dodatnie))).Any())
                        .Sum(w => w.PrzyznanePunkty);

                        RazemDodatnie.Add(razemDodatnie.ToString());
                    }

                    // razem punkty ujemne
                    if (!Klatki[i].Wyniki.ToList().Where(w =>
                        (cechySpiewu.Where(c => (c.Nazwa == w.NazwaCechySpiewu &&
                                           c.WagaPunktow == WagaPunktow.Ujemne)))
                        .Any()).Any())
                    {
                        RazemUjemne.Add("-");
                    }
                    else
                    {
                        int razemDodatnie = Klatki[i].Wyniki.ToList().Where(w =>
                        (cechySpiewu.Where(c => (c.Nazwa == w.NazwaCechySpiewu &&
                                                 c.WagaPunktow == WagaPunktow.Ujemne))).Any())
                        .Sum(w => w.PrzyznanePunkty);

                        RazemUjemne.Add(razemDodatnie.ToString());
                    }

                    // oceny ogólne
                    if (RazemDodatnie[i] == "-" && RazemUjemne[i] == "-")
                    {
                        OcenaOgolna.Add("-");
                    }
                    else
                    {
                        int dodatnie = RazemDodatnie[i] != "-" ? Int32.Parse(RazemDodatnie[i]) : 0;
                        int ujemne = RazemUjemne[i] != "-" ? Int32.Parse(RazemUjemne[i]) : 0;

                        OcenaOgolna.Add((dodatnie - ujemne).ToString());
                    }
                }

                // oceny Kolekcji
                for (int i = 0; i < _context.Klatki.Count(); i += 4)
                {
                    if (OcenaOgolna[i] == "-" && OcenaOgolna[i + 1] == "-" && OcenaOgolna[i + 2] == "-" && OcenaOgolna[i + 3] == "-")
                    {
                        OcenyKolekcji.Add("-");
                        continue;
                    }
                    else
                    {
                        int ocenaKolekcji = 0;

                        if (OcenaOgolna[i] != "-")
                        {
                            ocenaKolekcji += Int32.Parse(OcenaOgolna[i]);
                        }
                        if (OcenaOgolna[i + 1] != "-")
                        {
                            ocenaKolekcji += Int32.Parse(OcenaOgolna[i + 1]);
                        }
                        if (OcenaOgolna[i + 2] != "-")
                        {
                            ocenaKolekcji += Int32.Parse(OcenaOgolna[i + 2]);
                        }
                        if (OcenaOgolna[i + 3] != "-")
                        {
                            ocenaKolekcji += Int32.Parse(OcenaOgolna[i + 3]);
                        }

                        OcenyKolekcji.Add(ocenaKolekcji.ToString());
                    }
                }
            }
        }
    }
}
