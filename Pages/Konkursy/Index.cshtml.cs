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
        public IList<Kolekcja> ZgloszoneKolekcje { get; set; } = null;

        public async Task OnGetAsync(int? id)
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

                        if (wybranyKonkurs.ZgloszoneKolekcje.Count() > 0)
                        {
                            ZgloszoneKolekcje = wybranyKonkurs.ZgloszoneKolekcje.ToList();
                        }
                    }
                }
            }
        }
    }
}
