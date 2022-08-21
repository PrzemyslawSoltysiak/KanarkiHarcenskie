using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Hodowcy
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Hodowca> Hodowcy { get; set; } = default!;

        public Hodowca WybranyHodowca = null;
        public IList<Wynik> WynikiHodowcy = null;

        public async Task OnGetAsync(string? cih)
        {
            if (_context.Hodowcy != null)
            {
                Hodowcy = await _context.Hodowcy
                    .Include(h => h.ZgloszoneKolekcje)
                    .AsNoTracking().ToListAsync();

                if (cih != null)
                {
                    var wybranyHodowca = await _context.Hodowcy
                        .Include(h => h.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Klatki)
                        .Include(h => h.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Konkurs)
                        .Include(h => h.ZgloszoneKolekcje)
                            .ThenInclude(k => k.Przesluchanie)
                        .FirstOrDefaultAsync(h => h.SygnumHodowcy == cih);

                    if (wybranyHodowca != null)
                    {
                        WybranyHodowca = wybranyHodowca;
                        WynikiHodowcy = await _context.Wyniki
                            .Include(w => w.PrzyznanoZa)
                            .Include(w => w.PrzyznanoDla)
                            .Where(w => w.PrzyznanoDla.Kolekcja.SygnumWlasciciela == cih)
                            .AsNoTracking().ToListAsync();
                    }
                }
            }
        }
    }
}
