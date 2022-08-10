using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHercenskie.Data;
using KanarkiHercenskie.Models;

namespace KanarkiHercenskie.Pages.Przesluchania
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Przesluchanie> Przesluchanie { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Przesluchania != null)
            {
                Przesluchanie = await _context.Przesluchania
                .Include(p => p.PrzesluchiwanaKolekcja)
                .Include(p => p.PrzesluchiwanaKolekcja.Wlasciciel)
                .Include(p => p.PrzesluchiwanaKolekcja.Konkurs)
                .ToListAsync();
            }
        }
    }
}
