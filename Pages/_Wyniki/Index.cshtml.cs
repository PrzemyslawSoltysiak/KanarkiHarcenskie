using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Wyniki
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Wynik> Wynik { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Wyniki != null)
            {
                Wynik = await _context.Wyniki
                .Include(w => w.PrzyznanoDla)
                .Include(w => w.PrzyznanoZa)
                .AsNoTracking().ToListAsync();
            }
        }
    }
}
