using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KanarkiHarcenskie.Data;
using KanarkiHarcenskie.Models;

namespace KanarkiHarcenskie.Pages.Przesluchania
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Przesluchanie Przesluchanie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Przesluchania == null)
            {
                return NotFound();
            }

            var przesluchanie = await _context.Przesluchania.Include(p => p.PrzesluchiwanaKolekcja)
                .Include(p => p.PrzesluchiwanaKolekcja.Wlasciciel)
                .Include(p => p.PrzesluchiwanaKolekcja.Konkurs).FirstOrDefaultAsync(m => m.ID == id);

            if (przesluchanie == null)
            {
                return NotFound();
            }
            else 
            {
                Przesluchanie = przesluchanie;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Przesluchania == null)
            {
                return NotFound();
            }
            var przesluchanie = await _context.Przesluchania.FindAsync(id);

            if (przesluchanie != null)
            {
                Przesluchanie = przesluchanie;
                _context.Przesluchania.Remove(Przesluchanie);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
